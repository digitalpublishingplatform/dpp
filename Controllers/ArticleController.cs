using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
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
using Orchard.UI.Notify;
using Orchard.ContentManagement;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Controllers
{
    [Admin]
    public class ArticleController : Controller, IUpdateModel
    {
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        private readonly IArticleService _articleService;

        public ArticleController(IOrchardServices orchardServices,
            IArticleService articleService)        
        {
            _articleService = articleService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        [HttpGet]
        public ActionResult Index(int publicationId, int issueId, PagerParameters pagerParameters)
        {
            return View(_articleService.GetAllItems(new ArticleItemListViewModel { Find = "" }, issueId, pagerParameters));
        }

        [HttpPost]
        public ActionResult Index(ArticleItemListViewModel model, int publicationId, int issueId, PagerParameters pagerParameters)
        {
            return View(_articleService.GetAllItems(model, issueId, pagerParameters));
        }

        [HttpGet]
        public ActionResult Create(int publicationId, int issueId)
        {
            var model = Services.ContentManager.BuildEditor(Services.ContentManager.New(Constants.Article));
            var mainArticle = _articleService.GetMainArticle(issueId);
            ViewBag.MainArticleTitle = (mainArticle != null) ? mainArticle.Title : "";
            ViewBag.MainArticleId = (mainArticle != null) ? mainArticle.Id : 0;
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePost(int publicationId, int issueId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't create article")))
                return new HttpUnauthorizedResult();

            return CreateArticle(publicationId, issueId);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPost(int publicationId, int issueId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't publish article")))
                return new HttpUnauthorizedResult();

            return CreateArticle(publicationId, issueId, true);
        }

        private ActionResult CreateArticle(int publicationItemId, int issueItemId, bool publish = false)
        {
            var issue = Services.ContentManager.Get<IssuePart>(issueItemId, VersionOptions.Latest);
            if (issue == null)
                return HttpNotFound();

            var articlePart = Services.ContentManager.New<ArticlePart>(Constants.Article);
            articlePart.IssuePart = issue;
            Services.ContentManager.Create(articlePart, VersionOptions.Draft);

            var model = Services.ContentManager.UpdateEditor(articlePart, this);

            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(model as object);
            }

            if (publish)
            {
                Services.ContentManager.Publish(articlePart.ContentItem);
            }

            Services.Notifier.Information(T("Article was created."));
            return RedirectToAction("Index", "Article", new { publicationId = publicationItemId, issueId = issueItemId });
        }

        private object GetArticle(int id, int issueId)
        {
            var articleItemContent = Services.ContentManager.Get(id, VersionOptions.Latest);
            var editor = Services.ContentManager.BuildEditor(articleItemContent);

            var mainArticle = _articleService.GetMainArticle(issueId);
            ViewBag.MainArticleTitle = (mainArticle != null && mainArticle.Id != id) ? mainArticle.Title : "";
            ViewBag.MainArticleId = (mainArticle != null && mainArticle.Id != id) ? mainArticle.Id : 0;

            return editor;
        }

        [HttpGet]
        public ActionResult Edit(int id, int issueId, int publicationId)
        {
            var editor = GetArticle(id, issueId);
            return View(editor);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPost(int id, int issueId, int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit article.")))
            {
                return new HttpUnauthorizedResult();
            }

            return EditArticle(id, issueId, publicationId);
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditPublishPost(int id, int issueId, int publicationId)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't edit article.")))
            {
                return new HttpUnauthorizedResult();
            }

            return EditArticle(id, issueId, publicationId, true);
        }

        private ActionResult EditArticle(int id, int issueItemId, int publicationItemId, bool publish = false) {
            var articleContentItem = Services.ContentManager.Get(id, VersionOptions.Latest);

            var model = Services.ContentManager.UpdateEditor(articleContentItem, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                model = GetArticle(id, issueItemId);
                return View(model);
            }

            if (publish)
            {
                Services.ContentManager.Publish(articleContentItem);
            }

            Services.Notifier.Information(T("Article was saved."));
            return RedirectToAction("Index", "Article", new { publicationId = publicationItemId, issueId = issueItemId });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete article.")))
            {
                return new HttpUnauthorizedResult();
            }
            var articlePart = Services.ContentManager.Get<ArticlePart>(id, VersionOptions.Latest);
            if (articlePart == null)
            {
                return HttpNotFound();
            }

            ViewBag.ArticleTitle = articlePart.Title;

            return View();
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.Yes")]
        public ActionResult DeletePostYes(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete article.")))
            {
                return new HttpUnauthorizedResult();
            }
            var articlePart = Services.ContentManager.Get<ArticlePart>(id, VersionOptions.Latest);
            if (articlePart == null)
            {
                return HttpNotFound();
            }
            //this change status
            Services.ContentManager.Remove(articlePart.ContentItem);
            Services.Notifier.Information(T("Article has been removed."));
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete"), FormValueRequired("submit.No")]
        public ActionResult DeletePostNo(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.PublicationFramework, T("Couldn't delete article.")))
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
