using DigitalPublishingPlatform.Helpers;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;

namespace DigitalPublishingPlatform.Models
{    
    public class MediaItemPart: ContentPart<MediaItemRecord>{
        
        /// <summary>
        /// Media title
        /// </summary>
        public string Title {
            get { return ContentItem.As<TitlePart>().Title; }            
        }
        /// <summary>
        /// Media description
        /// </summary>
        public string Description
        {
            get { return ContentItem.As<BodyPart>().Text; }            
        }
        /// <summary>
        /// Blob url
        /// </summary>
        public string Url
        {
            get { return Record.Url; }
            set { Record.Url = value; }
        }
        /// <summary>
        /// Indicates if this media file is available in frontend
        /// </summary>
        public bool Published
        {
            get { return ContentItem.As<CommonPart>().IsPublished(); }            
        }
       
        /// <summary>
        /// Indicate type of media file: Audio, Video
        /// </summary>
        public MediaType Type
        {
            get { return Record.Type; }
            set { Record.Type = value; }
        }
        /// <summary>
        /// Main file mime info
        /// </summary>
        public string MimeType
        {
            get { return Record.MimeType; }
            set { Record.MimeType = value; }
        }
        /// <summary>
        /// Original filename
        /// </summary>
        public string Filename
        {
            get { return Record.Filename; }
            set { Record.Filename = value; }
        }
        /// <summary>
        /// Blobstorage assetId
        /// </summary>
        public string AssetId
        {
            get { return Record.AssetId; }
            set { Record.AssetId= value; }
        }       

        public long Size {
            get { return Record.Size; }
            set { Record.Size = value; }
        }

        public string LongTitle {
            get { return String.Format("{0} ({1})", Title, Filename); }
        }

        public string DefaultThumbnailUrl{
            get { return Record.DefaultThumbnailUrl; }
            set { Record.DefaultThumbnailUrl = value; }
        }

        public DateTime? CreatedUtc
        {
            get { return ContentItem.As<CommonPart>().CreatedUtc; }
        }

        public DateTime? ModifiedUtc
        {
            get { return ContentItem.As<CommonPart>().ModifiedUtc; }
        }

        public string Owner {
            get { return ContentItem.Owner(); }
        }

        public string FileToken {
            get { return Record.FileToken; }
            set { Record.FileToken = value; }
        }
    }
}