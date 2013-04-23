using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Xml;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.Services;
using Microsoft.WindowsAzure.MediaServices.Client;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Services;
using Orchard.Tasks;
using System.Drawing;
using Orchard.Logging;

namespace DigitalPublishingPlatform {
    public class MediaServiceUpdater : IBackgroundTask {
        private readonly IRepository<EncodedMediaRecord> _encodedMediaRepository;
        private readonly IRepository<ThumbnailRecord> _thumbnailRepository;
        private readonly IRepository<MediaItemRecord> _mediaItemRepository;
        private readonly IConfig _config;
        private readonly IClock _clock;
        public IOrchardServices Services { get; set; }
        public ILogger _logger;
        private static object _locker = new object();
        public MediaServiceUpdater(IOrchardServices services, 
            IRepository<EncodedMediaRecord> encodedMediaRepository, 
            IRepository<ThumbnailRecord> thumbnailRepository, 
            IRepository<MediaItemRecord> mediaItemRepository, 
            IConfig config, 
            IClock clock) {
            Services = services;
            _encodedMediaRepository = encodedMediaRepository;
            _thumbnailRepository = thumbnailRepository;
            _mediaItemRepository = mediaItemRepository;
            _logger = NullLogger.Instance;
            _config = config;
            _clock = clock;
        }

        public void Sweep() {
                Refresh();             
        }
      
        private void Refresh() {
            lock (_locker) {
                
                var cloudMediaContext = new CloudMediaContext(_config.MediaServiceAccount, _config.MediaServiceKey);
                var query = _encodedMediaRepository.Table.Where(em =>
                                                                em.Status != JobState.Finished.ToString()
                                                                && em.Status != JobState.Error.ToString()
                                                                && em.Status != JobState.Canceled.ToString()
                                                                && em.JobId != null
                                                                && em.JobId != "").ToList();

                foreach (var encodedMediaRecord in query) {
                    var job = cloudMediaContext.Jobs.Where(j => j.Id == encodedMediaRecord.JobId).ToList().FirstOrDefault();
                    if (job == null) {
                        continue;
                    }
                    if (job.State.ToString() == encodedMediaRecord.Status) {
                        continue;
                    }
                    encodedMediaRecord.Status = job.State.ToString();
                    var outputAsset = job.OutputMediaAssets.FirstOrDefault();
                    if (outputAsset != null) {
                        encodedMediaRecord.AssetId = outputAsset.Id;

                        if (job.State == JobState.Finished) {
                            var locator = outputAsset.Locators.ToList().FirstOrDefault();
                            if (locator != null) {
                                locator.Delete();
                            }

                            var accessPolicy = cloudMediaContext.AccessPolicies.Create(outputAsset.Name, TimeSpan.FromDays(365*10), AccessPermissions.Read);
                            locator = cloudMediaContext.Locators.CreateLocator(LocatorType.Sas, outputAsset, accessPolicy);

                            if (encodedMediaRecord.EncodingPreset.MediaType == MediaType.Image) {
                                var thumbnails = _thumbnailRepository.Table.Where(t => t.EncodedMedia.Id == encodedMediaRecord.Id);
                                foreach (var thumb in thumbnails) {
                                    _thumbnailRepository.Delete(thumb);
                                }

                                var i = 0;
                                var mid = outputAsset.AssetFiles.Count()/2;
                                foreach (var file in outputAsset.AssetFiles) {
                                    var thumb = new ThumbnailRecord {
                                        EncodedMedia = encodedMediaRecord,
                                        Url = HttpUtility.UrlPathEncode(locator.BaseUri + "/" + file.Name) + locator.ContentAccessComponent
                                    };
                                    _thumbnailRepository.Create(thumb);
                                    if (i == mid) {
                                        var mediaItem = _mediaItemRepository.Get(encodedMediaRecord.MediaItem.Id);
                                        mediaItem.DefaultThumbnailUrl = thumb.Url;
                                        _mediaItemRepository.Update(mediaItem);
                                    }
                                    i++;
                                }

                                _thumbnailRepository.Flush();
                            }
                            else {
                                if (encodedMediaRecord.EncodingPreset.MediaType == MediaType.Video) {
                                    var metadata = outputAsset.AssetFiles.Where(f => f.Name.EndsWith("_metadata.xml")).ToList().FirstOrDefault();
                                    if (metadata != null) {
                                        var xdoc = new XmlDocument();
                                        xdoc.Load(HttpUtility.UrlPathEncode(locator.BaseUri + "/" + metadata.Name) + locator.ContentAccessComponent);
                                        encodedMediaRecord.Metadata = xdoc.InnerXml;
                                        //TODO: check this
                                        if (!SetMetadata(encodedMediaRecord, xdoc)) continue;
                                    }
                                    var mainFile = outputAsset.AssetFiles.Where(f => f.Name.EndsWith(".ism") || f.Name.EndsWith(".mp4")).ToList().FirstOrDefault();
                                    if (mainFile != null) {
                                        string filename;
                                        if (mainFile.Name.EndsWith(".ism")) {
                                            filename = mainFile.Name + "/manifest";
                                            locator.Delete();
                                            locator = cloudMediaContext.Locators.CreateLocator(LocatorType.OnDemandOrigin, outputAsset, accessPolicy);
                                            encodedMediaRecord.Url = HttpUtility.UrlPathEncode(locator.BaseUri + "/" + locator.ContentAccessComponent + "/" + filename);
                                        }
                                        else {
                                            filename = mainFile.Name;
                                            encodedMediaRecord.Url = HttpUtility.UrlPathEncode(locator.BaseUri + "/" + filename) + locator.ContentAccessComponent;
                                        }

                                    }
                                }
                            }
                        }
                    }

                    if (job.State == JobState.Error) {
                        encodedMediaRecord.Status = job.State.ToString();
                        encodedMediaRecord.JobErrorMessage = "";
                        foreach (var message in job.Tasks.Where(t => t.ErrorDetails.Any()).SelectMany(t => t.ErrorDetails).Select(ed => ed.Message)) {
                            encodedMediaRecord.JobErrorMessage += message + "; ";
                        }
                    }
                    encodedMediaRecord.ModifiedUtc = DateTime.UtcNow;
                    _encodedMediaRepository.Update(encodedMediaRecord);
                    //dont do flush this evict interlock
                    //_encodedMediaRepository.Flush();
                }
            }
        }

        private bool SetMetadata(EncodedMediaRecord encodedMediaRecord, XmlDocument metadata) {
            try {
                var sizeNode = metadata.SelectNodes("MediaItems/MediaItem/DisplayVideoSize").Item(0);
                encodedMediaRecord.Width = Convert.ToInt32(sizeNode.Attributes["Width"].Value);
                encodedMediaRecord.Height = Convert.ToInt32(sizeNode.Attributes["Height"].Value);
                var frameRate = metadata.SelectNodes("MediaItems/MediaItem/FrameRate").Item(0);
                encodedMediaRecord.Framerate = Convert.ToDecimal(frameRate.InnerText, CultureInfo.InvariantCulture);
                var aspectRatio = metadata.SelectNodes("MediaItems/MediaItem/AspectRatio").Item(0);
                encodedMediaRecord.AspectRatio = Convert.ToDecimal(aspectRatio.InnerText, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex) {
                _logger.Warning(ex, "Problem setting metadata for id=" + encodedMediaRecord.Id);
                return false;
            }
            
        }
    }
}