using Taskify.Core.Entities;

namespace Taskify.Core.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string TZ { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public User.IsManager Level { get; set; }
    }

    public class UserSummaryDTO
    {
        public int Id { get; set; }
        public string TZ { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public User.IsManager Level { get; set; }

        // סטטיסטיקות
        public int AssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int ManagedProjects { get; set; }
    }
}
