using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class PublicationItemListViewModel
    {
        public  PublicationItemListViewModel() {
            PublicationItems = new List<PublicationItemViewModel>();
        }
        public IEnumerable<PublicationItemViewModel> PublicationItems { set; get; }
        public dynamic Pager { get; set; }
        public string Find { get; set; }
    }
}