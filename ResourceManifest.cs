using Orchard.UI.Resources;

namespace DigitalPublishingPlatform {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("PublicationFrameworkAdmin.Style").SetUrl("menu.publicationframework-admin.css");
            manifest.DefineStyle("PublicationFrameworkAdmin.Style.MediaElement").SetUrl("mediaelement/mediaelementplayer.css");
            manifest.DefineStyle("PublicationFrameworkAdmin.Style.MediaElement.Skins").SetUrl("mediaelement/mejs-skins.css");
            
            manifest.DefineScript("PublicationFrameworkAdmin.Script.Plupload").SetUrl("plupload/plupload.full.js");
            manifest.DefineScript("PublicationFrameworkAdmin.Script.MediaElement").SetUrl("mediaelement/mediaelement-and-player.js").SetDependencies("jQuery");
            manifest.DefineScript("PublicationFrameworkAdmin.Script").SetUrl("script.js").SetDependencies("PublicationFrameworkAdmin.Script.Plupload");
            manifest.DefineScript("PublicationFrameworkAdmin.Script.VideoPicker").SetUrl("videopicker.js").SetDependencies("jQuery");
            manifest.DefineScript("PublicationFrameworkAdmin.Script.JQueryCopyToClipboard").SetUrl("jquery.copytoclipboard.js").SetDependencies("jQuery");
            manifest.DefineScript("PublicationFrameworkAdmin.Script.ImagePicker").SetUrl("imagepicker.js").SetDependencies("jQuery");
        }
    }
}