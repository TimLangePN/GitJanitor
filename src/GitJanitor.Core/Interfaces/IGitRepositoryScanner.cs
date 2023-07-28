using GitJanitor.Core.Models;

namespace GitJanitor.Core.Interfaces;
public interface IGitRepositoryScanner
{
    Task<List<GitRepository>> ScanForRepositoriesAsync(string path, string organization);
}