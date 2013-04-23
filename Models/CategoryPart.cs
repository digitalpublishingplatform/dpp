using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;
using Orchard.Tags.Models;

namespace DigitalPublishingPlatform.Models
{
    public class CategoryPart: ContentPart<CategoryRecord>
    {
        public string Name
        {
            get { return Record.Name; }
            set { Record.Name = value; }
        }
        
        public IUser Owner
        {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public int Position  
        {
            get { return Record.Position; }
            set { Record.Position = value; }
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

        public DateTime? ModifiedUtc
        {
            get { return this.As<ICommonPart>().ModifiedUtc; }
        }

        public int PublicationId
        {
            get { return Record.PublicationId; }
            set { Record.PublicationId = value; }
        }  
    }
}