using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class IssueItemViewModel
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? ModifiedUtc { get; set; }
        public int Id { get; set; }
    }
}