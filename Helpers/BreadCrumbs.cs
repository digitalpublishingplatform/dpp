using DigitalPublishingPlatform.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Helpers
{
    public static class BreadCrumbs
    {
        /// <summary>
        /// Get navigation list using full path
        /// </summary>
        /// <returns>Navigation list</returns>
        public static IEnumerable<Navigation> GetNavigationHierarchy(string fullPath, WorkContext workContext)
        {
            var navigations = new List<Navigation>();
            int indexStart = fullPath.IndexOf("/Publication/") + 1;
            string[] navigationParts = fullPath.Substring(indexStart, (fullPath.Length - indexStart)).Split('/');
            string currentPath = fullPath.Substring(0, indexStart - 1);
            string partName;
            int index = 0;
            while( index < (navigationParts.Count() - 1)){
                switch (navigationParts[index])
                {
                    case("Publication"):
                        partName = "Publications";
                        break;
                    case ("Issue"):
                        partName = "Issues";
                        break;
                    case ("Article"):
                        partName = "Articles";
                        break;
                    case ("Category"):
                        partName = "Categories";
                        break;
                    default:
                        partName = "";
                        break;
                }

                var orchardServices = workContext.Resolve<IOrchardServices>();
                var name = "";
                if (index < (navigationParts.Count() - 2))
                {
                    var navigationValue = GetNavigationValue(navigationParts[index + 1]);
                    if (navigationValue > 0) {
                        name = GetNavigationName(partName, navigationValue, orchardServices);
                    }
                }

                currentPath = currentPath + "\\" + navigationParts[index];
                index++;
                if (partName != "")
                {
                    navigations.Add(new Navigation { Name = partName, Path = currentPath });
                }
                if(name != "") {
                    navigations.Add(new Navigation { Name = name, Path = "" });    
                }
            }

            return navigations;
        }

        private static int GetNavigationValue(string navigationPart) {
            int navigationValue = 0;
            int.TryParse(navigationPart, out navigationValue);

            return navigationValue;
        }

        private static string GetNavigationName(string partName, int id, IOrchardServices orchardServices)
        {
            string name = "";
            switch (partName)
            {
                case ("Publications"):
                    var publicationPart = orchardServices.ContentManager.Get<PublicationPart>(id, VersionOptions.Latest);
                    name = publicationPart != null ? publicationPart.Title : partName;
                    break;
                case ("Issues"):
                    var issuePart = orchardServices.ContentManager.Get<IssuePart>(id, VersionOptions.Latest);
                    name = issuePart != null ? issuePart.Title : partName;
                    break;
                case ("Articles"):
                    var articlePart = orchardServices.ContentManager.Get<ArticlePart>(id, VersionOptions.Latest);
                    name = articlePart != null ? articlePart.Title : "Article";
                    break;
                case ("Categories"):
                    name = "Category";
                    break;
                default:
                    name = partName;
                    break;
            }

            return name;
        }
    }
}
