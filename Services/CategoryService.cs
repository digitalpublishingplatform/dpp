using System.Collections.Generic;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.ViewModels.Category;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Navigation;
using System.Linq;

namespace DigitalPublishingPlatform.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ArticleCategoryRecord> _articleCategoryRepository;
        private readonly IRepository<CategoryRecord> _categoryRepository;
        private readonly IRepository<ArticleRecord> _articleRepository;
        private readonly IShapeFactory _shapeFactory;
        private readonly ISiteService _siteService;

        public CategoryService(IOrchardServices orchardServices, IShapeFactory shapeFactory, ISiteService siteService, IRepository<CategoryRecord> categoryRepository, IRepository<ArticleCategoryRecord> articleCategoryRepository, IRepository<ArticleRecord> articleRepository)
        {
            _orchardServices = orchardServices;            
            _shapeFactory = shapeFactory;
            _siteService = siteService;
            _categoryRepository = categoryRepository;
            _articleCategoryRepository = articleCategoryRepository;
            _articleRepository = articleRepository;
        }

        public CategoryItemListViewModel GetAll(CategoryItemListViewModel model, int publicationId) {
            var categoryQuery = _orchardServices.ContentManager
                                                   .Query()
                                                   .ForVersion(VersionOptions.Latest)                                                                                                     
                                                   .ForPart<CategoryPart>().List()
                                                   .OrderBy(c => c.Position).ThenBy(c => c.Name)
                                                   .Where(c => c.PublicationId == publicationId && c.Name != "" && c.Name != null);
            return new CategoryItemListViewModel{                                
                Categories = categoryQuery.Select(x => new CategoryViewModel {
                    Name = x.Name,                    
                    Published = x.IsPublished(),
                    CreatedUtc = x.As<CommonPart>().CreatedUtc,
                    ModifiedUtc = x.As<CommonPart>().ModifiedUtc,
                    Position = x.Position,
                    Id = x.Id
                })};            
        }

        public IEnumerable<CategoryLightViewModel> GetList(int articleId)
        {
            var articleCategoriesIds = _articleCategoryRepository.Table.Where(vm => vm.ArticleRecord.Id == articleId);
            return _orchardServices.ContentManager.HqlQuery()
                .ForPart<CategoryPart>()
                .ForVersion(VersionOptions.Published)                
                .List()
                .OrderBy(c => c.Position)
                .Select(x => new CategoryLightViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Position = x.Position,
                    Checked = (articleCategoriesIds.Any(m => m.CategoryRecord.Id == x.Id))
                });
        }

        public void UpdateCategoryForContentItem(ContentItem item, EditCategoriesViewModel model)
        {
            var currentArticleRecord = item.As<ArticlePart>().Record;
            var oldMedia = _articleCategoryRepository.Fetch(r => r.ArticleRecord == currentArticleRecord);
            var ids = model.Categories.Where(x => x.Checked).Select(x => x.Id).ToList();
            var newCategories = _categoryRepository.Table.Where(mi => ids.Contains(mi.Id));            
            foreach (var articleCategoryRecord in oldMedia)
            {
                _articleCategoryRepository.Delete(articleCategoryRecord);                
            }            
            foreach (var categoryRecord in newCategories) {
                var categoryVideModel =  model.Categories.FirstOrDefault(c => c.Id == categoryRecord.Id);
                _articleCategoryRepository.Create(new ArticleCategoryRecord
                {
                    ArticleRecord = currentArticleRecord,
                    CategoryRecord = categoryRecord,
                    CategoryDisplayOrder = (categoryVideModel != null) ? ((categoryVideModel.CategoryDisplayOrder == 0) ? null : categoryVideModel.CategoryDisplayOrder) : null
                });
            }
        }


        public void ChangePosition(int id, Direction direction) {
            var currentCategory = _categoryRepository.Get(id);
            CategoryRecord selectedCandidateToSwap = null;
            if (direction == Direction.Up) {
                selectedCandidateToSwap = _categoryRepository.Table
                                                                 .Where(c =>
                                                                        c.Position < currentCategory.Position
                                                                        && c.Id != currentCategory.Id)
                                                                 .OrderByDescending(c => c.Position)
                                                                 .FirstOrDefault();
            }
            else {
                selectedCandidateToSwap = _categoryRepository.Table
                                                                 .Where(c =>
                                                                        c.Position > currentCategory.Position
                                                                        && c.Id != currentCategory.Id)
                                                                 .OrderBy(c => c.Position)
                                                                 .FirstOrDefault();
            }

            if (selectedCandidateToSwap == null) {
                return;
            }

            var backupPostition = selectedCandidateToSwap.Position;
            selectedCandidateToSwap.Position = currentCategory.Position;
            currentCategory.Position = backupPostition;
            _categoryRepository.Update(currentCategory);
            _categoryRepository.Update(selectedCandidateToSwap);
        }

        public IEnumerable<CategoryViewModel> GetAll(int publicationId)
        {

            var result = _orchardServices.ContentManager.HqlQuery()
                .ForPart<CategoryPart>()
                .ForVersion(VersionOptions.Published)
                .List()
                .Where(x => x.PublicationId == publicationId )
                .Select(x => new CategoryViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Position = x.Position
                })
                .OrderBy(x => x.Position);

            return result;
        }
        
        public void InsertIssueFront(CategoriesArticlesViewModel model, int? currentMainArticleId, int issueId) {
            if(currentMainArticleId != null) {
                var currentMainArticleRecord = _articleRepository.Get(currentMainArticleId.Value);
                currentMainArticleRecord.MainArticle = false;
                _articleRepository.Update(currentMainArticleRecord);
            }

            if(model.MainArticleId != null) {
                var mainArticleRecord = _articleRepository.Get(model.MainArticleId.Value);
                mainArticleRecord.MainArticle = true;
                _articleRepository.Update(mainArticleRecord);
            }

            foreach (var categoryArticle in model.CategoryArticleList)
            {
                var oldCategoriesArticles = _articleCategoryRepository.Table.Where(r => r.CategoryRecord.Id == categoryArticle.CategoryId);
                foreach (var articleCategoryRecord in oldCategoriesArticles) {
                    var articlePart = _orchardServices.ContentManager.Get<ArticlePart>(articleCategoryRecord.ArticleRecord.Id, VersionOptions.Latest);
                    if (articlePart != null && articlePart.IssuePart != null && articlePart.IssuePart.Id == issueId)
                    {
                        _articleCategoryRepository.Delete(articleCategoryRecord);    
                    }                    
                }

                var categoryRecord = _categoryRepository.Get(categoryArticle.CategoryId);

                if(categoryArticle.FistArticleId != null) {
                    var articleRecord = _articleRepository.Get(categoryArticle.FistArticleId.Value);
                    _articleCategoryRepository.Create(new ArticleCategoryRecord
                    {
                        ArticleRecord = articleRecord,
                        CategoryRecord = categoryRecord,
                        CategoryDisplayOrder = 1
                    });
                }

                if (categoryArticle.SecondArticleId != null)
                {
                    var articleRecord = _articleRepository.Get(categoryArticle.SecondArticleId.Value);
                    _articleCategoryRepository.Create(new ArticleCategoryRecord
                    {
                        ArticleRecord = articleRecord,
                        CategoryRecord = categoryRecord,
                        CategoryDisplayOrder = 2
                    });
                }

                if (categoryArticle.ThirdArticleId != null)
                {
                    var articleRecord = _articleRepository.Get(categoryArticle.ThirdArticleId.Value);
                    _articleCategoryRepository.Create(new ArticleCategoryRecord
                    {
                        ArticleRecord = articleRecord,
                        CategoryRecord = categoryRecord,
                        CategoryDisplayOrder = 3
                    });
                }

                if (categoryArticle.FourthArticleId != null)
                {
                    var articleRecord = _articleRepository.Get(categoryArticle.FourthArticleId.Value);
                    _articleCategoryRepository.Create(new ArticleCategoryRecord
                    {
                        ArticleRecord = articleRecord,
                        CategoryRecord = categoryRecord,
                        CategoryDisplayOrder = 4
                    });
                }
            }
        }

        public CategoryArticleViewModel GetCategoryArticle(int issueId, int categoryId, string categoryName)
        {
            List<int> articleItemQuery = _orchardServices.ContentManager
                                                             .Query()
                                                             .ForVersion(VersionOptions.Latest)
                                                             .Join<CommonPartRecord>()
                                                             .Where(x => x.Container != null && x.Container.Id == issueId)
                                                             .ForPart<ArticlePart>().List().Select(x => x.Id).ToList<int>();

            var categoryArticleList = _articleCategoryRepository.Table.Where(x => x.CategoryRecord.Id == categoryId && articleItemQuery.Contains(x.ArticleRecord.Id));

            int? firstArticleId = null;
            int? secondArticleId = null;
            int? thirdArticleId = null;
            int? fourthArticleId = null;
            int? categoryArticleId = null;

            foreach (var categoryArticle in categoryArticleList.ToList()) {
                categoryArticleId = categoryArticle.Id;
                switch (categoryArticle.CategoryDisplayOrder) {
                    case 1:
                        firstArticleId = categoryArticle.ArticleRecord.Id;
                        break;
                    case 2:
                        secondArticleId = categoryArticle.ArticleRecord.Id;
                        break;
                    case 3:
                        thirdArticleId = categoryArticle.ArticleRecord.Id;
                        break;
                    case 4:
                        fourthArticleId = categoryArticle.ArticleRecord.Id;
                        break;
                }       
            }

            return new CategoryArticleViewModel {
                CategoryArticleId = categoryArticleId,
                CategoryId = categoryId,
                Name = categoryName,
                FistArticleId = firstArticleId,
                SecondArticleId = secondArticleId,
                ThirdArticleId = thirdArticleId,
                FourthArticleId = fourthArticleId
            };
        }

        public void ChangePosition(int[] ids)
        {
            for (int i = 0; i < ids.Count(); i++ )
            {
                var category = _categoryRepository.Table.Where(c => c.Id == ids[i]).FirstOrDefault();
                if(category != null) {
                    category.Position = i + 1;
                    _categoryRepository.Update(category);
                }
            }
        }

        public int GetPositionByCategoryArticle(string categoryName, int articleId)
        {
            var categoryArticle = _articleCategoryRepository.Table.FirstOrDefault(x => x.CategoryRecord.Name == categoryName && x.ArticleRecord.Id == articleId);

            if(categoryArticle == null) {
                return 0;
            }

            return categoryArticle.CategoryDisplayOrder.Value;
        }
    }
}