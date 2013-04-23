using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace DigitalPublishingPlatform.Drivers
{
    public class ImageDriver : ContentPartDriver<ImagePart>
    {
        private const string TemplateName = "Parts/Image";
        protected override string Prefix
        {
            get { return "Image"; }
        }

        protected override DriverResult Editor(
            ImagePart part,
            dynamic shapeHelper)
        {

            return ContentShape("Parts_Image_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: part,
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(ImagePart part, IUpdateModel updater, dynamic shapeHelper)
        {            
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}