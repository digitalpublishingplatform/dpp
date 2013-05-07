using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using NHibernate.Linq;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.Data;

namespace DigitalPublishingPlatform.Services
{
    public class IssueService : IIssueService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IShapeFactory _shapeFactory;
        private readonly ISiteService _siteService;
        private readonly IRepository<ArticleCategoryRecord> _articleCategoryRepository;
        private readonly IVideoService _videoService;
        private readonly IImageService _imageService;

        public IssueService(IOrchardServices orchardServices, 
            IShapeFactory shapeFactory, 
            ISiteService siteService,
            IRepository<ArticleCategoryRecord> articleCategoryRepository,
            IVideoService videoService, IImageService imageService)
        {
            _orchardServices = orchardServices;
            _shapeFactory = shapeFactory;
            _siteService = siteService;
            _articleCategoryRepository = articleCategoryRepository;
            _videoService = videoService;
            _imageService = imageService;
        }

        public IssueItemListViewModel GetAllItems(IssueItemListViewModel model, int publicationId, PagerParameters pagerParameters) {
            var find = model.Find ?? "";
            var issueItemQuery = _orchardServices.ContentManager
                                                .Query()
                                                .ForVersion(VersionOptions.Latest)
                                                .Join<CommonPartRecord>()
                                                .Where(x => x.Container != null && x.Container.Id == publicationId)
                                                .ForPart<IssuePart>().List().Where(x => find == "" || x.Title.ToLowerInvariant().Contains(find));

            var count = issueItemQuery.Count();
            // when is selected the option 'Show All' in pagination,the page size parameter is equal to 0
            if (pagerParameters.PageSize == 0)
            {
                // assign at page size parameter value the total number of records found
                pagerParameters.PageSize = count;
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            dynamic shape = _shapeFactory;
            
            model = new IssueItemListViewModel
            {
                PublicationId = publicationId,
                Pager = shape.Pager(pager).TotalItemCount(count),
                IssueItems = issueItemQuery.Select(x => new IssueItemViewModel
                {
                    Title = x.As<TitlePart>().Title,
                    Description = x.As<BodyPart>().Text,
                    Published = x.IsPublished(),
                    CreatedUtc = x.As<CommonPart>().CreatedUtc,
                    ModifiedUtc = x.ModifiedUtc,
                    Id = x.Id
                }).Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize == 0 ? count : pager.PageSize)
            };

            return model;
        }


        public IEnumerable<string> TagsByIssueId(int id)
        {            
            var articleList = _orchardServices.ContentManager
                           .Query<ArticlePart>()
                           .List()
                           .Where(a => a.IssuePart != null
                                       && a.IssuePart.Id == id)
                           .Select(a => a.Id);

            return _articleCategoryRepository.Table
                                      .Where(ac => articleList.Contains(ac.ArticleRecord.Id))
                                      .OrderBy(ac => ac.CategoryRecord.Position)
                                      .Select(ac => ac.CategoryRecord.Name);
        }

        public MainIssueFrontViewModel IssueFront(int id)
        {
            var articleList = _orchardServices.ContentManager
                                              .Query<ArticlePart>()
                                              .ForVersion(VersionOptions.Published)
                                              .List()
                                              .Where(a => a.IssuePart != null
                                                          && a.IssuePart.Id == id)
                                              .Select(a => a);

            var articleIds = articleList.Select(a => a.Id).ToList();
            
            var articleCategories = _articleCategoryRepository.Table
                                                       .Where(ac => articleIds.Contains(ac.ArticleRecord.Id))
                                                       .OrderBy(ac => ac.CategoryRecord.Position)
                                                       .Select(ac => ac);

            var categoryNames = articleCategories.Select(ac => ac.CategoryRecord.Name).ToList().Distinct();

            var result = new List<CategoryArticlesViewModel>();
            foreach (var categoryName in categoryNames)
            {
                var tagArticle = new CategoryArticlesViewModel
                {
                    Category = categoryName
                };
                var newArticleList = new List<ArticleViewModel>();
                var articleCategoriesByCategoryName = articleCategories.Where(ac => ac.CategoryRecord.Name == categoryName).OrderBy(ac => ac.CategoryDisplayOrder);
                foreach (var articleCategoryRecord in articleCategoriesByCategoryName)
                {
                    var article = articleList.FirstOrDefault(al => al.Id == articleCategoryRecord.ArticleRecord.Id);
                    if (article != null)
                    {
                        newArticleList.Add(new ArticleViewModel
                        {
                            Id = article.Id,
                            Title = article.Title,
                            Text = article.Text,
                            CreatedUtc = article.CreatedUtc,
                            ModifiedUtc = article.ModifiedUtc,
                            PublishedUtc = article.PublishedUtc,
                            ImageUrl = article.Url,
                            Author = article.Author,
                            Videos = _videoService.GetVideoList(article.Id),
                            Categories = article.Categories,
                            Images = _imageService.GetImageUrlListByArticle(article.Id)
                        });
                    }
                }

                tagArticle.Articles = newArticleList;

                result.Add(tagArticle);
            }
            var mainArticle = articleList.FirstOrDefault(al => al.MainArticle);

            return new MainIssueFrontViewModel {
                MainArticle = (mainArticle != null) ? new ArticleViewModel {
                    Id = mainArticle.Id,
                    Title = mainArticle.Title,
                    Text = mainArticle.Text,
                    CreatedUtc = mainArticle.CreatedUtc,
                    ModifiedUtc = mainArticle.ModifiedUtc,
                    PublishedUtc = mainArticle.PublishedUtc,
                    ImageUrl = mainArticle.Url,
                    Author = mainArticle.Author,
                    Videos = _videoService.GetVideoList(mainArticle.Id),
                    Categories = mainArticle.Categories,
                    Images = _imageService.GetImageUrlListByArticle(mainArticle.Id)
                } : null,
                CategoriesAndArticles = result
            };
        }
    }
}