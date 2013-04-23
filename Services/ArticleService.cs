using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IShapeFactory _shapeFactory;
        private readonly ISiteService _siteService;

        public ArticleService(IOrchardServices orchardServices, IContentManager contentManager, IShapeFactory shapeFactory, ISiteService siteService)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
            _siteService = siteService;
        }

        public ArticleItemListViewModel GetAllItems(ArticleItemListViewModel model, int issueId, PagerParameters pagerParameters) {
            var find = model.Find ?? "";

            var articleItemQuery = _orchardServices.ContentManager
                                                             .Query()
                                                             .ForVersion(VersionOptions.Latest)
                                                             .Join<CommonPartRecord>()
                                                             .Where(x => x.Container != null && x.Container.Id == issueId)
                                                             .ForPart<ArticlePart>().List()
                                                             .Where(x => x.Title.ToLowerInvariant().Contains(find));

            var count = articleItemQuery.Count();

            // when is selected the option 'Show All' in pagination,the page size parameter is equal to 0
            if (pagerParameters.PageSize == 0)
            {
                // assign at page size parameter value the total number of records found
                pagerParameters.PageSize = count;
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            dynamic shape = _shapeFactory;
            
            model = new ArticleItemListViewModel
            {
                IssueId = issueId,
                Pager = shape.Pager(pager).TotalItemCount(count),
                ArticleItems = articleItemQuery.Select(x => new ArticleItemViewModel
                {
                    Title = x.As<TitlePart>().Title,
                    Content = x.As<BodyPart>().Text,
                    Published = x.IsPublished(),
                    CreatedUtc = x.As<CommonPart>().CreatedUtc,
                    ModifiedUtc = x.As<CommonPart>().ModifiedUtc,
                    Author = x.Author,
                    Id = x.Id
                }).Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize == 0 ? count : pager.PageSize)
            };

            return model;
        }

        public IEnumerable<ArticleItemViewModel> GetAllItems(int issueId)
        {
            var articleItemQuery = _orchardServices.ContentManager
                                                             .Query()
                                                             .ForVersion(VersionOptions.Latest)
                                                             .Join<CommonPartRecord>()
                                                             .Where(x => x.Container != null && x.Container.Id == issueId)
                                                             .ForPart<ArticlePart>().List()
                                                             .OrderBy(a => a.Title);

            var result = articleItemQuery.Select(x => new ArticleItemViewModel
                {
                    Title = x.As<TitlePart>().Title,
                    Content = x.As<BodyPart>().Text,
                    Published = x.IsPublished(),
                    CreatedUtc = x.As<CommonPart>().CreatedUtc,
                    ModifiedUtc = x.As<CommonPart>().ModifiedUtc,
                    Author = x.Author,
                    Id = x.Id
                });

            return result;
        }

        public ArticleItemViewModel GetMainArticle(int issueId)
        {
            var article = _orchardServices.ContentManager
                                            .Query()
                                            .ForVersion(VersionOptions.Latest)
                                            .Join<CommonPartRecord>()
                                            .Where(x => x.Container != null && x.Container.Id == issueId)
                                            .ForPart<ArticlePart>().List().FirstOrDefault(x => x.MainArticle);

            if (article == null)
            {
                return null;
            }

            var result = new ArticleItemViewModel
            {
                Title = article.Title,
                Id = article.Id
            };

            return result;
        }

        public int? GetMainArticleId(int issueId)
        {
            var articlesList = _orchardServices.ContentManager
                                            .Query()
                                            .ForVersion(VersionOptions.Latest)
                                            .Join<CommonPartRecord>()
                                            .Where(x => x.Container != null && x.Container.Id == issueId)
                                            .ForPart<ArticlePart>().List().Where(x => x.MainArticle);

            var mainArticle = articlesList.FirstOrDefault();

            if (mainArticle == null)
            {
                return null;
            }

            return mainArticle.Id;
        }
    }
}