using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels.Category
{
    public class CategoryLightViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public int? CategoryDisplayOrder { get; set; }
        public bool Checked { get; set; }
    }
}