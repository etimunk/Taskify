using System.ComponentModel.DataAnnotations;

namespace Taskify.Models
{
    public class TasksPostModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ProjectId is required and must be a valid project.")]
        public int ProjectId { get; set; }
        public int UserId { get; set; }
    }

    public class TaskUpdateModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int? Priority { get; set; }
        public int? UserId { get; set; }  // שיוך לעובד אחר
    }

    public class TaskCreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; } = "pending";

        [Range(1, 5)]
        public int Priority { get; set; } = 3;

        [Required]
        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
