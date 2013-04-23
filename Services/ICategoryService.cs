using System.Collections.Generic;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.ViewModels.Category;
using Orchard;
using Orchard.ContentManagement;
using DigitalPublishingPlatform.Models;
namespace DigitalPublishingPlatform.Services
{
    public interface ICategoryService: IDependency
    {
        /// <summary>
        /// Gets all categories filtered by Publication
        /// </summary>
        /// <param name="categoryItemListViewModel">The CategoryItemListViewModel parameter</param>
        /// <param name="publicationId">Numeric id of Publication</param>
        /// <returns>An CategoryItemListViewModel List</returns>
        CategoryItemListViewModel GetAll(CategoryItemListViewModel categoryItemListViewModel, int publicationId);

        IEnumerable<CategoryLightViewModel> GetList(int articleId);

        /// <summary>
        /// Gets all categories filtered by Publication
        /// </summary>
        /// <param name="publicationId">Numeric id of Publication</param>
        /// <returns>An enumeration of all published categories  corresponding to Publication and sorted by position</returns>
        IEnumerable<CategoryViewModel> GetAll(int publicationId);        

        /// <summary>
        /// Updates the Issue structure
        /// </summary>
        /// <param name="item">The content item of categoriesPart</param>
        /// <param name="model">The EditCategoriesViewModel parameter</param>
        void UpdateCategoryForContentItem(ContentItem item, EditCategoriesViewModel model);

        /// <summary>
        /// Changes position of category using direction
        /// </summary>
        /// <param name="id">Numeric id of Category</param>
        /// <param name="direction">The direction parameter</param>
        void ChangePosition(int id, Direction direction);        

        /// <summary>
        /// Gets all articles filtered by IssueId and CategoryId with your corresponding position
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="categoryId">Numeric id of Category</param>
        /// <param name="categoryName">The CategoryName parameter</param>
        /// <returns>An CategoryArticleViewModel</returns>
        CategoryArticleViewModel GetCategoryArticle(int issueId, int categoryId, string categoryName);

        /// <summary>
        /// Inserts articles and categories of Issue Structure
        /// </summary>
        /// <param name="model">The CategoriesArticlesViewModel parameter</param>
        /// <param name="currentMainArticleId">The current main article</param>
        /// <param name="issueId">Numeric id of Issue</param>
        void InsertIssueFront(CategoriesArticlesViewModel model, int? currentMainArticleId, int issueId);

        /// <summary>
        /// Updates the position of the categories
        /// </summary>
        /// <param name="ids">The categories list with new order</param>
        void ChangePosition(int[] ids);

        /// <summary>
        /// Gets position of article inside Issue Structure by category
        /// </summary>
        /// <param name="categoryName">The CategoryName parameter</param>
        /// <param name="articleId">Numeric id of Article</param>
        /// <returns>The position of article</returns>
        int GetPositionByCategoryArticle(string categoryName, int articleId);
    }
}
