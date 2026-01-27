using System.ComponentModel.DataAnnotations;

namespace CoworkerHub.Core
{
    public class Workspace
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public ICollection<Desk> Desks { get; set; } = new List<Desk>();
    }
}
