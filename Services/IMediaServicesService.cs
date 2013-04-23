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
        EncodingViewModel InitEncodingViewModel(int mediaItemId);        
        AzureUploadResultViewModel CreateAssetAndUploadSingleFile(string filename, string oldContainerName, string alternateId);
        void DeleteByAssetId(string assetId, CloudMediaContext context = null);
        Task<IJob> CreateEncodingJob(string assetId, int encodingId, string username);
        bool UploadChunk(string name, int p, int chunks, System.IO.Stream stream, string token);
        FileInfoViewModel GetFileInfoByToken(string token);
        MediaItemListViewModel GetAllItems(MediaItemListViewModel model, PagerParameters pagerParameters);
        EncodedListViewModel GetEncodedList(int id, PagerParameters pagerParameters);
        Tuple<string, List<string>, int> ImageList(int id);
        VideoPlayerViewModel GetEncodedItem(int id);
        void SetDefaultThumbnail(int id, string thumbnailUrl);
        void DeleteEncoded(int id);
        int GetCurrentEncodingCount(int mediaItemId);
        void RemoveMediaItem(MediaItemPart mediaItemPart );
        List<string> GetRelatedContentTitle(int mediaItemId);
    }
}
