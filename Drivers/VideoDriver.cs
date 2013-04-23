using DigitalPublishingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace DigitalPublishingPlatform.Drivers
{
    public class VideoDriver : ContentPartDriver<VideoPart> {
        private readonly IVideoService _videoService;
        private const string TemplateName = "Parts/Videos";
        protected override string Prefix{
            get { return "Videos"; }
        }

        public VideoDriver(IVideoService videoService) {
            _videoService = videoService;
        }
   
        protected override DriverResult Editor(
            VideoPart part,
            dynamic shapeHelper)
        {

            return ContentShape("Parts_Videos_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: BuildEditorViewModel(part),
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(
            VideoPart part,
            IUpdateModel updater,
            dynamic shapeHelper)
        {
            var model = new EditVideosViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);
            if (part.ContentItem.Id != 0)
            {
                _videoService.UpdateVideosForContentItem(part.ContentItem, model.Ids);
            }
            return Editor(part, shapeHelper);
        }

        private EditVideosViewModel BuildEditorViewModel(VideoPart part) {            
            var videos = _videoService.GetMediaList(part.Id);            
            return new EditVideosViewModel {                
                Videos = videos,
                Ids = videos.Select(v => v.Id).ToArray()
            };            
        }
    }
}