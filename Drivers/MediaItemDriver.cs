using DigitalPublishingPlatform.Helpers;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System;
using Orchard.Localization;

namespace DigitalPublishingPlatform.Drivers
{
    public class MediaItemDriver: ContentPartDriver<MediaItemPart>{
        private IMediaServicesService _mediaServicesService;
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public MediaItemDriver(IOrchardServices services) {
            Services = services;            
            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(MediaItemPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_MediaItem", () => shapeHelper.DisplayTemplate(TemplateName: "Parts/MediaItem", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MediaItemPart part, dynamic shapeHelper) {
            if (String.IsNullOrEmpty(part.FileToken)) {
                part.FileToken = Guid.NewGuid().ToString();
            }
            return ContentShape("Parts_MediaItem_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/MediaItem", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MediaItemPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            _mediaServicesService = Services.WorkContext.Resolve<IMediaServicesService>();
            var file = _mediaServicesService.GetFileInfoByToken(part.FileToken);            
            if (file != null)
            {
                if (file.AssetId != part.AssetId) {
                    //TODO: Delete all encoded files
                }
                part.Filename = file.Filename;
                part.MimeType = file.MimeType;
                part.Url = file.Url;
                part.AssetId = file.AssetId;
                part.Type = file.MimeType.GetMediaType();
                part.Size = file.Size;
            }
            else
            {                
                updater.AddModelError("", T("Media file not selected."));
            }             
            return Editor(part, shapeHelper);
        }
    }
}