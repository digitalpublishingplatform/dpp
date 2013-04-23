using DigitalPublishingPlatform.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Drivers
{
    public class ArticleDriver : ContentPartDriver<ArticlePart> {
        private readonly IRepository<ArticleRecord> _repository;
        private readonly IOrchardServices _services;

        public ArticleDriver(IRepository<ArticleRecord> repository, IOrchardServices services) {
            _repository = repository;
            _services = services;
        }

        protected override DriverResult Display(ArticlePart part, string displayType, dynamic shapeHelper){
            return ContentShape("Parts_Article", () => shapeHelper.DisplayTemplate(TemplateName: "Parts/Article", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ArticlePart part, dynamic shapeHelper) {
            return ContentShape("Parts_Article_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Article", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ArticlePart part, IUpdateModel updater, dynamic shapeHelper) {
            
            updater.TryUpdateModel(part, Prefix, null, null);
            
            if (part.MainArticle)
            {
                // get another existing main article and change this property to false
                var articleParts = _services.ContentManager.Query<ArticlePart>()
                                                               .ForVersion(VersionOptions.Latest)
                                                               .List()
                                                               .Where(a => a.MainArticle && a.IssuePart.Id == part.IssuePart.Id && a.Id != part.Id);
            
                foreach (var articlePart in articleParts)
                {
                    articlePart.Record.MainArticle = false;
                    _repository.Update(articlePart.Record);
                }
            }
            
            return Editor(part, shapeHelper);
        }
    }    
}