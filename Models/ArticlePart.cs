using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;
using Orchard.Tags.Models;

namespace DigitalPublishingPlatform.Models
{
    public class ArticlePart : ContentPart<ArticleRecord>
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

        public IssuePart IssuePart
        {
            get { return this.As<ICommonPart>().Container.As<IssuePart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public IUser Owner
        {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public List<string> Categories {
            get { return Record.ArticleCategoryRecords
                               .OrderBy(c => c.CategoryRecord.Position)
                               .Select(x => x.CategoryRecord.Name)
                               .ToList(); }
        }

        public string Author {
            get { return Record.Author; }
            set { Record.Author = value; }
        }
        public bool MainArticle
        {
            get { return Record.MainArticle; }
            set { Record.MainArticle = value; }
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

        public DateTime? PublishedUtc
        {
            get { return this.As<ICommonPart>().PublishedUtc; }
        }

        public DateTime? CreatedUtc
        {
            get { return this.As<ICommonPart>().CreatedUtc; }
        }

        public DateTime? ModifiedUtc
        {
            get { return this.As<ICommonPart>().ModifiedUtc; }
        }

        public string Url 
        {
            get { return this.As<ImagePart>().Url; }
            set { this.As<ImagePart>().Url = value; }
        }
    }
}