using System;
using DigitalPublishingPlatform.Models;
using Orchard.Data.Conventions;

namespace DigitalPublishingPlatform.ViewModels {
    public class EncodedMediaViewModel
    {
        public int Id { set; get; }        
        public string Url { get; set; }
        public string EncodingPreset { get; set; }        
        public string Status { set; get; }        
        public string Metadata { set; get; }
        public DateTime? CreatedUtc { set; get; }
        public DateTime? ModifiedUtc { get; set; }
        public string Owner { get; set; }
        public string JobErrorMessage { get; set; }
        public int PresetWidth { get; set; }
        public MediaTarget MediaTarget { get; set; }
        public bool IsFinished {
            get { return Status == "Finished"; }
        }
    }
}