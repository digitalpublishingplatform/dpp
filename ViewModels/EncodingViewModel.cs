using System.Collections.Generic;
using System.Linq;
using DigitalPublishingPlatform.Models;

namespace DigitalPublishingPlatform.ViewModels {
    public class EncodingViewModel {
        public EncodingViewModel() {
            Formats = new List<EncodingPresetViewModel>();
        }
        public int MediaItemId { set; get; }
        public string Status { set; get; }
        public IEnumerable<EncodingPresetViewModel> Formats { set; get; }
        public IEnumerable<int> SelectedFormats { get; set; }

        public EncodingPresetViewModel ThumbnailEncoding {
            get { return Formats.FirstOrDefault(f => f.Target == MediaTarget.Thumbnail); }
        }

        public IEnumerable<EncodingPresetViewModel> Windows8Encoding
        {
            get { return Formats.Where(f => f.Target == MediaTarget.SmoothStreaming).OrderByDescending(f => f.Width); }
        }

        public IEnumerable<EncodingPresetViewModel> Html5Encoding
        {
            get { return Formats.Where(f => f.Target == MediaTarget.Html5).OrderByDescending(f => f.Width); }
        }

        public string LongTitle { get; set; }
    }
}