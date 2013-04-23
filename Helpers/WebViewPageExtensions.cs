using System.Web;

namespace DigitalPublishingPlatform.Helpers
{
    public static class WebViewPageExtensions
    {
        public static string ReturnUrl(this System.Web.Mvc.WebViewPage webViewPage)
        {
            return HttpUtility.UrlPathEncode(webViewPage.Request.RawUrl);
        }
    }
}