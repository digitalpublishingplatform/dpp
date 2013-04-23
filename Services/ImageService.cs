using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.ViewModels;
using DigitalPublishingPlatform.ViewModels.Api;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace DigitalPublishingPlatform.Services {
    public class ImageService : IImageService {
        private readonly IRepository<ImageSetRecord> _imageSetRepository;
        private readonly IRepository<ImageSetItemRecord> _imageSetItemRepository;
        private readonly IRepository<ArticleRecord> _articleRepository;
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly IShapeFactory _shapeFactory;

        public ImageService(IRepository<ImageSetRecord> imageSetRepository,
            IRepository<ImageSetItemRecord> imageSetItemRepository,
            IRepository<ArticleRecord> articleRepository,
            IOrchardServices orchardServices, 
            ISiteService siteService,
            IShapeFactory shapeFactory) {
            _imageSetRepository = imageSetRepository;
            _imageSetItemRepository = imageSetItemRepository;
            _articleRepository = articleRepository;
            _orchardServices = orchardServices;
            _siteService = siteService;
            _shapeFactory = shapeFactory;
        }

        public void UpdateImagesForContentItem(ContentItem item, List<string> urls) {            
            
            var imageSetPart = item.As<ImageSetPart>().Record;
            
            var oldImageList = _imageSetItemRepository.Table.Where(i => i.ImageSet.Id == item.Id).ToList();
            foreach (var oldImageRecord in oldImageList) {
                _imageSetItemRepository.Delete(oldImageRecord);
            }
            var position = 0;
            foreach (var url in urls) {
                _imageSetItemRepository.Create(new ImageSetItemRecord
                {                    
                    Url = url,
                    Position = position,
                    ImageSet = imageSetPart
                });
                position++;
            }
        }

        public List<string> GetImageUrlList(int imageSetPartId) {
            return _imageSetItemRepository.Table.Where(r => r.ImageSet.Id == imageSetPartId).OrderBy(r => r.Position).Select(r => r.Url).ToList();            
        }

        public List<string> GetImageUrlListByArticle(int articleId) {

            var articlePart = _orchardServices.ContentManager.Query()
                .ForVersion(VersionOptions.Latest)
                .Join<CommonPartRecord>()
                .ForPart<ArticlePart>().List().FirstOrDefault(x => x.Id == articleId);

            if(articlePart == null) {
                return null;
            }

            return GetImageUrlList(articlePart.As<ImageSetPart>().Id);
        }
    }

}