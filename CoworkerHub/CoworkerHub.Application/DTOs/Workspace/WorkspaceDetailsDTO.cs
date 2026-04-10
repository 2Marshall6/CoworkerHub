using CoworkerHub.Application.DTOs.Desk;

namespace CoworkerHub.Application.DTOs.Workspace
{
    public class WorkspaceDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<DeskDTO> Desks { get; set; }
    }
}
