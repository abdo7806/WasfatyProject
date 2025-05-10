namespace Wasfaty.Application.DTOs.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserRoleEnum Role { get; set; }
        public DateTime CreatedAt { get; set; }
       // public DateTime UpdatedAt { get; set; }
        public string RoleName { get; set; } = string.Empty;

        /*public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;*/
    }
}
