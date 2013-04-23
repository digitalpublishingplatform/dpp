using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services
{
    public interface IPublicationService : IDependency
    {
        /// <summary>
        /// Gets all articles filtered by parameter Find
        /// </summary>
        /// <param name="model">The PublicationItemListViewModel parameter</param>
        /// <param name="pagerParameters">The paging parameters</param>
        /// <returns>A PublicationItemViewModel list</returns>
        PublicationItemListViewModel GetAllItems(PublicationItemListViewModel model, PagerParameters pagerParameters);

        /// <summary>
        /// Gets the publication with the specified id and version
        /// </summary>
        /// <param name="id">Numeric id of the publication</param>
        /// <param name="versionOptions">The version option</param>
        /// <returns>A Publication part</returns>
        PublicationPart Get(int id, VersionOptions versionOptions);
    }
}