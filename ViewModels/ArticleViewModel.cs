using DigitalPublishingPlatform.ViewModels.Api;
using System;
using System.Collections.Generic;

namespace DigitalPublishingPlatform.ViewModels {
    public class ArticleViewModel {
        public int Id {set; get;}                    
        public string Title {set; get;}
        public string Text {set; get;}
        public DateTime? CreatedUtc { get; set; }
        public DateTime? ModifiedUtc { get; set; }
        public DateTime? PublishedUtc {set; get;}
        public string Author { set; get; }
        public string ImageUrl { set; get; }
        public IEnumerable<VideoViewModel> Videos {set; get;}        
        public IEnumerable<string> Categories { set; get; }
        public List<string> Images { set; get; }
        public int Position { set; get; }
    }
}