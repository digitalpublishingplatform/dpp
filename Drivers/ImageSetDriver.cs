using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace DigitalPublishingPlatform.Drivers
{
    public class ImageSetDriver : ContentPartDriver<ImageSetPart>
    {
        private readonly IImageService _imageService;
        private const string TemplateName = "Parts/ImageSet";
        protected override string Prefix{
            get { return "ImageSet"; }
        }

        public ImageSetDriver(IImageService imageService) {
            _imageService = imageService;
        }
   
        protected override DriverResult Editor(
            ImageSetPart part,
            dynamic shapeHelper)
        {

            return ContentShape("Parts_ImageSet_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: BuildEditorViewModel(part),
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(
            ImageSetPart part,
            IUpdateModel updater,
            dynamic shapeHelper)
        {
            var model = new ImageSetViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);
            if (part.ContentItem.Id != 0)
            {
                _imageService.UpdateImagesForContentItem(part.ContentItem, model.Urls.ToList());
            }
            return Editor(part, shapeHelper);
        }

        private ImageSetViewModel BuildEditorViewModel(ImageSetPart part)
        {
            var urlList = _imageService.GetImageUrlList(part.Id);
            return new ImageSetViewModel
            {
                Urls = urlList.ToArray()
            };
        }
    }
}