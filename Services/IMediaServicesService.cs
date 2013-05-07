using System.Threading.Tasks;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MediaServices.Client;
using Orchard;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services
{
    public interface IMediaServicesService : IDependency
    {   
        /// <summary>
        /// Gets all existent formats in data base with available codecs, disabling those that are already completed or are in-progress, by mediaItemId
        /// </summary>
        /// <param name="mediaItemId">Numeric id of the mediaItem</param>
        /// <returns>An EncodingViewModel</returns>
        EncodingViewModel InitEncodingViewModel(int mediaItemId);        

        /// <summary>
        /// Creates an asset and uploads a single file
        /// </summary>
        /// <param name="filename">The media file name</param>
        /// <param name="oldContainerName">The old container name</param>
        /// <param name="alternateId">Numeric id of the alternate</param>
        /// <returns>An AzureUploadResultViewModel</returns>
        AzureUploadResultViewModel CreateAssetAndUploadSingleFile(string filename, string oldContainerName, string alternateId);

        /// <summary>
        /// Deletes asset by Id
        /// </summary>
        /// <param name="assetId">Id of the asset</param>
        /// <param name="context">The CloudMediaContext parameter</param>
        void DeleteByAssetId(string assetId, CloudMediaContext context = null);

        /// <summary>
        /// Creates an encoding job
        /// </summary>
        /// <param name="assetId">Id of the asset</param>
        /// <param name="encodingId">Numeric id of the encoding</param>
        /// <param name="username">The user name</param>
        Task<IJob> CreateEncodingJob(string assetId, int encodingId, string username);
        bool UploadChunk(string name, int p, int chunks, System.IO.Stream stream, string token);

        /// <summary>
        /// Gets file info by token
        /// </summary>
        /// <param name="token">The token parameter</param>
        /// <returns>A FileInfoViewModel</returns>
        FileInfoViewModel GetFileInfoByToken(string token);

        /// <summary>
        /// Gets all media item filtered by parameter Find
        /// </summary>
        /// <param name="model">The MediaItemListViewModel parameter</param>
        /// <param name="pagerParameters">The paging parameters</param>
        /// <returns>A MediaItemViewModel list</returns>
        MediaItemListViewModel GetAllItems(MediaItemListViewModel model, PagerParameters pagerParameters);

        /// <summary>
        /// Gets a encoded list of a media item
        /// </summary>
        /// <param name="id">Numeric id of the media item</param>
        /// <param name="pagerParameters">The paging parameters</param>
        /// <returns>A EncodedMediaViewModel list</returns>
        EncodedListViewModel GetEncodedList(int id, PagerParameters pagerParameters);


        Tuple<string, List<string>, int> ImageList(int id);

        /// <summary>
        /// Gets information of encoded item
        /// </summary>
        /// <param name="id">Numeric id of the encoded item</param>
        /// <returns>A VideoPlayerViewModel</returns>
        VideoPlayerViewModel GetEncodedItem(int id);


        /// <summary>
        /// Sets the default thumbnail url of a media item
        /// </summary>
        /// <param name="id">Numeric id of the encoded item</param>
        /// <param name="thumbnailUrl">The thumbnail url of media item</param>
        void SetDefaultThumbnail(int id, string thumbnailUrl);

        /// <summary>
        /// Deletes a encoded item from data base and media service by Id
        /// </summary>
        /// <param name="id">Numeric id of the encoded item</param>
        void DeleteEncoded(int id);

        /// <summary>
        /// Gets the current encoding amount of a media item
        /// </summary>
        /// <param name="mediaItemId">Numeric id of the media item</param>
        /// <returns>A numeric value of current encoding amount of a media item</returns>
        int GetCurrentEncodingCount(int mediaItemId);

        /// <summary>
        /// Removes a media item from data base and media service
        /// </summary>
        /// <param name="mediaItemPart">The mediaItemPart parameter</param>
        void RemoveMediaItem(MediaItemPart mediaItemPart );

        List<string> GetRelatedContentTitle(int mediaItemId);
    }
}
