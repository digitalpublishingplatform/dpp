using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class VideoPlayerViewModel
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Src { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string AdditionalInfo { get; set; }
        public bool IsSmoothStreaming {
            get {
                return !String.IsNullOrEmpty(Src) && Src.Contains(".ism/manifest");
            }
        }
    }
}