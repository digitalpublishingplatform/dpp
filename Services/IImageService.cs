using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using DigitalPublishingPlatform.ViewModels.Api;
using Orchard.UI.Navigation;
using DigitalPublishingPlatform.Drivers;

namespace DigitalPublishingPlatform.Services
{
    public interface IImageService : IDependency {
        /// <summary>
        /// Updates the images of content item
        /// </summary>
        /// <param name="item">The ContentItem parameter</param>
        /// <param name="urls">The url list of images</param>
        void UpdateImagesForContentItem(ContentItem item, List<string> urls);

        /// <summary>
        /// Gets url list of images
        /// </summary>
        /// <param name="imageSetPartId">The imageSetPartId parameter</param>
        /// <returns>An url list of images</returns>
        List<string> GetImageUrlList(int imageSetPartId);

        /// <summary>
        /// Gets url list of images by article
        /// </summary>
        /// <param name="articleId">Numeric id of the article</param>
        /// <returns>An url list of images by article</returns>
        List<string> GetImageUrlListByArticle(int articleId);
    }
}
