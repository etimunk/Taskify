using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.DTOs
{
    public class TasksDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }  // ProjectId במקום Project
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? ProjectDueDate { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? ProjectManagerEmail { get; set; }
    }

    public class TasksExtendedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }  // שם הפרויקט
        public int UserId { get; set; }
        public string UserName { get; set; }     // שם העובד
        public string UserEmail { get; set; }    // אימייל העובד
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
