
namespace CoworkerHub.Domain.Entities
{
    public class Desk
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }

        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
    }
}
