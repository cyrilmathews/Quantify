using System.ComponentModel.DataAnnotations;

namespace Quantify.Estimates.Core.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public required string Code { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Timestamp]
        public required byte[] SourceVersion { get; set; }

        [Required]
        public DateTime ReplicatedOn { get; set; } = DateTime.UtcNow;
    }
}
