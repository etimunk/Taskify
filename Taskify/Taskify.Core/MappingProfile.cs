using AutoMapper;
using Taskify.Core.DTOs;
using Taskify.Core.Entities;
using Taskify.Models;
namespace Taskify.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Tasks, TaskDTO>().ReverseMap();
            CreateMap<Project, ProjectDTO>().ReverseMap();

            CreateMap<ProjectPostModel, Project>().ReverseMap();
            CreateMap<TaskPostModel, Tasks>().ReverseMap();
            CreateMap<UserPostModel, User>().ReverseMap();
        }
    }
}
