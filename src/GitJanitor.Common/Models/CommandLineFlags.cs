using System.ComponentModel.DataAnnotations;
using GitJanitor.Common.Enums;

namespace GitJanitor.Common.Models
{
    public class CommandLineFlags
    {
        [Required]
        [StringLength(256)]
        public string WorkingDirectory { get; set; }
        
        [Required]
        public GitRepositoryAction Action { get; set; }
        
        [StringLength(256)]
        public string? Organization { get; set; }
        
        [StringLength(256)]
        public string? TargetDirectory { get; set; }
    }
}
