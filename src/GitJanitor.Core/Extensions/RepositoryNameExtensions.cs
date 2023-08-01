using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace GitJanitor.Core.Extensions;

public static class RepositoryNameExtensions
{
    private static readonly Regex _gitUrlRegex = new(@".*/(.+?)(\.git)?$", RegexOptions.Compiled);

    public static string GetRepositoryName(this Repository repository)
    {
        {
            var remote = repository.Network.Remotes["origin"];
            if (remote == null)
                return Path.GetFileName(repository.Info.WorkingDirectory.TrimEnd(Path.DirectorySeparatorChar));
            // Extract the repo name from the remote URL
            var url = remote.Url;
            var match = _gitUrlRegex.Match(url);
            return match.Success ? match.Groups[1].Value :
                // If the remote URL does not match the expected pattern,
                // return the name of the directory that contains the .git folder
                Path.GetFileName(repository.Info.WorkingDirectory.TrimEnd(Path.DirectorySeparatorChar));
        }
    }
}