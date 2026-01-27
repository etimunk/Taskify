

namespace Taskify.Models
{
    public class TaskPostModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }  
        public int UserId { get; set; } 

    }
}
