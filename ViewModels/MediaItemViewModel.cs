using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using System;

namespace DigitalPublishingPlatform.ViewModels
{
    public class MediaItemViewModel
    {                
        public string Title { get; set; }
        public string Description { get; set; }        
        public string Url { get; set; }
        public bool Published { get; set; }
        public MediaType Type { get; set; }
        public string MimeType { get; set; }
        public string Filename { get; set; }                
        public int Id { get; set; }
        public long Size { get; set; }
        public string DefaultThumbnailUrl { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Created { get; set; }
        public string ModifiedLongFormat {
            get { return (Modified.HasValue) ? String.Format("{0:f}", Modified) : String.Empty; }
        }
        public string CreatedLongFormat
        {
            get { return (Created.HasValue) ? String.Format("{0:f}", Created) : String.Empty; }
        }

        public string Owner { set; get; }

        public string FormattedSize {
            get { return Size.ToReadeableSize(); }
        }

        public int Position { set; get; }
        public bool HasEncodedMedia {set; get; }
    }
    
}
