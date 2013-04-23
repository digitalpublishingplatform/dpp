using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.ViewModels.Api;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services {
    public class VideoService : IVideoService {
        private readonly IRepository<VideoRecord> _videoRepository;
        private readonly IRepository<VideoMediaItemRecord> _videoMediaItemRepository;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<EncodedMediaRecord> _encodedMediaRepository;
        private readonly IRepository<MediaItemRecord> _mediaItemRepository;
        private readonly ISiteService _siteService;
        private readonly IShapeFactory _shapeFactory;

        public VideoService(IRepository<VideoRecord> videoRepository, 
            IRepository<VideoMediaItemRecord> videoMediaItemRepository,
            IOrchardServices orchardServices, 
            IRepository<EncodedMediaRecord> encodedMediaRepository,
            IRepository<MediaItemRecord> mediaItemRepository, 
            ISiteService siteService,
            IShapeFactory shapeFactory) {
            _videoRepository = videoRepository;
            _videoMediaItemRepository = videoMediaItemRepository;
            _orchardServices = orchardServices;
            _encodedMediaRepository = encodedMediaRepository;
            _mediaItemRepository = mediaItemRepository;
            _siteService = siteService;
            _shapeFactory = shapeFactory;
        }

        public void UpdateVideosForContentItem(ContentItem item, int[] ids)
        {
            var currentVideoRecord = item.As<VideoPart>().Record;
            var oldMedia = _videoMediaItemRepository.Fetch(r => r.VideoRecord == currentVideoRecord);            
            var newVideos = _mediaItemRepository.Table.Where(mi => ids.Contains(mi.Id)).ToList();            
            
            foreach (var videoMediaItemRecord in oldMedia)
            {
               _videoMediaItemRepository.Delete(videoMediaItemRecord);
            }            
            for (var position = 0; position < ids.Count(); position++) {
                var mediaItem = newVideos.First(v => v.Id == ids[position]);
                _videoMediaItemRepository.Create(new VideoMediaItemRecord
                {
                    VideoRecord = currentVideoRecord,
                    MediaItemRecord = mediaItem,
                    Position = position
                });
            }           
        }

        public IEnumerable<MediaItemViewModel> GetMediaList(int videoPartId) {

            var mediaItemIds = _videoMediaItemRepository.Table.Where(vm =>
                                                                     vm.VideoRecord.Id == videoPartId)
                                                        .OrderBy(vm => vm.Position).ToList()
                                                        .Select(vm => new {MediaItemId = vm.MediaItemRecord.Id, Position = vm.Position}).ToList();
                                                        

            return _orchardServices.ContentManager.HqlQuery()
                .ForPart<MediaItemPart>()
                .ForVersion(VersionOptions.Latest)
                .List()
                .Where(x => mediaItemIds.Select(y => y.MediaItemId).Any(mi => mi == x.Id) && x.IsPublished())
                .Select(x => new MediaItemViewModel {
                    Id = x.Id,
                    Description = x.Description,
                    Filename = x.Filename,
                    MimeType = x.MimeType,
                    Published = x.IsPublished(),
                    Size = x.Size,
                    Title = x.Title,
                    Url = x.Url,
                    DefaultThumbnailUrl = x.DefaultThumbnailUrl,
                    Modified = x.ModifiedUtc,
                    Created = x.CreatedUtc,
                    Owner = x.Owner,
                    Position = mediaItemIds.First(z => z.MediaItemId == x.Id).Position
                }).OrderBy(x => x.Position);            
        }

        public IEnumerable<VideoViewModel> GetVideoList(int articleId)
        {
            var mediaItemIds = _videoMediaItemRepository.Table.Where(vm => vm.VideoRecord.Id == articleId).Select(vm => vm.MediaItemRecord.Id);
                        
            var result = _orchardServices.ContentManager.HqlQuery()
                .ForPart<MediaItemPart>()
                .ForVersion(VersionOptions.Published)
                .List()
                .Where(x => mediaItemIds.Any(mi => mi == x.Id) && x.IsPublished())                
                .Select(x => new VideoViewModel
                {          
                    Id = x.Id,
                    Description = x.Description,                    
                    Title = x.Title,
                    ThumbnailUrl = x.DefaultThumbnailUrl,
                    EncodedVideos = GetEncodedVideoList(x.Id).Where(v => v.EncodingPreset.ToLowerInvariant().Contains("win"))
                    
                });

            return result;
        }

        public IEnumerable<EncodedVideoViewModel> GetEncodedVideoList(int mediaItemId) {
            var model = _encodedMediaRepository.Table
                                          .Where(e => e.MediaItem.Id == mediaItemId && e.Status == "Finished")
                                          .OrderByDescending(e => e.EncodingPreset.Target)
                                          .ThenByDescending(e => e.EncodingPreset.Id)
                                          .Select(x => new EncodedVideoViewModel {
                                              AspectRatio = x.AspectRatio,
                                              EncodingPreset = x.EncodingPreset.ShortDescription,
                                              Height = x.Height,
                                              Width = x.Width,
                                              PresetWidth = x.EncodingPreset.Width,
                                              Status = x.Status,
                                              Url = x.Url
                                          });
            return model;
        }

        public VideoPickerViewModel FindVideo(VideoPickerViewModel model, PagerParameters pagerParameters) {
            var find = model.Find ?? "";
            var mediaItemQuery = _orchardServices.ContentManager
                                                 .HqlQuery()
                                                 .ForPart<MediaItemPart>()
                                                 .ForVersion(VersionOptions.Published) 
                                                 .List()
                                                 .Where(m => find == "" 
                                                             || m.Title.ToLowerInvariant().Contains(find)
                                                             || m.Filename.ToLowerInvariant().Contains(find)
                                                             || m.MimeType.ToLowerInvariant().Contains(find))
                                                 .OrderBy(m => m.Title);

            var count = mediaItemQuery.Count();
            // when is selected the option 'Show All' in pagination,the page size parameter is equal to 0
            if (pagerParameters.PageSize == 0)
            {
                // assign at page size parameter value the total number of records found
                pagerParameters.PageSize = count;
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            dynamic shape = _shapeFactory;

            model = new VideoPickerViewModel
            {
                Find = find,
                Pager = shape.Pager(pager).TotalItemCount(count),
                SearchResult = mediaItemQuery.Select(x => new MediaItemViewModel
                {
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
                    HasEncodedMedia = _encodedMediaRepository.Table.Any(encoded => encoded.MediaItem.Id == x.Id && encoded.EncodingPreset.MediaType == MediaType.Video),
                    Modified = x.ModifiedUtc,
                    Created = x.CreatedUtc,
                    Owner = x.Owner
                }).Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize == 0 ? count : pager.PageSize).ToList()
            };

            return model;
        }


        public EditVideosViewModel GetVideoList(int[] ids) {
            var mediaItemQuery = _orchardServices.ContentManager
                                                 .HqlQuery()
                                                 .ForPart<MediaItemPart>()
                                                 .ForVersion(VersionOptions.Published)
                                                 .List()
                                                 .Where(m => ids.Contains(m.Id)).Select(x => new MediaItemViewModel {
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
                                                 });
            return new EditVideosViewModel {
                Videos = mediaItemQuery
            };

        }
    }

}