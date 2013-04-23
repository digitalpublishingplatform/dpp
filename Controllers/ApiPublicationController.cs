using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.ViewModels.Api;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DigitalPublishingPlatform.Controllers
{
    /// <summary>
    /// Simple api controller for the "publication framework" frontend
    /// </summary>
    public class ApiPublicationController : ApiController
    {
        private readonly IVideoService _videoService;
        private readonly IIssueService _issueService;
        private readonly ICategoryService _categoryService;
        private readonly IImageService _imageService;
        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }

        public ApiPublicationController(IOrchardServices orchardServices, IVideoService videoService, IIssueService issueService, ICategoryService categoryService, IImageService imageService)
        {
            _videoService = videoService;
            _issueService = issueService;
            _categoryService = categoryService;
            _imageService = imageService;
            Services = orchardServices;
            T = NullLocalizer.Instance;            
        }
        
        /// <summary>
        /// Gets all published publications 
        /// </summary>
        /// <returns>publications</returns>
        [HttpGet, HttpPost]
        public IEnumerable<PublicationViewModel> All() {
            return Services.ContentManager.HqlQuery()
                    .ForPart<PublicationPart>()
                    .ForVersion(VersionOptions.Published)
                    .List()
                    .OrderByDescending(pp => pp.CreatedUtc)
                    .Select(pp => new PublicationViewModel {
                        Id = pp.Id,
                        CreatedUtc = pp.CreatedUtc,
                        ModifiedUtc = pp.ModifiedUtc,
                        Text = pp.Text,
                        Title = pp.Title,
                        ImageUrl = pp.Url
                    });
        }

        /// <summary>
        /// Gets all published publications with corresponding published issues
        /// </summary>
        /// <returns>publications with issues</returns>
        [HttpGet, HttpPost]
        public IEnumerable<PublicationViewModel> AllWithIssues() {
            var issues = Services.ContentManager.HqlQuery()
                                 .ForPart<IssuePart>()
                                 .ForVersion(VersionOptions.Published)
                                 .List()
                                 .OrderByDescending(pp => pp.PublishedUtc);
            
            var publicationList = All().ToList();
            foreach (var issue in issues) {
                var publicationViewModel = publicationList.FirstOrDefault(p => p.Id == issue.PublicationPart.Id);
                if (publicationViewModel == null) {
                    publicationViewModel = new PublicationViewModel {
                        Id = issue.PublicationPart.Id,
                        CreatedUtc =  issue.PublicationPart.CreatedUtc,
                        ModifiedUtc = issue.PublicationPart.ModifiedUtc,
                        ImageUrl = issue.PublicationPart.Url,
                        Text = issue.PublicationPart.Text,
                        Title = issue.PublicationPart.Title                       
                    };
                    publicationList.Add(publicationViewModel);
                }

                publicationViewModel.Issues.Add(new IssueViewModel {
                    ModifiedUtc = issue.ModifiedUtc,
                    CreatedUtc = issue.CreatedUtc,
                    Id = issue.Id,
                    Text = issue.Text,
                    Title = issue.Title,
                    ImageUrl = issue.Url
                });
            }
            return publicationList;
        }

        /// <summary>
        /// Gets all articles for issue
        /// </summary>
        /// <param name="id">issue id</param>
        /// <returns>articles</returns>
        [HttpGet, HttpPost]
        public IEnumerable<ArticleViewModel>  Articles(int id) {
            return Services.ContentManager.HqlQuery()
                .ForPart<ArticlePart>()
                .ForVersion(VersionOptions.Published)
                .List()
                .Where(a => a.IssuePart != null && a.IssuePart.Id == id)
                .OrderByDescending(pp => pp.PublishedUtc)
                .Select(a => new ArticleViewModel {
                    Id = a.Id,
                    Title = a.Title,
                    Text = a.Text,
                    CreatedUtc = a.CreatedUtc,
                    ModifiedUtc = a.ModifiedUtc,
                    PublishedUtc = a.PublishedUtc,
                    ImageUrl = a.Url,
                    Author = a.Author,
                    Videos = _videoService.GetVideoList(a.Id),
                    Tags = a.Categories,
                    Images = _imageService.GetImageUrlListByArticle(a.Id),
                });            
        }

        /// <summary>
        /// Get all published categories for an issue
        /// </summary>
        /// <param name="id">issue id</param>
        /// <returns>list of category names</returns>
        [HttpGet, HttpPost]
        public IEnumerable<string> CategoriesByIssueId(int id) {
            return _issueService.TagsByIssueId(id);            
        }

        /// <summary>
        /// Gets all published articles by category name and issue id
        /// </summary>
        /// <param name="id">issue id</param>
        /// <param name="categoryName">category name</param>
        /// <returns>articles</returns>
        [HttpGet, HttpPost]
        public IEnumerable<ArticleViewModel> ArticlesByCategoryNameAndIssueId(int id, string categoryName)
        {
            
            return Services.ContentManager
                            .Query<ArticlePart>()
                            .List()
                            .Where(a => a.IssuePart != null
                                && a.IssuePart.Id == id
                                && a.Categories.Contains(categoryName))
                            .Select(a => new ArticleViewModel
                            {
                                Id = a.Id,
                                Title = a.Title,
                                Text = a.Text,
                                CreatedUtc = a.CreatedUtc,
                                ModifiedUtc = a.ModifiedUtc,
                                PublishedUtc = a.PublishedUtc,
                                ImageUrl = a.Url,
                                Author = a.Author,
                                Videos = _videoService.GetVideoList(a.Id),
                                Images = _imageService.GetImageUrlListByArticle(a.Id),
                                Tags = a.Categories,
                                Position = _categoryService.GetPositionByCategoryArticle(categoryName, a.Id)
                            })
                            .OrderBy(a => a.Position);
        }


        /// <summary>
        /// Gets published categories and articles for an issue 
        /// </summary>
        /// <param name="id">issue id</param>
        /// <returns>list of cat</returns>
        [HttpGet, HttpPost]
        public IEnumerable<TagArticlesViewModel> CategoryAndArticlesByIssueId(int id) {
            return _issueService.IssueFront(id).TagsAndArticles;            
        }

        /// <summary>
        /// Gets all articles and categories specified in "Issue front" for an issue
        /// </summary>
        /// <param name="id">Issue id</param>
        /// <returns>the main article and a list of categories with articles</returns>
        [HttpGet, HttpPost]
        public MainIssueFrontViewModel IssueFront(int id)
        {
            return _issueService.IssueFront(id);
        }
    }
}
