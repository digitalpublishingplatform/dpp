using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class AzureUploadResultViewModel
    {
        public string AssetId { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
    }
}