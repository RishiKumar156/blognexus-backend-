using API.Models;
using API.Models.MDTOS;

namespace API.MapperProfile
{
    public class CreateBlogsPagination
    {
        public List<CreateBlog> CreatedBlogs { get; set; } = new List<CreateBlog>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
