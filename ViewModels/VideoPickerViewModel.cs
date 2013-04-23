using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class VideoPickerViewModel
    {
        public string Find { get; set; }
        public dynamic Pager { get; set; }
        public IEnumerable<MediaItemViewModel> SearchResult { get; set; }
    }
}