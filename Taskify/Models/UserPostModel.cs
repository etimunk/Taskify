using Taskify.Core.Entities;

namespace Taskify.Models
{
    public class UserPostModel
    {
        public int Id { get; set; }
        public string TZ { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
