using DigitalPublishingPlatform.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace DigitalPublishingPlatform.Drivers
{    
    public class PublicationFrameworkSettingPartDriver: ContentPartDriver<PublicationFrameworkSettingPart>
    {
        public Localizer T { get; set; }
        private const string TemplateName = "Parts/Share.Settings";
        private readonly INotifier _notifier;

        public PublicationFrameworkSettingPartDriver(INotifier notifier,
           IOrchardServices services)
        {
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(PublicationFrameworkSettingPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Share_Settings",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: part,
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(PublicationFrameworkSettingPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (!updater.TryUpdateModel(part, Prefix, null, null))
            {
                _notifier.Error(T("Error during content sharing settings update!"));
            }            
            return Editor(part, shapeHelper);
        }
    }
}