using Statiq.Common;
using System.Collections.Generic;
using System.Linq;

namespace Docs
{
    public static class DocumentExtensions
    {
        public static string GetDescription(this IDocument document)
        {
            return document?.GetString(Constants.Description, string.Empty) ?? string.Empty;
        }

        public static bool IsVisible(this IDocument document)
        {
            if (document.GetContentStringAsync().GetAwaiter().GetResult().Contains("<title>Redirected</title>"))
            {
                return false;
            }

            return document.GetBool(Constants.ShowInSidebar, true);
        }

        public static bool ShowLink(this IDocument document)
        {
            return !document.GetBool(Constants.NoLink, false);
        }
    }
}