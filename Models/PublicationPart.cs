using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Security;
using Orchard.Core.Title.Models;

namespace DigitalPublishingPlatform.Models
{
    public class PublicationPart : ContentPart
    {
        public string Title
        {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Text
        {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public IUser Owner
        {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public bool IsPublished
        {
            get { return ContentItem.VersionRecord != null && ContentItem.VersionRecord.Published; }
        }

        public bool HasDraft
        {
            get
            {
                return (
                           (ContentItem.VersionRecord != null) && (
                               (ContentItem.VersionRecord.Published == false) ||
                               (ContentItem.VersionRecord.Published && ContentItem.VersionRecord.Latest == false)));
            }
        }

        public string Url {
            set { ContentItem.As<ImagePart>().Url = value; }
            get { return ContentItem.As<ImagePart>().Url; }
        }

        public bool HasPublished
        {
            get
            {
                return IsPublished || ContentItem.ContentManager.Get(Id, VersionOptions.Published) != null;
            }
        }

        public DateTime? CreatedUtc
        {
            get { return this.As<ICommonPart>().CreatedUtc; }
        }

        public DateTime? PublishedUtc
        {
            get { return this.As<ICommonPart>().PublishedUtc; }
        }

        public DateTime? ModifiedUtc {
            get { return this.As<ICommonPart>().ModifiedUtc; }
        }
    }
}