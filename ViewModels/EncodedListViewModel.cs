using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class EncodedListViewModel
   {        
        public string Title { get; set; }
        public string Filename { get; set; }
        public int MediaItemId { get; set; }        
        public IList<EncodedMediaViewModel> EncodedMediaList { get; set; }
        public IEnumerable<string> SelectedFormats { get; set; }
        public dynamic Pager { get; set; }
    }
}