using LibGit2Sharp;
using System.Text.RegularExpressions;

namespace GitJanitor.Core.Handlers
{
    public class OrganizationHandler
    {
        private static Dictionary<string, string> platforms = new Dictionary<string, string>
        {
            { "github.com", @"github\.com[:/](.+?)/" },
            { "bitbucket.org", @"bitbucket\.org[:/](.+?)/" },
            { "gitlab.com", @"gitlab\.com[:/](.+?)/" },
            // Add other platforms as needed
        };

        public static string? GetOrganization(string path, string organizationInput)
        {

            using var repo = new Repository(path);

            // Get the "origin" remote
            var remote = repo.Network.Remotes["origin"];

            // Get the URL
            var url = remote.Url;

            foreach (var platform in platforms)
            {
                if (url.Contains(platform.Key))
                {
                    var match = Regex.Match(url, platform.Value);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }

                if (url.Contains(organizationInput))
                {
                    return organizationInput;
                }

            }
            throw new Exception($"{url}, seems to not point to a valid git repository");
        }
    }
}
