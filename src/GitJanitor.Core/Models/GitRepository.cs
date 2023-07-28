namespace GitJanitor.Core.Models
{
    public class GitRepository
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Owner { get; set; }
    }
}
