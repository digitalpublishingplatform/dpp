using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels.Category;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents.Controllers;
using Orchard.Localization;
using Orchard.UI.Navigation;
using DigitalPublishingPlatform.Models;
using Orchard.UI.Notify;
using VersionOptions = Orchard.ContentManagement.VersionOptions;
using Orchard.UI.Admin;

namespace DigitalPublishingPlatform.Controllers
{
    [Admin]
    public class CategoryController : Controller, IUpdateModel
    {
        private readonly ICategoryService _categoryService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }        

        public CategoryController(IOrchardServices orchardServices, 
            ICategoryService categoryService)        
        {
            _categoryService = categoryService;
            Services = orchardServices;
            T = NullLocalizer.Instance;            
        }

        public ActionResult Index(CategoryItemListViewModel model, int publicationId)
        {
            return View(_categoryService.GetAll(model, publicationId));            
        }
        
        [HttpGet]
        public ActionResult Create(int publicationId) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create a category")))
                return new HttpUnauthorizedResult();
            var model = Services.ContentManager.BuildEditor(Services.ContentManager.New(Constants.Category));
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePost(int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create a category")))
                return new HttpUnauthorizedResult();
            return CreateCategory(publicationId);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPost(int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create a category")))
                return new HttpUnauthorizedResult();
            return CreateCategory(publicationId, true);
        }

        private ActionResult CreateCategory(int publicationId, bool publish = false)
        {            
            var categoryPart = Services.ContentManager.New<CategoryPart>(Constants.Category);
            categoryPart.PublicationId = publicationId;
            Services.ContentManager.Create(categoryPart, VersionOptions.Draft);
            var model = Services.ContentManager.UpdateEditor(categoryPart, this);

            // category name is required
            if (!categoryPart.Name.IsNullOrEmpty())
            {
                if (!ModelState.IsValid)
                {
                    Services.TransactionManager.Cancel();
                    return View(model as object);
                }

                if (publish)
                {
                    Services.ContentManager.Publish(categoryPart.ContentItem);
                }

                Services.Notifier.Information(T("Category was created."));
                return RedirectToAction("Index", "Category");   
            }
            else {
                Services.TransactionManager.Cancel();
                Services.Notifier.Information(T("Category name is required."));
                model = Services.ContentManager.BuildEditor(Services.ContentManager.New(Constants.Category));
                return View(model);
            }
        }
    
        [HttpGet]
        public ActionResult Edit(int id) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit a category.")))
                return new HttpUnauthorizedResult();
            var categoryPart = Services.ContentManager.Get<CategoryPart>(id, VersionOptions.Latest);
            if (categoryPart == null)
            {
                return HttpNotFound();
            }
            return View(Services.ContentManager.BuildEditor(categoryPart));
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPost(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit a category.")))
            {
                return new HttpUnauthorizedResult();
            }
            return EditCategory(id);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditPublishPost(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit a category.")))
            {
                return new HttpUnauthorizedResult();
            }
            return EditCategory(id, true);
        }

        private ActionResult EditCategory(int id, bool publish = false)
        {
            var categoryContent = Services.ContentManager.Get(id, VersionOptions.Latest);
            var model = Services.ContentManager.UpdateEditor(categoryContent, this);

            // category name is required
            if (!categoryContent.As<CategoryPart>().Name.IsNullOrEmpty()) {
                if (!ModelState.IsValid) {
                    Services.TransactionManager.Cancel();
                    return View(model as object);
                }

                if (publish) {
                    Services.ContentManager.Publish(categoryContent);
                }

                Services.Notifier.Information(T("Category has been saved."));
                return RedirectToAction("Index", "Category");
            }
            else {
                Services.TransactionManager.Cancel();
                var categoryPart = Services.ContentManager.Get<CategoryPart>(id, VersionOptions.Latest);
                Services.Notifier.Information(T("Category name is required."));
                return View(Services.ContentManager.BuildEditor(categoryPart));
            }
        }

        public ActionResult ChangePosition(int id, Direction direction, string returnUrl) {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't change category position.")))
            {
                return new HttpUnauthorizedResult();
            }
            var categoryPart = Services.ContentManager.Get<CategoryPart>(id, VersionOptions.Latest);
            if (categoryPart == null)
            {
                return HttpNotFound();
            }
            _categoryService.ChangePosition(id, direction);
            return !returnUrl.IsNullOrEmpty() ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete category.")))
            {
                return new HttpUnauthorizedResult();
            }
            var categoryPart = Services.ContentManager.Get<CategoryPart>(id, VersionOptions.Latest);
            if (categoryPart == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryName = categoryPart.Name;

            return View();
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.Yes")]
        public ActionResult DeletePostYes(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete category.")))
            {
                return new HttpUnauthorizedResult();
            }
            var categoryPart = Services.ContentManager.Get<CategoryPart>(id, VersionOptions.Latest);
            if (categoryPart == null)
            {
                return HttpNotFound();
            }
            //this change status
            Services.ContentManager.Remove(categoryPart.ContentItem);
            Services.Notifier.Information(T("Category has been removed."));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.No")]
        public ActionResult DeletePostNo(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete category.")))
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

        [HttpPost]
        public ActionResult UpdateCategoryOrder(int[] ids)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't update category order.")))
            {
                return new HttpUnauthorizedResult();
            }
            _categoryService.ChangePosition(ids);

            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction("Index"); 
            }
            else {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
