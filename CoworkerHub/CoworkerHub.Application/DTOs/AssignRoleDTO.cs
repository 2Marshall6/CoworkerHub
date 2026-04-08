namespace CoworkerHub.Application.DTOs.Authentication
{
    public class AssignRoleDTO
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}