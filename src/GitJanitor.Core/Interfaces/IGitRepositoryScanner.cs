using LibGit2Sharp;

namespace GitJanitor.Core.Interfaces;

public interface IGitRepositoryScanner
{
    Task<IList<Repository>> ScanForRepositoriesAsync(string path, string owner);
}