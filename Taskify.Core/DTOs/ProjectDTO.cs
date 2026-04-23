using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taskify.Core.DTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int ManagerId { get; set; }
    }

    public class ProjectExtendedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }    // שם המנהל
        public string ManagerEmail { get; set; }   // אימייל המנהל

        // סטטיסטיקות
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int PendingTasks { get; set; }
        public double CompletionPercentage { get; set; }

        // רשימת משימות מורחבת (אופציונלי)
        public List<TasksExtendedDTO> Tasks { get; set; }
    }

    public class ProjectSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ManagerName { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double CompletionPercentage { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}
