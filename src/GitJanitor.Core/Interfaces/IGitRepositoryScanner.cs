using GitJanitor.Core.Models;
using LibGit2Sharp;

namespace GitJanitor.Core.Interfaces;
public interface IGitRepositoryScanner
{
    Task<List<Repository>> ScanForRepositoriesAsync(string path, string organization);
}