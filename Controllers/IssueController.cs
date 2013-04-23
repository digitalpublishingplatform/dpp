using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.ViewModels.Category;
using Orchard;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.Localization;
using Orchard.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement;
using Orchard.UI.Admin;

namespace DigitalPublishingPlatform.Controllers
{
    [Admin]
    public class IssueController : Controller, IUpdateModel
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private readonly IIssueService _issueService;
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;

        public IssueController(IOrchardServices orchardServices,
            IIssueService issueService, IArticleService articleService, ICategoryService categoryService)        
        {
            _issueService = issueService;
            _articleService = articleService;
            _categoryService = categoryService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        [HttpGet]
        public ActionResult Index(int publicationId, PagerParameters pagerParameters)
        {
            return View(_issueService.GetAllItems(new IssueItemListViewModel { Find = "" }, publicationId, pagerParameters));
        }

        [HttpPost]
        public ActionResult Index(IssueItemListViewModel model, int publicationId, PagerParameters pagerParameters)
        {
            return View(_issueService.GetAllItems(model, publicationId, pagerParameters));
        }

        [HttpGet]
        public ActionResult Create(int publicationId)
        {
            var model = Services.ContentManager.BuildEditor(Services.ContentManager.New<IssuePart>(Constants.Issue));
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePost(int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create issue.")))
                return new HttpUnauthorizedResult();

            return CreateIssue(publicationId, false);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPost(int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't publish issue.")))
                return new HttpUnauthorizedResult();

            return CreateIssue(publicationId, true);
        }

        private ActionResult CreateIssue(int publicationItemId, bool publish = false) {
            var publicationPart = Services.ContentManager.Get<PublicationPart>(publicationItemId, VersionOptions.Latest);
            if (publicationPart == null)
                return HttpNotFound();

            var issuePart = Services.ContentManager.New<IssuePart>(Constants.Issue);
            issuePart.PublicationPart = publicationPart;
            Services.ContentManager.Create(issuePart, VersionOptions.Draft);

            var model = Services.ContentManager.UpdateEditor(issuePart, this);

            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }

            if (publish)
            {
                Services.ContentManager.Publish(issuePart.ContentItem);
            }

            Services.Notifier.Information(T("Issue was created."));
            return RedirectToAction("Index", "Issue", new { publicationId = publicationItemId });
        }

        [HttpGet]
        public ActionResult Edit(int id, int publicationId)
        {
            var issueItemContent = Services.ContentManager.Get(id, VersionOptions.Latest);
            var editor = Services.ContentManager.BuildEditor(issueItemContent);

            return View(editor);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPost(int id, int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit issue.")))
            {
                return new HttpUnauthorizedResult();
            }
            return EditIssue(id, publicationId);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditPublishPost(int id, int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit issue.")))
            {
                return new HttpUnauthorizedResult();
            }
            return EditIssue(id, publicationId, true);
        }

        private ActionResult EditIssue(int id, int publicationItemId, bool publish = false)
        {
            var issuePart = Services.ContentManager.Get<IssuePart>(id, VersionOptions.Latest);

            var model = Services.ContentManager.UpdateEditor(issuePart, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }

            if (publish)
            {
                Services.ContentManager.Publish(issuePart.ContentItem);
            }

            Services.Notifier.Information(T("Issue was saved."));

            return RedirectToAction("Index", "Issue", new { publicationId = publicationItemId });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete issue.")))
            {
                return new HttpUnauthorizedResult();
            }
            var issuePart = Services.ContentManager.Get<IssuePart>(id, VersionOptions.Latest);
            if (issuePart == null)
            {
                return HttpNotFound();
            }

            ViewBag.IssueTitle = issuePart.Title;

            return View();
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.Yes")]
        public ActionResult DeletePostYes(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete issue.")))
            {
                return new HttpUnauthorizedResult();
            }
            var issuePart = Services.ContentManager.Get<IssuePart>(id, VersionOptions.Latest);
            if (issuePart == null)
            {
                return HttpNotFound();
            }
            //this change status
            Services.ContentManager.Remove(issuePart.ContentItem);
            Services.Notifier.Information(T("Issue has been removed."));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.No")]
        public ActionResult DeletePostNo(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete issue.")))
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

        [HttpGet]
        public ActionResult IssueStructure(int publicationId, int id) {
            var model = new CategoriesArticlesViewModel {
                Articles = (new[]{new SelectListItem{Text = "None", Value = ""}}).Union(_articleService.GetAllItems(id).Select(a => new SelectListItem{Text = a.Title, Value = a.Id.ToString()})), 
                Categories = _categoryService.GetAll(publicationId)
            };

            var categoryArticleList = new List<CategoryArticleViewModel>();
            foreach (var category in model.Categories)
            {
                var categoryArticle = _categoryService.GetCategoryArticle(id, category.Id, category.Name);
                categoryArticleList.Add(categoryArticle);
            }
            model.CategoryArticleList = categoryArticleList;
            model.MainArticleId = _articleService.GetMainArticleId(id);
            return View(model);
        }

        [HttpPost, ActionName("IssueStructure"), FormValueRequired("submit.Save")]
        public ActionResult Save(CategoriesArticlesViewModel model, int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't save issue front.")))
            {
                return new HttpUnauthorizedResult();
            }

            var issuePart = Services.ContentManager.Get<IssuePart>(id, VersionOptions.Latest);
            if (issuePart == null)
            {
                return HttpNotFound();
            }

            int? currentMainArticleId = _articleService.GetMainArticleId(id);

            _categoryService.InsertIssueFront(model, currentMainArticleId, id);

            Services.Notifier.Information(T("Issue Front has been saved."));
            return RedirectToAction("Index");
        }
    }
}
