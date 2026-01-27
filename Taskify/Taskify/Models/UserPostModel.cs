
namespace Taskify.Models
{
    public class UserPostModel
    {
        public int Id { get; set; }
        public string TZ { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public IsManager Level { get; set; }
        public enum IsManager
        {
            worker, manager, headmanager
        }
    }
}
