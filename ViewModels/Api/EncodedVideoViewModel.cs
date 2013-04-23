using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalPublishingPlatform.ViewModels.Api
{
    public class EncodedVideoViewModel
    {
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string EncodingPreset { get; set; }
        public string Status { set; get; }        
        public int PresetWidth { get; set; }        
        public bool IsFinished
        {
            get { return Status == "Finished"; }
        }

        public decimal AspectRatio { get; set; }
    }
}
