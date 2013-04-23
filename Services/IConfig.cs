using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace DigitalPublishingPlatform.Services
{
    public interface IConfig : IDependency
    {
        string MediaServiceAccount { get; }
        string MediaServiceKey { get; }
        string BlobStorageAccount { get; }
        string BlobStorageKey { get; }
    }
}