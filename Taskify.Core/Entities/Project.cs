namespace Taskify.Core.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int ManagerId {  get; set; }
        public User Manager { get; set; }  // קשר למנהל (User)
        public List<Tasks> Tasks { get; set; }  // קשר למשימות
    }
}
