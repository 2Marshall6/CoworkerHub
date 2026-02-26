using System.ComponentModel.DataAnnotations;

namespace CoworkerHub.Domain.Entities
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Desk> Desks { get; set; } = new List<Desk>();
    }
}
