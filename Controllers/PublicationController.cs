using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Settings;
using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.ContentManagement;
using Orchard.UI.Admin;

namespace DigitalPublishingPlatform.Controllers
{
    [Admin]
    public class PublicationController : Controller, IUpdateModel
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private readonly IPublicationService _publicationService;

        public PublicationController(IOrchardServices orchardServices, 
            IPublicationService publicationService)        
        {
            _publicationService = publicationService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        [HttpGet]
        public ActionResult Index(PagerParameters pagerParameters)
        {
            return View(_publicationService.GetAllItems(new PublicationItemListViewModel{Find = ""}, pagerParameters));
        }

        [HttpPost]
        public ActionResult Index(PublicationItemListViewModel model, PagerParameters pagerParameters)
        {
            return View(_publicationService.GetAllItems(model, pagerParameters));
        }

        [HttpGet]
        public ActionResult Create() {
            ViewBag.UploadMediaFolder = "Publication";
            var model = Services.ContentManager.BuildEditor(Services.ContentManager.New<PublicationPart>(Constants.Publication));
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePost()
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create publication.")))
                return new HttpUnauthorizedResult();

            return CreatePublication(false);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPost()
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't publish publication.")))
                return new HttpUnauthorizedResult();

            return CreatePublication(true);
        }

        private ActionResult CreatePublication(bool publish = false)
        {
            var publicationPart = Services.ContentManager.New<PublicationPart>(Constants.Publication);
            Services.ContentManager.Create(publicationPart, VersionOptions.Draft);

            var model = Services.ContentManager.UpdateEditor(publicationPart, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }

            if (publish)
            {
                Services.ContentManager.Publish(publicationPart.ContentItem);
            }

            Services.Notifier.Information(T("Publication was created."));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var publicationItemContent = Services.ContentManager.Get(id, VersionOptions.Latest);
            var editor = Services.ContentManager.BuildEditor(publicationItemContent);
            return View(editor);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPost(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit publication.")))
            {
                return new HttpUnauthorizedResult();
            }
            return EditPublication(id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditPublishPost(int id, string token)
        {
            return EditPublication(id, true);
        }

        private ActionResult EditPublication(int id, bool publish = false) {
            var publicationPart = Services.ContentManager.Get<PublicationPart>(id, VersionOptions.Latest);
            var model = Services.ContentManager.UpdateEditor(publicationPart, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }

            if (publish)
            {
                Services.ContentManager.Publish(publicationPart.ContentItem);
            }

            Services.Notifier.Information(T("Publication was saved."));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete publication.")))
            {
                return new HttpUnauthorizedResult();
            }
            var publicationPart = Services.ContentManager.Get<PublicationPart>(id, VersionOptions.Latest);
            if(publicationPart == null) {
                return HttpNotFound();
            }

            ViewBag.PublicationTitle = publicationPart.Title;

            return View();
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.Yes")]
        public ActionResult DeletePostYes(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete publication.")))
            {
                return new HttpUnauthorizedResult();
            }
            var publicationPart = Services.ContentManager.Get<PublicationPart>(id, VersionOptions.Latest);
            if (publicationPart == null)
            {
                return HttpNotFound();
            }
            //this change status
            Services.ContentManager.Remove(publicationPart.ContentItem);
            Services.Notifier.Information(T("Publication has been removed."));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.No")]
        public ActionResult DeletePostNo(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete publication.")))
            {
                return new HttpUnauthorizedResult();
            }
            return RedirectToAction("Index");
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
