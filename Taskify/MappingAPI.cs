using AutoMapper;
using Taskify.Core.Entities;
using Taskify.Models;

namespace Taskify
{
    public class MappingAPI: Profile
    {
        public MappingAPI()
        {
            CreateMap<ProjectPostModel, Project>().ReverseMap();
            CreateMap<TasksPostModel, Tasks>().ReverseMap();
            CreateMap<UserPostModel, User>().ReverseMap();
        }
        
    }
}
