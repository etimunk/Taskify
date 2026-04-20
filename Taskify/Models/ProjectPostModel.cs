namespace Taskify.Models
{
    public class ProjectPostModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int ManagerId { get; set; }
    }

    public class ProjectUpdateModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Status { get; set; }
        public int? ManagerId { get; set; }  // שיוך למנהל אחר
    }
}
