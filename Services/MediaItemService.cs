using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using Orchard;
using Orchard.ContentManagement;
using DigitalPublishingPlatform.Helpers;

namespace DigitalPublishingPlatform.Services
{
    public class MediaItemService: IMediaItemService
    {
        private readonly IContentManager _contentManager;        

        public MediaItemService(IContentManager contentManager) {
            _contentManager = contentManager;            
        }

        public MediaItemPart NewMediaItem() {
            return _contentManager.New<MediaItemPart>(Constants.MediaItem);
        }

    }
}