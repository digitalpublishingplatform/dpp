using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Helpers
{
    public static class ContentExtensions
    {
        public static string Owner(this ContentItem contentItem) {
            if (contentItem == null) {
                return "";
            }
            var commontPart = contentItem.As<CommonPart>();
            if (commontPart == null || commontPart.Owner == null) {
                return "";
            }
            return commontPart.Owner.UserName;
        }
    }
}