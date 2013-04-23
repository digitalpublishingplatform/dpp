using System.Collections.Generic;

namespace DigitalPublishingPlatform.ViewModels
{
    public class EditVideosViewModel
    {
        public IEnumerable<MediaItemViewModel> Videos { get; set; }
        public int[] Ids { get; set; }
    }
}
