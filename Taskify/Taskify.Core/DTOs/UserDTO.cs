using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.Entities;

namespace Taskify.Core.DTOs
{
    public class UserDTO
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
