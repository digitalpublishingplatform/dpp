using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.UI.Navigation;
using Orchard.UI.Admin;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.Services;

namespace DigitalPublishingPlatform.Controllers
{
    [Themed(false), Admin, ValidateInput(false)]
    public class ImagePickerController : Controller
    {
        private readonly IImageService _imageService;

        public ImagePickerController(IImageService imageService) {
            _imageService = imageService;
        }

        [HttpGet]
        public ActionResult ImageInfo(string[] urls) {
            var model = new ImageSetViewModel();
            model.Urls = urls;
            return PartialView("_ImageInfo", model);
        }        
        
    }
}
