using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Services;
using Microsoft.WindowsAzure.MediaServices.Client;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.Models;
using Orchard.Tags.Models;
using Orchard.Core.Title.Models;
using Orchard.Themes;

namespace DigitalPublishingPlatform.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private readonly ISiteService _siteService;
        private readonly IMediaServicesService _mediaServicesService;
        private readonly IBlobStorageService _blobStorageService;

        public AdminController(IOrchardServices orchardServices, 
            ISiteService siteService, 
            IMediaServicesService mediaServicesService,  
            IBlobStorageService blobStorageService){
            _siteService = siteService;
            _mediaServicesService = mediaServicesService;
            _blobStorageService = blobStorageService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        [HttpGet]
        public ActionResult Index(PagerParameters pagerParameters) {
            return View(_mediaServicesService.GetAllItems(new MediaItemListViewModel {Find = ""}, pagerParameters));
        }

        [HttpPost]
        public ActionResult Index(MediaItemListViewModel model, PagerParameters pagerParameters){
            return View(_mediaServicesService.GetAllItems(model, pagerParameters));
        }

        [HttpGet]
        public ActionResult Create() {            
            var model = Services.ContentManager.BuildEditor(Services.ContentManager.New<MediaItemPart>(Constants.MediaItem));
            return View(model);
        }

        [HttpPost, ActionName("Create"), ValidateInput(false), FormValueRequired("submit.Save")]
        public ActionResult CreatePost() {
            return CreateMedia();
        }

        [HttpPost, ActionName("Create"), ValidateInput(false), FormValueRequired("submit.Publish")]
        public ActionResult CreatePublishPost() {
            return CreateMedia(true);
        }

        private ActionResult CreateMedia(bool publish = false) {
            
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create media file."))) {
                return new HttpUnauthorizedResult();
            }

            var mediaItemPart = Services.ContentManager.New<MediaItemPart>(Constants.MediaItem);
            if (mediaItemPart == null) {
                return HttpNotFound();
            }
            Services.ContentManager.Create(mediaItemPart, VersionOptions.Draft);
            var model = Services.ContentManager.UpdateEditor(mediaItemPart, this);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }

            Services.Notifier.Information(T("Media has been created."));
            if (publish){
                Services.ContentManager.Publish(mediaItemPart.ContentItem);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id) {
            var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            return mediaItemPart == null ? HttpNotFound() : View(Services.ContentManager.BuildEditor(mediaItemPart));
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPost(int id) {
            return EditMedia(id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditPublishPost(int id) {
            return EditMedia(id, true);
        }

        private ActionResult EditMedia(int id, bool publish = false) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create media file."))) {
                return new HttpUnauthorizedResult();
            }
            var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItemPart == null) {
                return HttpNotFound();
            }
            
            var model = Services.ContentManager.UpdateEditor(mediaItemPart, this);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }
            if (publish) {
                Services.ContentManager.Publish(mediaItemPart.ContentItem);
            }
            Services.Notifier.Information(T("Media has been saved."));
            return RedirectToAction("Index");
        }       

        [HttpGet]
        public ActionResult DeleteEncoded(int id, int encodedId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete encoded.")))
            {
                return new HttpUnauthorizedResult();
            }

            _mediaServicesService.DeleteEncoded(encodedId);
            return RedirectToAction("EncodedList", new {id});
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete media file.")))
            {
                return new HttpUnauthorizedResult();
            }
            var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItemPart == null)
            {
                return HttpNotFound();
            }

            ViewBag.MediaTitle = mediaItemPart.LongTitle;
            ViewBag.ArticlesTitle = _mediaServicesService.GetRelatedContentTitle(id);
            return View();
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.Yes")]
        public ActionResult DeletePostYes(int id) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete media file."))) {
                return new HttpUnauthorizedResult();
            }
            var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItemPart == null) {
                return HttpNotFound();
            }
            //this change status
            _mediaServicesService.RemoveMediaItem(mediaItemPart);                   
            Services.Notifier.Information(T("Media has been removed."));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.No")]
        public ActionResult DeletePostNo(int id) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete media file."))) {
                return new HttpUnauthorizedResult();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult AddNewEncoding(int id) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create new encoding.")))
            {
                return new HttpUnauthorizedResult();
            }
            var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItemPart == null) {
                return HttpNotFound();
            }
            var model = _mediaServicesService.InitEncodingViewModel(id);
            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewEncoding(int id, EncodingViewModel model) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create new encoding."))) {
                return new HttpUnauthorizedResult();
            }

            if (model.SelectedFormats != null) {
                var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
                if (mediaItemPart == null) {
                    return HttpNotFound();
                }
                foreach (var format in model.SelectedFormats) {
                    _mediaServicesService.CreateEncodingJob(mediaItemPart.AssetId, format, Services.WorkContext.CurrentUser.UserName);
                }
                Services.Notifier.Information(T("New encoding has been added."));
                return RedirectToAction("EncodedList", "Admin", new {id});
            }
            else {
                if (_mediaServicesService.GetCurrentEncodingCount(id) > 0) {
                    return RedirectToAction("EncodedList", "Admin", new { id });
                }
                else {
                    model = _mediaServicesService.InitEncodingViewModel(id);
                    Services.Notifier.Information(T("Select at least one item."));
                    return View(model);   
                }
            }
        }

        [HttpGet]
        public ActionResult EncodedList(int id, PagerParameters pagerParameters) {
                       
            var mediaItemPart = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItemPart == null) {
                return HttpNotFound();
            }
            var model = _mediaServicesService.GetEncodedList(id, pagerParameters); 
            ViewBag.Filename = mediaItemPart.As<MediaItemPart>().Filename;
            ViewBag.Title = mediaItemPart.As<TitlePart>().Title;            
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Upload(int? chunk, int? chunks, string name, string token) {
            var fileUpload = Request.Files[0];
            chunk = chunk ?? 0;
            chunks = chunks ?? 1;
            _mediaServicesService.UploadChunk(name, chunk.Value, chunks.Value, fileUpload.InputStream, token);
            return Content("chunk uploaded", "text/plain");
        }

        [HttpGet, OutputCache(Duration = 0)]
        public ActionResult PlayVideo(int id) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't play media.")))
            {
                return new HttpUnauthorizedResult();
            }
            var mediaItem = Services.ContentManager.Get<MediaItemPart>(id, VersionOptions.Latest);
            if (mediaItem == null) {
                return new HttpNotFoundResult();
            }
            var model = new VideoPlayerViewModel {
                Width = 600,
                Height = 420,
                Src = mediaItem.Url,
                Title = mediaItem.Title,
                Type = mediaItem.MimeType                
            };
            
            if (Request.IsAjaxRequest()) {
                return PartialView("_VideoPlayer", model);
            }
            return View(model);
        }

        [HttpGet, OutputCache(Duration = 0)]
        public ActionResult PlayEncoded(int id){
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't play media.")))
            {
                return new HttpUnauthorizedResult();
            }
            var encodedVideo = _mediaServicesService.GetEncodedItem(id);
            if (encodedVideo == null)
            {
                return new HttpNotFoundResult();
            }
            
            if (Request.IsAjaxRequest())
            {
                return PartialView("_VideoPlayer", encodedVideo);
            }
            return View("PlayVideo", encodedVideo);
        }

        public ActionResult ImageList(int id) {

            var model = _mediaServicesService.ImageList(id);            
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ImageList", model);
            }
            return View(model);
        }

        [HttpGet, OutputCache(Duration = 3600 * 24)]
        public ActionResult Crossdomain() {
            var path = Server.MapPath("~/Modules/DigitalPublishingPlatform/Content/OsmfPlugin/crossdomain.xml");
            var lines = System.IO.File.ReadAllLines(path);
            var xmlBuilder = new StringBuilder();
            foreach (var line in lines) {
                xmlBuilder.Append(line);
            }            
            return Content(xmlBuilder.ToString(), "text/xml");
        }

        public ActionResult Publish(int id, string returnUrl) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't publish this content")))
            {
                return new HttpUnauthorizedResult();
            }
            var content = Services.ContentManager.Get(id, VersionOptions.Latest);
            if (content == null)
            {
                return HttpNotFound();
            }
            Services.ContentManager.Publish(content);            
            Services.Notifier.Information(T("Content has been published."));
            return !returnUrl.IsNullOrEmpty() ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index");
        }

        public ActionResult Unpublish(int id, string returnUrl){
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't unpublish this content.")))
            {
                return new HttpUnauthorizedResult();
            }
            var content = Services.ContentManager.Get(id, VersionOptions.Latest);
            if (content == null)
            {
                return HttpNotFound();
            }
            Services.ContentManager.Unpublish(content);
            Services.Notifier.Information(T("Content has been unpublished."));
            return !returnUrl.IsNullOrEmpty() ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index");
        }

        public ActionResult SetDefaultThumbnail(int id, string thumbnailUrl) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't unpublish media file.")))
            {
                return new HttpUnauthorizedResult();
            }
            var encoded = _mediaServicesService.GetEncodedItem(id);
            if (encoded == null)
            {
                return HttpNotFound();
            }
            _mediaServicesService.SetDefaultThumbnail(id, thumbnailUrl);
            return Request.IsAjaxRequest() ? (ActionResult) Json(true, JsonRequestBehavior.AllowGet) : RedirectToAction("ImageList", new {id = id});
        }

        public ActionResult Help() {
            return View();
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}
