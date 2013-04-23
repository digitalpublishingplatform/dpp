using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels.Api
{
    public class PublicationViewModel
    {
        public PublicationViewModel() {
            Issues = new List<IssueViewModel>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? ModifiedUtc { get; set; }
        public IList<IssueViewModel> Issues { get; set; }
    }
}