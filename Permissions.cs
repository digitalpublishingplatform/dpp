using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace DigitalPublishingPlatform
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission PublicationFramework = new Permission { Description = "Microsoft Digital Publishing Platform", Name = "MicrosoftDigitalPublishingPlatform" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
                   {
                       PublicationFramework,
                   };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
                   {
                       new PermissionStereotype
                       {
                           Name = "Administrator",
                           Permissions = new[] {PublicationFramework}
                       },
                       new PermissionStereotype
                       {
                           Name = "Editor",
                           Permissions = new[] {PublicationFramework}
                       },
                       new PermissionStereotype
                       {
                           Name = "Moderator",
                       },
                       new PermissionStereotype
                       {
                           Name = "Author",
                           Permissions = new[] {PublicationFramework}
                       },
                       new PermissionStereotype
                       {
                           Name = "Contributor",
                       },
                   };
        }
    }
}