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
    public class VideoPickerController : Controller
    {
        private readonly IVideoService _videoService;

        public VideoPickerController(IVideoService videoService) {
            _videoService = videoService;
        }

        [HttpGet]
        public ActionResult Index(PagerParameters pagerParameters) {
            return View(_videoService.FindVideo(new VideoPickerViewModel{Find = ""}, pagerParameters));
        }
    
        [HttpPost]
        public ActionResult Index(VideoPickerViewModel model, int[] ids, PagerParameters pagerParameters)
        {
            return View(_videoService.FindVideo(model, pagerParameters));
        }
        [HttpGet]
        public ActionResult VideoInfo(int[] ids)
        {
            return PartialView("_VideoInfo", _videoService.GetVideoList(ids));
        }        
        
    }
}
