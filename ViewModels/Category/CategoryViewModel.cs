using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public DateTime? ModifiedUtc { get; set; }
        public string Owner { get; set; }
        public string ModifiedLongFormat
        {
            get { return (ModifiedUtc.HasValue) ? String.Format("{0:f}", ModifiedUtc) : String.Empty; }
        }
        public string CreatedLongFormat
        {
            get { return (CreatedUtc.HasValue) ? String.Format("{0:f}", CreatedUtc) : String.Empty; }
        }
        public bool Published { get; set; }
        public bool Checked { get; set; }
    }
}