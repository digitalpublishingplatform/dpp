using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.ViewModels
{
    public class CategoryArticleViewModel
    {
        public int CategoryId { set; get; }
        public string Name { get; set; }
        public int? CategoryArticleId { set; get; }
        public int? FistArticleId { set; get; }
        public int? SecondArticleId { set; get; }
        public int? ThirdArticleId { set; get; }
        public int? FourthArticleId { set; get; }
    }
}