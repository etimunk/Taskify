namespace Taskify.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string TZ { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public IsManager Level { get; set; }
        public List<Tasks> Tasks { get; set; }  // קשר למשימות

        public enum IsManager
        {
            worker, manager, headmanager
        }
    }
}
