using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using DigitalPublishingPlatform.Services;
using DigitalPublishingPlatform.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace DigitalPublishingPlatform.Drivers
{
    public class CategoriesDriver : ContentPartDriver<CategoriesPart>
    {
        private readonly ICategoryService _categoryService;
        private const string TemplateName = "Parts/Categories";

        protected override string Prefix
        {
            get { return "Categories"; }
        }

        public CategoriesDriver(ICategoryService categoryService) {
            _categoryService = categoryService;
        }

        protected override DriverResult Editor(
            CategoriesPart part,
            dynamic shapeHelper)
        {

            return ContentShape("Parts_Categories_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: BuildEditorViewModel(part),
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(
            CategoriesPart part,
            IUpdateModel updater,
            dynamic shapeHelper)
        {
            var model = new EditCategoriesViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);
            if (part.ContentItem.Id != 0)
            {
                _categoryService.UpdateCategoryForContentItem(part.ContentItem, model);
            }
            return Editor(part, shapeHelper);
        }

        private EditCategoriesViewModel BuildEditorViewModel(CategoriesPart part)
        {
            var categories = _categoryService.GetList(part.Id).ToList();
            return new EditCategoriesViewModel
            {
                Categories = categories
               
            };
        }
    }
}