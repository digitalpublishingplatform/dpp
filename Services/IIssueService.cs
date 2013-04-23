using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services
{
    public interface IIssueService : IDependency
    {
        /// <summary>
        /// Gets all issues filtered by Publication and parameter Find 
        /// </summary>
        /// <param name="model">The IssueItemListViewModel parameter</param>
        /// <param name="publicationId">Numeric id of Publication</param>
        /// <param name="pagerParameters">The paging parameter</param>
        /// <returns>An IssueItemViewModel list</returns>
        IssueItemListViewModel GetAllItems(IssueItemListViewModel model, int publicationId, PagerParameters pagerParameters);

        /// <summary>
        /// Gets all category names corresponding a articles of Issue
        /// </summary>
        /// <param name="id">Numeric id of Issue</param>
        /// <returns>An enumeration of category names</returns>
        IEnumerable<string> TagsByIssueId(int id); 
       
        MainIssueFrontViewModel IssueFront(int id);
    }
}