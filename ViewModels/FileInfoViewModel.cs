using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;

namespace DigitalPublishingPlatform.ViewModels
{
    public class FileInfoViewModel
    {
        public string Filename { get; set; }
        public long Size { get; set; }
        public string MimeType { get; set; }
        public string Url { get; set; }
        public string AssetId { get; set; }        
    }
}