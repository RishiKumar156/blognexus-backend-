using API.Data;
using API.MapperProfile;
using API.Models;
using API.Models.MDTOS;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;

namespace API.Interface
{
    public class Service : AppInterface
    {
        private readonly BlogContext _context;
        public Service(BlogContext context)
        {
            _context = context;
        }

        public async Task<CreateUser> GetUserdependence(Guid UserID)
        {
            var db = await _context.NewUser.FindAsync(UserID);
            return db;
        }

        public async Task<CreateUser> CreateUser(CreateUser createUser)
        {
            await _context.NewUser.AddAsync(createUser);
            await _context.SaveChangesAsync();
            return createUser;
        }

        public async  Task<List<CreateUser>> GetAllNewUsers()
        {
            return  await _context.NewUser.ToListAsync();        
        }

        public async Task<CreateBlog> CreateNewBlog(CreateBlog createBlog)
        {
            await _context.NewBlogs.AddAsync(createBlog);
            await _context.SaveChangesAsync();
            return createBlog;
        }
    }
}
