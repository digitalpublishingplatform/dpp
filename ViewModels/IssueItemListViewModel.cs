using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class IssueItemListViewModel
    {
        public IssueItemListViewModel()
        {
            IssueItems = new List<IssueItemViewModel>();
        }
        public IEnumerable<IssueItemViewModel> IssueItems { set; get; }
        public int PublicationId { set; get; }
        public dynamic Pager { get; set; }
        public string Find { get; set; }
    }
}