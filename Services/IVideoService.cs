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
    public interface IVideoService : IDependency {
        /// <summary>
        /// Updates the videos of content item
        /// </summary>
        /// <param name="item">The ContentItem parameter</param>
        /// <param name="ids">The url list of videos</param>
        void UpdateVideosForContentItem(ContentItem item, int[] ids);

        
        IEnumerable<MediaItemViewModel> GetMediaList(int videoPartId);

        /// <summary>
        /// Gets video list of article
        /// </summary>
        /// <param name="id">Numeric id of the article</param>
        /// <returns>An enumeration of VideoViewModel</returns>
        IEnumerable<VideoViewModel> GetVideoList(int id);

        /// <summary>
        /// Gets all videos filtered by parameter Find
        /// </summary>
        /// <param name="model">The VideoPickerViewModel parameter</param>
        /// <param name="pagerParameters">The paging parameters</param>
        /// <returns>A VideoPickerViewModel list</returns>
        VideoPickerViewModel FindVideo(VideoPickerViewModel model, PagerParameters pagerParameters);

        /// <summary>
        /// Gets all videos including in list of id
        /// </summary>
        /// <param name="ids">The list of id</param>
        /// <returns>An EditVideosViewModel</returns>
        EditVideosViewModel GetVideoList(int[] ids);
    }
}
