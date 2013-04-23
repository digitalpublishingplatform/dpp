using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using Microsoft.WindowsAzure.MediaServices.Client;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using System.Linq;
using System.Diagnostics;
using System.Web;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services
{
    public class MediaServicesService: IMediaServicesService {
        private CloudMediaContext CloudServiceContext {
            get {
                return new CloudMediaContext(_config.MediaServiceAccount, _config.MediaServiceKey);
            }
        }
        private readonly IConfig _config;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IRepository<EncodedMediaRecord> _encodedMediaRepository;
        private readonly IRepository<MediaItemRecord> _mediaItemRepository;
        private readonly IRepository<EncodingPresetRecord> _encodingPresetRepository;
        private readonly IRepository<ThumbnailRecord> _thumbnailRepository;
        private readonly IRepository<VideoMediaItemRecord> _videoMediaItemRepository;
        private readonly IWorkContextAccessor _workContextAccessor;        
        private readonly IOrchardServices _orchardServices;
        private readonly IShapeFactory _shapeFactory;
        private readonly ISiteService _siteService;
        private static object _locker = new object();
        private readonly ILogger _logger;

        public MediaServicesService(IOrchardServices orchardServices, 
            IShapeFactory shapeFactory, 
            ISiteService siteService, 
            IConfig config, 
            IBlobStorageService blobStorageService, 
            IRepository<EncodedMediaRecord> encodedMediaRepository, 
            IRepository<MediaItemRecord> mediaItemRepository, 
            IRepository<EncodingPresetRecord> encodingPresetRepository, 
            IRepository<ThumbnailRecord> thumbnailRepository,
            IRepository<VideoMediaItemRecord> videoMediaItemRepository,
           IWorkContextAccessor workContextAccessor)
        {
            _config = config;
            _blobStorageService = blobStorageService;
            _encodedMediaRepository = encodedMediaRepository;
            _mediaItemRepository = mediaItemRepository;
            _encodingPresetRepository = encodingPresetRepository;
            _thumbnailRepository = thumbnailRepository;
            _videoMediaItemRepository = videoMediaItemRepository;
            _workContextAccessor = workContextAccessor;            
            _orchardServices = orchardServices;
            _shapeFactory = shapeFactory;
            _siteService = siteService;
            _logger = NullLogger.Instance;
        }

        public MediaItemListViewModel GetAllItems(MediaItemListViewModel model, PagerParameters pagerParameters) {
            var find = model.Find ?? "";
            var mediaItemQuery = _orchardServices.ContentManager
                       .Query<MediaItemPart, MediaItemRecord>()                       
                       .ForVersion(VersionOptions.Latest)
                       .List()
                       .Where(m => find == "" || m.Title.ToLowerInvariant().Contains(find));

            var count = mediaItemQuery.Count();
            // when is selected the option 'Show All' in pagination,the page size parameter is equal to 0
            if (pagerParameters.PageSize == 0)
            {
                // assign at page size parameter value the total number of records found
                pagerParameters.PageSize = count;
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            dynamic shape = _shapeFactory;

            model = new MediaItemListViewModel {
                Pager = shape.Pager(pager).TotalItemCount(count),
                MediaItems = mediaItemQuery.Select(x => new MediaItemViewModel {
                    Title = x.Title,
                    Description = x.Description,
                    Filename = x.Filename,
                    Published = x.Published,
                    Type = x.Type,
                    Url = x.Url,
                    MimeType = x.MimeType,
                    Id = x.Id,
                    Size = x.Size,
                    DefaultThumbnailUrl = x.DefaultThumbnailUrl,
                    Modified = x.ModifiedUtc,
                    Created = x.CreatedUtc,
                    Owner = x.Owner
                }).Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize == 0 ? count : pager.PageSize).ToList()
            };

            return model;
        }

        public EncodingViewModel InitEncodingViewModel(int mediaItemId)
        {
            var mediaItemPart = _orchardServices.ContentManager.Get<MediaItemPart>(mediaItemId, VersionOptions.Latest);
            var currentEncodingList = _encodedMediaRepository.Table.Where(e => 
                e.MediaItem.Id == mediaItemId 
                && e.Status != JobState.Error.ToString() 
                && e.Status != JobState.Canceled.ToString()).Select(e => e.EncodingPreset.Id);
            return new EncodingViewModel
            {
                LongTitle = mediaItemPart.LongTitle,
                Formats = _encodingPresetRepository.Table.Select( e => new EncodingPresetViewModel {
                    Description = e.Description,
                    Id = e.Id,
                    Name = e.Name,
                    ShortDescription = e.ShortDescription,
                    Target = e.Target,
                    Width = e.Width,
                    Disabled = currentEncodingList.Any(c => c == e.Id)
                }).ToList(),
                MediaItemId = mediaItemId,
                Status = "Pending"
            };
        }

        private IAsset CreateEmptyAsset(string assetName, AssetCreationOptions assetCreationOptions)
        {            
            return CloudServiceContext.Assets.Create(assetName, assetCreationOptions);            
        }

        public AzureUploadResultViewModel CreateAssetAndUploadSingleFile(string filename, string oldContainerName, string alternateId)
        {                        
            var assetName = filename.Replace('.', '-') + "-Source";
            var asset = CreateEmptyAsset(assetName, AssetCreationOptions.None);
            asset.AlternateId = alternateId; 
            asset.Update();
            asset.AssetFiles.Create(filename);            
            var accessPolicy = CloudServiceContext.AccessPolicies.Create(assetName, TimeSpan.FromDays(3), AccessPermissions.Write | AccessPermissions.List);
            var locator = CloudServiceContext.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);
            var assetUrl = new Uri(locator.BaseUri);
            var newContainerName = assetUrl.Segments[assetUrl.Segments.Count() - 1];
            long filesize;
            _blobStorageService.CopyBetweenContainers(filename, oldContainerName, newContainerName, out filesize);
            locator.Delete();
            accessPolicy.Delete();
            accessPolicy = CloudServiceContext.AccessPolicies.Create(assetName, TimeSpan.FromDays(365 * 10), AccessPermissions.Read);
            locator = CloudServiceContext.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);            
            var file = asset.AssetFiles.First();            
            file.ContentFileSize = filesize;
            file.MimeType = filename.MimeType();
            file.Update();            
            return new AzureUploadResultViewModel
            {
                AssetId = asset.Id,
                Url = HttpUtility.UrlPathEncode(locator.BaseUri + "/" + file.Name) + locator.ContentAccessComponent,
                Size = file.ContentFileSize
            };
        }

        public bool UploadChunk(string filename, int chunk, int chunksLength, Stream inputStream, string token) {
            var uploaded = _blobStorageService.UploadChunk(filename, chunk, chunksLength, inputStream, token);
            if (chunk == chunksLength - 1 && uploaded) {
                CreateAssetAndUploadSingleFile(filename, token, token);
            }
            return uploaded;
        }

        public void DeleteByAssetId(string assetId, CloudMediaContext context = null) {
            if (context == null) {
                context = CloudServiceContext;
            }
            var assets = context.Assets.Where(a => a.Id == assetId);
            foreach (var asset in assets) {
                asset.DeleteAsync();
            }
        }

        private IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName, CloudMediaContext context = null)
        {
            if (context == null) {
                context = CloudServiceContext;
            }
            var processor = context.MediaProcessors.Where(p => p.Name == mediaProcessorName)
                                               .ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();
            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        public Task<IJob> CreateEncodingJob(string assetId, int encodingId, string username)
        {
            return Task.Factory.StartNew(() => BeginEncodingJob(assetId, encodingId, username));
        }        

        public IJob BeginEncodingJob(string assetId, int encodingId, string username) {
            lock (_locker)
            {                
                using (var scope = _workContextAccessor.CreateWorkContextScope()) {
                    var transactionManager = scope.Resolve<ITransactionManager>();
                    transactionManager.Demand();
                    var mediaItemRep = scope.Resolve<IRepository<MediaItemRecord>>();
                    var encodingPresetRep = scope.Resolve<IRepository<EncodingPresetRecord>>();
                    var encodedMediaRep = scope.Resolve<IRepository<EncodedMediaRecord>>();
                    var config = scope.Resolve<IConfig>();
                    var cloudServiceContext = new CloudMediaContext(config.MediaServiceAccount, config.MediaServiceKey);
                    try
                    {
                        var mediaItem = mediaItemRep.Table.FirstOrDefault(mi => mi.AssetId == assetId);
                        if (mediaItem == null)
                        {
                            throw new Exception("MediaItem with AssetId '" + assetId + "' not found.");
                        }

                        var encodingPreset = encodingPresetRep.Get(encodingId);
                        if (encodingPreset == null)
                        {
                            throw new Exception("Encoding preset with id:" + encodingId + "not found.");
                        }

                        var mediaEncodedList = encodedMediaRep.Table.Where(e => e.EncodingPreset.Id == encodingId && e.MediaItem.Id == mediaItem.Id);
                        foreach (var mediaEncoded in mediaEncodedList)
                        {
                            encodedMediaRep.Delete(mediaEncoded);
                            DeleteJob(mediaEncoded.JobId, cloudServiceContext);
                            DeleteAsset(GetAsset(mediaEncoded.AssetId, cloudServiceContext));
                        }

                        var encodedMediaRecord = new EncodedMediaRecord
                        {
                            MediaItem = mediaItem,
                            EncodingPreset = encodingPreset,
                            Status = "Initializing",
                            CreatedUtc = DateTime.UtcNow,
                            ModifiedUtc = DateTime.UtcNow,
                            Owner = username
                        };

                        encodedMediaRep.Create(encodedMediaRecord);
                        encodedMediaRep.Flush();

                        var asset = cloudServiceContext.Assets.Where(a => a.Id == assetId).ToList().FirstOrDefault();
                        if (asset == null)
                        {
                            throw new Exception("Asset with id '" + assetId + "' not found.");
                        }
                        var job = cloudServiceContext.Jobs.Create(String.Format("Job for '{0}' using '{1}'", asset.Name, encodingPreset.ShortDescription));
                        encodedMediaRecord.Status = job.State.ToString();
                        var processor = GetLatestMediaProcessorByName("Windows Azure Media Encoder", cloudServiceContext);
                        var file = asset.AssetFiles.ToList().FirstOrDefault();
                        if (file == null)
                        {
                            throw new Exception("File for asset '" + asset.Name + "' not found.");
                        }
                        var task = job.Tasks.AddNew(String.Format("Encoding task from file {0} to '{1}'", file.Name, encodingPreset.Name), processor, encodingPreset.Name, TaskOptions.ProtectedConfiguration);
                        task.InputAssets.Add(asset);
                        task.OutputAssets.AddNew(file.Name.Replace('.', '-') + "-Output-" + encodingPreset.ShortDescription, AssetCreationOptions.None);
                        job.Submit();
                        if (job.State == JobState.Error)
                        {
                            encodedMediaRecord.Status = job.State.ToString();
                            encodedMediaRecord.JobErrorMessage = "";
                            foreach (var message in job.Tasks.Where(t => t.ErrorDetails.Any()).SelectMany(t => t.ErrorDetails).Select(ed => ed.Message))
                            {
                                encodedMediaRecord.JobErrorMessage += message + "; ";
                            }

                        }
                        else
                        {
                            encodedMediaRecord.Status = job.State.ToString();
                        }
                        encodedMediaRecord.JobId = job.Id;
                        encodedMediaRep.Update(encodedMediaRecord);
                        encodedMediaRep.Flush();                        
                        return job;         
                        
                    }
                    catch (Exception e)
                    {     
                        transactionManager.Cancel();                   
                        _logger.Error(e, "Error while processing background task");
                        return null;
                    }
                }                    
            }
        }

        private void DeleteJob(string jobId, CloudMediaContext context = null)
        {
            var jobDeleted = false;
            while (!jobDeleted)
            {
                // Get an updated job reference.  
                var job = GetJob(jobId, context);
                if (job == null) {
                    return;
                }
                // Check and handle various possible job states. You can 
                // only delete a job whose state is Finished, Error, or Canceled.   
                // You can cancel jobs that are Queued, Scheduled, or Processing,  
                // and then delete after they are canceled.
                switch (job.State)
                {
                    case JobState.Finished:
                    case JobState.Canceled:
                    case JobState.Error:
                        // Job errors should already be logged by polling or event 
                        // handling methods such as CheckJobProgress or StateChanged.
                        // You can also call job.DeleteAsync to do async deletes.
                        job.Delete();
                        Console.WriteLine("Job has been deleted.");
                        jobDeleted = true;
                        break;
                    case JobState.Canceling:
                        Console.WriteLine("Job is cancelling and will be deleted "
                            + "when finished.");
                        Console.WriteLine("Wait while job finishes canceling...");
                        Thread.Sleep(5000);
                        break;
                    case JobState.Queued:
                    case JobState.Scheduled:
                    case JobState.Processing:
                        job.Cancel();
                        Console.WriteLine("Job is scheduled or processing and will "
                            + "be deleted.");
                        break;
                    default:
                        break;
                }

            }
        }

        private void DeleteAsset(IAsset asset, CloudMediaContext context = null)
        {
            if (asset == null)
            {
                return;                
            }

            if (context == null) {
                context = CloudServiceContext;
            }
            // delete the asset
            asset.Delete();
            // Verify asset deletion
            if (GetAsset(asset.Id, context) == null)                
                Debug.WriteLine("asset deleted");
        }

        public IJob GetJob(string jobId, CloudMediaContext context = null)
        {
            // Use a Linq select query to get an updated 
            // reference by Id. 
            if (context == null) {
                context = CloudServiceContext;
            }
            var jobInstance = 
                from j in context.Jobs
                where j.Id == jobId
                select j;
            // Return the job reference as an Ijob. 
            return jobInstance.FirstOrDefault();
        }

        private IAsset GetAsset(string assetId, CloudMediaContext context = null)
        {
            if (context == null)
            {
                context = CloudServiceContext;
            }
            // Use a LINQ Select query to get an asset.
            var assetInstance =
                from a in context.Assets
                where a.Id == assetId
                select a;
            // Reference the asset as an IAsset.
            return assetInstance.FirstOrDefault();
        }

        public FileInfoViewModel GetFileInfoByToken(string token)
        {
            var asset = CloudServiceContext.Assets.Where(a => a.AlternateId == token).ToList().FirstOrDefault();
            if (asset == null) return null;
            var locator = asset.Locators.First();
            var file = asset.AssetFiles.First();            
            return new FileInfoViewModel
            {
                Filename = file.Name,
                Size = file.ContentFileSize / 1024,
                MimeType = file.MimeType,
                Url = HttpUtility.UrlPathEncode(locator.BaseUri + "/" + file.Name) + locator.ContentAccessComponent,
                AssetId = asset.Id,                
            };
        }


        public EncodedListViewModel GetEncodedList(int id, PagerParameters pagerParameters) {            
            var mediaItemPart = _orchardServices.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItemPart == null) {
                throw new Exception("Inexistent content");
            }
            var mediaItemQuery = _encodedMediaRepository.Table.Where(e => e.MediaItem.Id == id).OrderByDescending(e => e.EncodingPreset.Target).ThenByDescending(e => e.EncodingPreset.Id);

            var count = mediaItemQuery.Count();
            if (pagerParameters.PageSize == 0)
            {
                pagerParameters.PageSize = count;
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            dynamic shape = _shapeFactory;
            
            var model = new EncodedListViewModel {
                MediaItemId = mediaItemPart.Id,
                Title = mediaItemPart.Title,
                Filename = mediaItemPart.Filename,
                Pager = shape.Pager(pager).TotalItemCount(count),
                EncodedMediaList = mediaItemQuery.Select(x => new EncodedMediaViewModel {
                    Id = x.Id,
                    Url = x.Url,
                    EncodingPreset = (x.EncodingPreset != null) ? x.EncodingPreset.ShortDescription : String.Empty,
                    Status = x.Status,
                    Metadata = x.Metadata,
                    JobErrorMessage = x.JobErrorMessage,
                    CreatedUtc = x.CreatedUtc,
                    ModifiedUtc = x.ModifiedUtc,
                    Owner = x.Owner,
                    PresetWidth = x.Width,
                    MediaTarget = (x.EncodingPreset != null) ? x.EncodingPreset.Target : MediaTarget.Unknown,
                }).Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize == 0 ? count : pager.PageSize).ToList()
            };

            return model;
        }

        public Tuple<string, List<string>, int> ImageList(int id) {
            var encodedMedia = _encodedMediaRepository.Get(id);            
            var title = "";            
            if (encodedMedia.MediaItem != null) {
                var mediaItemPart = _orchardServices.ContentManager.Get<MediaItemPart>(encodedMedia.MediaItem.Id, VersionOptions.Latest);
                title = mediaItemPart.LongTitle;                
            }
            return new Tuple<string, List<string>, int>(title, _thumbnailRepository.Table.Where(t => t.EncodedMedia.Id == id).OrderBy(t => t.Url).Select(t => t.Url).ToList(), id);
        }

        public VideoPlayerViewModel GetEncodedItem(int id)
        {
            var encodedMedia = _encodedMediaRepository.Get(id);
            
            if (encodedMedia == null) {
                return null;
            }

            var mediaItemPart = _orchardServices.ContentManager.Get<MediaItemPart>(encodedMedia.MediaItem.Id, VersionOptions.Latest);
            if (mediaItemPart== null) {
                return null;
            }

            return new VideoPlayerViewModel {
                //Height = encodedMedia.Height,
                //Width = encodedMedia.Width,
                Height = 420,
                Width = (int)(420 * encodedMedia.AspectRatio),
                Src = encodedMedia.Url,
                Title = mediaItemPart.LongTitle,
                Type = mediaItemPart.MimeType,
                AdditionalInfo = encodedMedia.EncodingPreset.ShortDescription
            };
        }

        public void SetDefaultThumbnail(int id, string thumbnailUrl) {
            var encodedMedia = _encodedMediaRepository.Get(id);
            var mediaItemRecord = _mediaItemRepository.Get(encodedMedia.MediaItem.Id);
            mediaItemRecord.DefaultThumbnailUrl = thumbnailUrl;
            _mediaItemRepository.Update(mediaItemRecord);            
        }


        public void DeleteEncoded(int id)
        {
            var encodedMedia = _encodedMediaRepository.Get(id);
            if (encodedMedia == null) {
                return;
            }

            var mediaItem = encodedMedia.MediaItem;
            
            _encodedMediaRepository.Delete(encodedMedia);

            var asset = GetAsset(encodedMedia.AssetId);
            DeleteAsset(asset);
                        
            if (encodedMedia.EncodingPreset.MediaType == MediaType.Image)
            {
                var thumbnails = _thumbnailRepository.Table.Where(t => t.EncodedMedia.Id == encodedMedia.Id);
                foreach (var thumbnailRecord in thumbnails)
                {
                    _thumbnailRepository.Delete(thumbnailRecord);
                }

                mediaItem.DefaultThumbnailUrl = String.Empty;
                _mediaItemRepository.Update(mediaItem);
            }                        
        }

        public int GetCurrentEncodingCount(int mediaItemId) {
            var encodingCount = _encodedMediaRepository.Table.Count(e => 
                                    e.MediaItem.Id == mediaItemId
                                    && e.Status != JobState.Error.ToString()
                                    && e.Status != JobState.Canceled.ToString());
            return encodingCount;
        }

        public List<string> GetRelatedContentTitle(int mediaItemId) {
            var list = _orchardServices.ContentManager
                                        .Query()
                                        .ForPart<VideoPart>()
                                        .ForVersion(VersionOptions.Latest)                                        
                                        .Where<VideoRecord>(vp => vp.MediaItemList.Any(v => v.MediaItemRecord.Id == mediaItemId)).ForPart<ArticlePart>().List()
                                        .Select(vp => vp.Title).Distinct().OrderBy(a => a).ToList();

            return list;
        }

        public void RemoveMediaItem(MediaItemPart mediaItemPart) {
            var toDelete = _videoMediaItemRepository.Table.Where(vm => vm.MediaItemRecord != null && vm.MediaItemRecord.Id == mediaItemPart.Id);
            foreach (var videoMediaItemRecord in toDelete) {
                _videoMediaItemRepository.Delete(videoMediaItemRecord);
            }
            var mediaItem = mediaItemPart.Record;
            if (mediaItem != null) {
                DeleteByAssetId(mediaItem.AssetId);
                
                var encodedItems = _encodedMediaRepository.Table.Where(em => em.MediaItem.Id == mediaItem.Id);
                foreach (var encodedMediaRecord in encodedItems) {
                    DeleteByAssetId(encodedMediaRecord.AssetId);
                    _encodedMediaRepository.Delete(encodedMediaRecord);
                    if (encodedMediaRecord.EncodingPreset.MediaType != MediaType.Image) {
                        continue;
                    }
                    var record = encodedMediaRecord;
                    var thumbnails = _thumbnailRepository.Table.Where(t => t.EncodedMedia.Id == record.Id);
                    foreach (var thumbnailRecord in thumbnails) {
                        _thumbnailRepository.Delete(thumbnailRecord);
                    }
                }

                _orchardServices.ContentManager.Remove(mediaItemPart.ContentItem);
            }
        }
    }
}