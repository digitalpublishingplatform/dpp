using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DigitalPublishingPlatform.Models;
using Orchard.Data;

namespace DigitalPublishingPlatform.Drivers
{
    public class CategoryDriver: ContentPartDriver<CategoryPart>
    {
        private readonly IRepository<CategoryRecord> _repository;

        public CategoryDriver(IRepository<CategoryRecord> repository) {
            _repository = repository;
        }

        protected override DriverResult Editor(CategoryPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Category_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Category", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(CategoryPart part, IUpdateModel updater, dynamic shapeHelper) {
            var max = 0;            
            if (part.Position == 0) {
                // get next value for category position and save this value
                var result = _repository.Table.Where(c => c.PublicationId == part.PublicationId);
                if(result.Any()) {
                    max = result.Max(c => c.Position) + 1;   
                }
                else {
                    max = 1;
                }
            }
            updater.TryUpdateModel(part, Prefix, null, null);

            if (max > 0) {
                var category = _repository.Get(part.Id);
                if (category != null) {
                    category.Position = max;
                    category.PublicationId = part.PublicationId;
                }
            }
            
            return Editor(part, shapeHelper);
        }
    }  
}