using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPublishingPlatform.Models;

namespace DigitalPublishingPlatform.ViewModels
{
    public class EncodingPresetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MediaTarget Target { get; set; }
        public string ShortDescription { get; set; }
        public int Width { get; set; }
        public bool Disabled { get; set; }
    }
}
