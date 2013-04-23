using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels.Api
{
    public class VideoViewModel
    {
        public VideoViewModel() {
            EncodedVideos = new List<EncodedVideoViewModel>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public IEnumerable<EncodedVideoViewModel> EncodedVideos { get; set; }
        public string MainVideoUrl {
            get {
                var mainEncoding = EncodedVideos.OrderByDescending(e => e.PresetWidth).FirstOrDefault();
                return mainEncoding != null ? mainEncoding.Url : "";
            }
        }
    }
}