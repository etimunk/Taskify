using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Taskify.Core.DTOs;
using Taskify.Core.Entities;

namespace Taskify.Core
{
    public class MappingProfile:Profile
    {

        public MappingProfile()
        {
            CreateMap<Tasks, TasksDTO>().ReverseMap();
            CreateMap<Project, ProjectDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
