using API.Models;

namespace API.Interface
{
    public interface AppInterface
    {
        Task<CreateUser> GetUserdependence(Guid UserID);
        Task<List<CreateUser>> GetAllNewUsers();
        Task<CreateUser> CreateUser(CreateUser createUser); 
        //Task<List<CreateBlog>> GetAllBlogs(int page);
        Task<CreateBlog> CreateNewBlog(CreateBlog createBlog);
    }
}
