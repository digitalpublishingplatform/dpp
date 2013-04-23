using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Navigation;
using DigitalPublishingPlatform.Helpers;

namespace DigitalPublishingPlatform.Services
{
    public class PublicationService : IPublicationService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IShapeFactory _shapeFactory;
        private readonly ISiteService _siteService;

        public PublicationService(IOrchardServices orchardServices, IContentManager contentManager, IShapeFactory shapeFactory, ISiteService siteService)
        {
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
            _siteService = siteService;
        }

        public PublicationItemListViewModel GetAllItems(PublicationItemListViewModel model, PagerParameters pagerParameters) {
            var find = model.Find ?? "";
            var publicationItemQuery = _orchardServices.ContentManager.HqlQuery().ForPart<PublicationPart>()
                                                                      .ForType(Constants.Publication)
                                                                      .ForVersion(VersionOptions.Latest)
                                                                      .List()
                                                                      .Where(m => find == ""
                                                                                  || m.Title.ToLowerInvariant().Contains(find));

            var count = publicationItemQuery.Count();
            // when is selected the option 'Show All' in pagination,the page size parameter is equal to 0
            if (pagerParameters.PageSize == 0)
            {
                // assign at page size parameter value the total number of records found
                pagerParameters.PageSize = count;
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            dynamic shape = _shapeFactory;

            model = new PublicationItemListViewModel
            {
                Find = find,
                Pager = shape.Pager(pager).TotalItemCount(count),
                PublicationItems = publicationItemQuery.Select(x => new PublicationItemViewModel
                {
                    Title = x.Title,
                    Content = x.Text,
                    Published = x.IsPublished(),
                    CreatedUtc = x.CreatedUtc,
                    ModifiedUtc = x.ModifiedUtc,
                    Id = x.Id
                }).Skip((pager.Page - 1) * pager.PageSize).Take(pager.PageSize == 0 ? count:pager.PageSize).ToList()
            };

            return model;
        }

        public PublicationPart Get(int id, VersionOptions versionOptions)
        {
            return _contentManager.Get<PublicationPart>(id, VersionOptions.Latest);
        }
    }
}