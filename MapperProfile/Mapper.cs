using API.Models;
using API.Models.MDTOS;
using AutoMapper;

namespace API.MapperProfile
{
    public class Mapper : Profile
    {
        public  Mapper()
        {
            CreateMap<CreateUser, CreateUserDTO>().ReverseMap();
            CreateMap<CreateBlog, CreateBlogDTO>().ReverseMap();
        }
    }
}
