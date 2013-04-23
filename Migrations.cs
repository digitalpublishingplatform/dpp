using System.Data;
using Orchard.Core.Contents.Extensions;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using Orchard.Indexing;
using Orchard.Localization;
using DigitalPublishingPlatform.Helpers;

namespace DigitalPublishingPlatform
{
    public class Migrations : DataMigrationImpl
    {
        private readonly IRepository<EncodingPresetRecord> _encodingPresetRepository;
        protected Localizer T { set; get; }

        public Migrations(IRepository<EncodingPresetRecord> encodingPresetRepository)
        {
            _encodingPresetRepository = encodingPresetRepository;T = NullLocalizer.Instance;
        }
       
        public int Create() {
            SchemaBuilder
                .CreateTable(typeof(MediaItemRecord).Name, table => table
                    .ContentPartRecord()                                                           
                    .Column<string>("Type")
                    .Column<string>("MimeType")
                    .Column<string>("Url", c => c.WithLength(5000))
                    .Column<string>("Filename", c => c.WithLength(5000))
                    .Column<string>("AssetId", c => c.WithLength(5000))
                    .Column<Int64>("Size", c => c.WithDefault(0)));

            ContentDefinitionManager
                .AlterPartDefinition(typeof(MediaItemPart).Name, cfg => cfg.Attachable());
            
            ContentDefinitionManager.AlterTypeDefinition(Constants.MediaItem, cfg => cfg
                .WithPart(typeof(MediaItemPart).Name)
                .WithPart("TitlePart")
                .WithPart("BodyPart", bodyPart => bodyPart.WithSetting("BodyTypePartSettings.Flavor", "plain"))                
                .WithPart("CommonPart", commomPart => commomPart.WithSetting("DateEditorSettings.ShowDateEditor", "true"))
                .WithPart("TagsPart")                
                .Draftable()                
                .Indexed());

            SchemaBuilder
                .CreateTable(typeof(PublicationFrameworkSettingPartRecord).Name, table =>
                                                                                  table
                                                                                      .ContentPartRecord()
                                                                                      .Column<string>("MediaServiceAccount", c => c.WithLength(1000))
                                                                                      .Column<string>("MediaServiceKey", c => c.WithLength(1000))
                                                                                      .Column<string>("BlobStorageAccount", c => c.WithLength(1000))
                                                                                      .Column<string>("BlobStorageKey", c => c.WithLength(1000)));            

            ContentDefinitionManager.AlterPartDefinition(typeof(PublicationFrameworkSettingPart).Name, cfg => cfg.Attachable());            
            
            ContentDefinitionManager.AlterPartDefinition(typeof(PublicationPart).Name, cfg => cfg.Attachable());
            ContentDefinitionManager.AlterTypeDefinition(Constants.Publication, cfg => cfg
               .WithPart(typeof(PublicationPart).Name)
               .WithPart("TitlePart")
               .WithPart("BodyPart")
               .WithPart("CommonPart")
               .WithPart("TagsPart")
               .Draftable()
               .Indexed());
       
            ContentDefinitionManager.AlterPartDefinition(typeof(ArticlePart).Name, cfg => cfg.Attachable());
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                .WithPart("TitlePart")
                .WithPart("ArticlePart")
                .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-article'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .WithPart("CommonPart")
                .WithPart("TagsPart")
                .Draftable()
                .Indexed());

            ContentDefinitionManager.AlterPartDefinition(typeof(IssuePart).Name, cfg => cfg.Attachable());
            ContentDefinitionManager.AlterTypeDefinition(Constants.Issue, cfg => cfg
               .WithPart("IssuePart")
               .WithPart("TitlePart")
               .WithPart("BodyPart", bodyPart => bodyPart.WithSetting("BodyTypePartSettings.Flavor", "plain"))
               .WithPart("CommonPart")
               .Draftable()
               .Indexed());        
        
            SchemaBuilder
                .CreateTable(typeof (EncodedMediaRecord).Name, table =>
                                                               table
                                                                   .Column<int>("Id", c => c.PrimaryKey().Identity())
                                                                   .Column<string>("JobErrorMessage", c => c.Unlimited())
                                                                   .Column<string>("JobId")
                                                                   .Column<string>("AssetId")
                                                                   .Column<string>("Url", c => c.Unlimited())                                                                   
                                                                   .Column<string>("Status")
                                                                   .Column<string>("Metadata", c => c.Unlimited())
                                                                   .Column<int>("EncodingPreset_Id")
                                                                   .Column<DateTime>("CreatedUtc", c => c.Nullable())
                                                                   .Column<DateTime>("ModifiedUtc", c => c.Nullable())
                                                                   .Column<string>("Owner")
                                                                   .Column<int>("MediaItem_Id"));

            SchemaBuilder.CreateForeignKey("FK_MediaItemRecordId", "EncodedMediaRecord", new[] { "MediaItem_Id" }, "MediaItemRecord", new[] { "Id" });
            
            
            SchemaBuilder.CreateTable(typeof (EncodingPresetRecord).Name, table =>
                                                                          table.Column<int>("Id", c => c.PrimaryKey().Identity())
                                                                               .Column<string>("Name")
                                                                               .Column<string>("Description", c => c.WithLength(1000))
                                                                               .Column<string>("MediaType")
                                                                               .Column<string>("Definition")
                                                                               .Column<string>("Target")
                                                                               .Column<string>("ShortDescription")
                                                                               .Column<int>("Width"));

            SchemaBuilder.CreateForeignKey("FK_EncodingPresetRecordId", "EncodedMediaRecord", new[] { "EncodingPreset_Id" }, "EncodingPresetRecord", new[] { "Id" });       
            #region Encoding presets initialization
            foreach (var preset in EncodingPreset.GetPresets().Where(p => p.IsAvailable).Select(encondingPreset => new EncodingPresetRecord
            {
                Definition = encondingPreset.Definition,
                Id = encondingPreset.Id,
                MediaType = encondingPreset.MediaType,
                Name = encondingPreset.Name,
                Description = encondingPreset.Description,
                ShortDescription = encondingPreset.ShortDescription,
                Target = encondingPreset.Target,
                Width = encondingPreset.Width
            }))
            {
                _encodingPresetRepository.Create(preset);
            }
            _encodingPresetRepository.Flush(); 
            #endregion
            
            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder
                .CreateTable(typeof (ThumbnailRecord).Name, table => table
                                                                         .Column<int>("Id", c => c.PrimaryKey().Identity())
                                                                         .Column<string>("Url", c => c.Unlimited())
                                                                         .Column<int>("EncodedMedia_Id"));
            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder
                .AlterTable(typeof (EncodedMediaRecord).Name, table =>
                                                              table
                                                                  .AlterColumn("Metadata", c => { 
                                                                      c.WithLength(15000);
                                                                      c.WithType(DbType.String);
                                                                  }));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder
                .AlterTable(typeof(EncodedMediaRecord).Name, table =>
                                                              table
                                                                  .AlterColumn("Metadata", c =>
                                                                  {
                                                                      c.Unlimited();
                                                                      c.WithType(DbType.String);
                                                                  }));
            return 4;
        }

        public int UpdateFrom4() {
            SchemaBuilder
                .AlterTable(typeof (EncodedMediaRecord).Name, table => {
                    table.AddColumn<int>("Width");
                    table.AddColumn<int>("Height");
                    table.AddColumn<int>("Framerate");
                    table.AddColumn<decimal>("AspectRatio", c => c.WithPrecision(20).WithScale(6));
                });
            return 5;
        }

        public int UpdateFrom5() {
            SchemaBuilder
                .AlterTable(typeof (EncodedMediaRecord).Name, table => table.AlterColumn("Framerate", c => c.WithType(DbType.Decimal, 20, 6)));
    
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.CreateTable(typeof(VideoRecord).Name, table => 
                table.ContentPartRecord()
            );

            SchemaBuilder.CreateTable(typeof(VideoMediaItemRecord).Name, table =>                
                table.Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("MediaItemRecord_Id")
                .Column<int>("VideoRecord_Id"));

            ContentDefinitionManager.AlterPartDefinition(typeof (VideoPart).Name, builder =>
                                                                                  builder.Attachable());

            return 7;
        }

        public int UpdateFrom7() {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                                                                                       .WithPart("VideoPart"));               
            return 8;
        }

        public int UpdateFrom8() {
            SchemaBuilder.AlterTable(typeof (MediaItemRecord).Name, table =>
                                                                    table.AddColumn<string>("DefaultThumbnailUrl", c => c.Unlimited()));
            return 9;
        }

        public int UpdateFrom9()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Publication, cfg => cfg
                                                                                       .WithPart("PublishLaterPart"));
            return 10;
        }

        public int UpdateFrom10()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Issue, cfg => cfg
                                                                                       .WithPart("PublishLaterPart"));
            return 11;
        }

        public int UpdateFrom11()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                                                                                       .WithPart("PublishLaterPart"));
            return 12;
        }

        public int UpdateFrom12()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Publication, cfg => cfg
                                                                                       .WithPart("ImagePart"));
            return 13;
        }


        public int UpdateFrom13() {

            SchemaBuilder
                .CreateTable(typeof (ImageRecord).Name, table =>
                                                                                  table
                                                                                      .ContentPartRecord()
                                                                                      .Column<string>("Url", c => c.Unlimited()));
            return 14;
        }

        public int UpdateFrom14()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Issue, cfg => cfg
                                                                                       .WithPart("ImagePart"));
            return 15;
        }

        public int UpdateFrom15()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                                                                                       .WithPart("ImagePart"));
            return 16;
        }

        public int UpdateFrom16() {
            SchemaBuilder
                .CreateTable(typeof (ArticleRecord).Name, table =>
                                                        table
                                                            .ContentPartRecord()
                                                            .Column<string>("Author"));
            return 17;
        }

        public int UpdateFrom17() {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                .RemovePart("AutoroutePart"));

            return 18;
        }

        public int UpdateFrom18()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Publication, cfg => cfg.RemovePart("TagsPart"));
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg.RemovePart("TagsPart"));

            SchemaBuilder.CreateTable(typeof (CategoryRecord).Name, table => table
                .ContentPartRecord()                
                .Column<int>("Position")
                .Column<string>("Name", c => c.Unlimited()));

            ContentDefinitionManager.AlterPartDefinition(typeof(CategoryPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(Constants.Category, cfg => cfg
               .WithPart(typeof(CategoryPart).Name)               
               .WithPart("CommonPart")
               .Draftable()
               .Indexed());

            SchemaBuilder.CreateTable(typeof(ArticleCategoryRecord).Name, table =>
              table.Column<int>("Id", c => c.PrimaryKey().Identity())
              .Column<int>("ArticleRecord_Id")
              .Column<int>("CategoryRecord_Id"));
            return 19;
        }

        public int UpdateFrom19()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                                                                                       .WithPart("CategoriesPart"));
            return 20;
        }

        public int UpdateFrom20() {
            SchemaBuilder
                .AlterTable(typeof (ArticleRecord).Name, table =>
                                                         table
                                                             .AddColumn<bool>("MainArticle"));                                                             
            return 21;
        }
         
        public int UpdateFrom21() {
            SchemaBuilder
                .AlterTable(typeof (ArticleCategoryRecord).Name, table =>
                                                         table
                                                             .AddColumn<int>("CategoryDisplayOrder", c => c.Nullable()));                                                             
            return 22;
        }

        public int UpdateFrom22()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg.RemovePart("CategoriesPart"));
            return 23;
        }

        public int UpdateFrom23()
        {

            SchemaBuilder
                .CreateTable(typeof(ImageSetItemRecord).Name, table =>
                                                                                  table
                                                                                      .ContentPartRecord()
                                                                                      .Column<string>("Url", c => c.Unlimited())
                                                                                      .Column<int>("ImageSet_Id"));
            return 24;
        }

        public int UpdateFrom24()
        {
            ContentDefinitionManager.AlterTypeDefinition(Constants.Article, cfg => cfg
                                                                                       .WithPart("ImageSetPart"));
            return 25;
        }

        public int UpdateFrom25()
        {

            SchemaBuilder.CreateTable(typeof(ImageSetRecord).Name, table =>table.ContentPartRecord());
            return 26;
        }

        public int UpdateFrom26() {
            ContentDefinitionManager.AlterPartDefinition(typeof (ImageSetPart).Name, builder =>
                                                                                     builder.Attachable());
            return 27;
        }

        public int UpdateFrom27()
        {
            SchemaBuilder.DropTable(typeof(ImageSetItemRecord).Name);
            SchemaBuilder.CreateTable(typeof(ImageSetItemRecord).Name, table =>table
                                                                                       .Column<int>("Id", c => c.Identity().PrimaryKey())
                                                                                       .Column<string>("Url", c => c.Unlimited())
                                                                                       .Column<int>("ImageSet_Id")); 
            return 28;
        }

        public int UpdateFrom28()
        {
            SchemaBuilder.AlterTable(typeof(ImageSetItemRecord).Name, table => table.AddColumn<int>("Position"));
            SchemaBuilder.AlterTable(typeof(VideoMediaItemRecord).Name, table => table.AddColumn<int>("Position"));
            return 29;
        }

        public int UpdateFrom29() {
            SchemaBuilder.AlterTable(typeof (CategoryRecord).Name, table => table
                                                                                .AddColumn<int>("PublicationId"));
            return 30;
        }

        public int UpdateFrom30()
        {
            SchemaBuilder.AlterTable(typeof(MediaItemRecord).Name, table => table
                                                                                .AddColumn<string>("FileToken"));
            return 31;
        }

        
    }
}