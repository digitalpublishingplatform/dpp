using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.ViewModels.Category;

namespace DigitalPublishingPlatform.ViewModels
{
    public class EditCategoriesViewModel
    {
        public IList<CategoryLightViewModel> Categories { get; set; }        
    }
}