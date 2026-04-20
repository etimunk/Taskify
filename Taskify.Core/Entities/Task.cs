namespace Taskify.Core.Entities
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }  // ProjectId במקום Project
        public Project Project { get; set; }  // קשר לפרויקט

        public int UserId { get; set; }  // UserId במקום User
        public User User { get; set; }  // קשר למשתמש

    }
}
