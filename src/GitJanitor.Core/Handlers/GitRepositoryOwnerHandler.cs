using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace GitJanitor.Core.Handlers;

public class GitRepositoryOwnerHandler
{
    private static readonly Dictionary<string, string> Platforms = new()
    {
        { "github.com", @"github\.com[:/](.+?)/" },
        { "bitbucket.org", @"bitbucket\.org[:/](.+?)/" },
        { "gitlab.com", @"gitlab\.com[:/](.+?)/" }
        // Add other platforms as needed
    };

    public static string? Getowner(string path, string ownerInput)
    {
        using var repo = new Repository(path);

        // Get the "origin" remote
        var remote = repo.Network.Remotes["origin"];

        // Get the URL
        var url = remote.Url;

        foreach (var platform in Platforms)
        {
            if (url.Contains(platform.Key))
            {
                var match = Regex.Match(url, platform.Value);
                if (match.Success) return match.Groups[1].Value;
            }

            if (url.Contains(ownerInput)) return ownerInput;
        }

        throw new Exception($"{url}, seems to not point to a valid git repository");
    }
}