using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class MediaItemListViewModel
    {
        public MediaItemListViewModel() {
            MediaItems = new List<MediaItemViewModel>();
        }
        public IEnumerable<MediaItemViewModel> MediaItems { set; get; }
        public dynamic Pager { get; set; }
        public int Length { get; set; }
        public string Find { get; set; }
    }
}