using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services
{
    public interface IArticleService : IDependency
    {
        /// <summary>
        /// Gets all articles filtered by Issue and parameter Find
        /// </summary>
        /// <param name="model">The ArticleItemListViewModel parameter</param>
        /// <param name="issueId">Numeric id of Issue</param>
        /// <param name="pagerParameters">The paging parameters</param>
        /// <returns>An ArticleItemViewModel list</returns>
        ArticleItemListViewModel GetAllItems(ArticleItemListViewModel model, int issueId, PagerParameters pagerParameters);

        /// <summary>
        /// Gets all articles corresponding to Issue 
        /// </summary>
        /// <param name="issueId">Numeric id of Issue</param>
        /// <returns>An enumeration of ArticleItemViewModel</returns>
        IEnumerable<ArticleItemViewModel> GetAllItems(int issueId);

        /// <summary>
        /// Gets main article corresponding to Issue
        /// </summary>
        /// <param name="issueId">Numeric id of Issue</param>
        /// <returns>The ArticleItemViewModel corresponding to main article</returns>
        ArticleItemViewModel GetMainArticle(int issueId);

        /// <summary>
        /// Get Id of main article corresponding to Issue
        /// </summary>
        /// <param name="issueId">Numeric id of Issue</param>
        /// <returns>Numeric id of main article</returns>
        int? GetMainArticleId(int issueId);
    }
}