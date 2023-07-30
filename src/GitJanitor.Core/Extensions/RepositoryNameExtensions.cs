using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace GitJanitor.Core.Extensions;

public static class RepositoryNameExtensions
{
    public static string GetRepositoryName(this Repository repository)
    {
        {
            var remote = repository.Network.Remotes["origin"];
            if (remote != null)
            {
                // Extract the repo name from the remote URL
                var url = remote.Url;
                var match = Regex.Match(url, @".*/(.+?)(\.git)?$");
                if (match.Success) return match.Groups[1].Value;
            }

            // If the remote URL does not match the expected pattern,
            // return the name of the directory that contains the .git folder
            return Path.GetFileName(repository.Info.WorkingDirectory.TrimEnd(Path.DirectorySeparatorChar));
        }
    }
}