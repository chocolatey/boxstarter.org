using System.Threading.Tasks;
using Docs.Shortcodes;
using Statiq.App;
using Statiq.Common;
using Statiq.Markdown;
using Statiq.Web;

namespace Docs
{
  public static class Program
  {
    public static async Task<int> Main(string[] args) =>
        await Bootstrapper.Factory
            .CreateWeb(args)
            .AddSetting(Keys.Host, "boxstarter.org")
            .AddSetting(Keys.LinksUseHttps, true)
            .AddSetting(Constants.EditLink, ConfigureEditLink())
            .AddSetting(WebKeys.GatherHeadingsLevel, 5)
            .ConfigureSite("chocolatey", "boxstarter.org", "master")
            .ConfigureTemplates(templates => ((RenderMarkdown)templates[MediaTypes.Markdown].Module).UseExtension(new Markdig.Extensions.Emoji.EmojiExtension(new Markdig.Extensions.Emoji.EmojiMapping(true))))
            .AddShortcode("Children", typeof(ChildrenShortcode))
            .AddPipelines()
            .RunAsync();

    private static Config<string> ConfigureEditLink()
    {
      return Config.FromDocument((doc, ctx) =>
      {
        return string.Format("https://github.com/{0}/{1}/edit/{2}/input/{3}",
                  ctx.GetString(Constants.Site.Owner),
                  ctx.GetString(Constants.Site.Repository),
                  ctx.GetString(Constants.Site.Branch),
                  doc.Source.GetRelativeInputPath());
      });
    }
  }
}
