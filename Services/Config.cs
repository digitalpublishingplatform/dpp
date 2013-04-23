using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitalPublishingPlatform.Models;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;

namespace DigitalPublishingPlatform.Services
{
    public class Config: IConfig {        
        private readonly PublicationFrameworkSettingPart _settingsPart;        

        public Config(IOrchardServices orchardServices) {

            if (orchardServices.WorkContext != null) {
                _settingsPart = orchardServices.WorkContext.CurrentSite.As<PublicationFrameworkSettingPart>();
            }
        }

        public string MediaServiceAccount
        {
            get { return _settingsPart.MediaServiceAccount; }
        }

        public string MediaServiceKey
        {
            get { return _settingsPart.MediaServiceKey; }
        }

        public string BlobStorageAccount
        {
            get { return _settingsPart.BlobStorageAccount; }
        }

        public string BlobStorageKey
        {
            get { return _settingsPart.BlobStorageKey; }
        }
    }
}