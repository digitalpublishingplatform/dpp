using System.Web.Routing;
using DigitalPublishingPlatform;
using Orchard.Environment;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using DigitalPublishingPlatform.Helpers;
using System;

namespace DigitalPublishingPlatform
{
    public class AdminMenu : INavigationProvider
    {                
        private readonly Work<RequestContext> _requestContextAccessor;
        public Localizer T { get; set; }

        public AdminMenu(Work<RequestContext> requestContextAccessor) {
            _requestContextAccessor = requestContextAccessor;

            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            var requestContext = _requestContextAccessor.Value;
            var idValue = (string)requestContext.RouteData.Values["id"];
            var id = string.Empty;
            if (!String.IsNullOrEmpty(idValue)) id = idValue;
            builder
                   .AddImageSet("publicationframework")
                   .Add(T("Digital Publishing \r\n Platform"), "6", menu =>
                   {
                       menu.LinkToFirstChild(true);
                       menu.Add(T("Media Service Gallery"), "0", item => item.Action("Index", "Admin", new { area = Constants.Area }).Permission(Permissions.PublicationFramework));
                       menu.Add(T("Add New Media"), "1", item => item.Action("Create", "Admin", new { area = Constants.Area }).Permission(Permissions.PublicationFramework));
                       menu.Add(T("Edit Media"), "2", item => item.Action("Edit", "Admin", new { area = Constants.Area, id }).Permission(Permissions.PublicationFramework).LocalNav());
                       menu.Add(T("Encoded Files"), "3", item => item.Action("EncodedList", "Admin", new { area = Constants.Area, id }).Permission(Permissions.PublicationFramework).LocalNav());
                       menu.Add(T("Publications"), "4", item => item.Action("Index", "Publication", new { area = Constants.Area }).Permission(Permissions.PublicationFramework));
                       menu.Add(T("Add New Publication"), "5", item => item.Action("Create", "Publication", new { area = Constants.Area }).Permission(Permissions.PublicationFramework));
                   });
            
        }
    }
}