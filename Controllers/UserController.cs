using API.Data;
using API.Interface;
using API.MapperProfile;
using API.Models;
using API.Models.MDTOS;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppInterface _appInterface;
        private readonly BlogContext _context;

        public UserController(IMapper mapper, AppInterface appInterface, BlogContext context)
        {
            _mapper = mapper;
            _appInterface = appInterface;
            _context = context;
        }


        [HttpGet("{userID}")]

        public async Task<ActionResult> GetNewUser(Guid userID)
        {
            var db = await _appInterface.GetUserdependence(userID);
            var map = _mapper.Map<CreateUserDTO>(db);
            return Ok(map);
        }

        [HttpPost("RegisterNewUser")]
        public async Task<ActionResult> CreateNewUser(CreateUser createUser)
        {
            await _appInterface.CreateUser(createUser);
            return Ok(createUser);
        }

        [HttpGet("NewlyCreatedUser")]
        public async Task<ActionResult> GetCreatedUsers()
        {
            var allUsers = await _appInterface.GetAllNewUsers();
            var map = _mapper.Map<List<CreateUserDTO>>(allUsers);
            return Ok(map);
        }

        //Login function 

        [HttpPost("LoginUser")]

        public async Task<ActionResult<LoginDTO>> GetLogin(LoginDTO loginData)
        {

            var userLogData = await _context.NewUser.FirstOrDefaultAsync(c => c.UserEmail == loginData.UserEmail && c.UserPassword == loginData.UserPassword);

            if (userLogData != null) {
                return Ok(userLogData);
            } else
            {
                return BadRequest("You'r not authorized");
            }
        }

        //Get all the created blogs at once
        [HttpGet("GetAllBlog")]
        public async Task<ActionResult> GetAllBlog(int Pages)
        {
            //this below codes are implemented to pare the blog creator name from user creator table which usese the common guid generated for both the user and blog which is craeted when the user created
            var blogCreatorID = await _context.NewBlogs.ToListAsync();
            foreach(var item in blogCreatorID)
            {
                var MatchingID = await _context.NewUser.SingleOrDefaultAsync(c => c.UserID.ToString() == item.BlogCreator);
            }
            //This below code is implemented to make the pagination 
            var totalPages = 3f;
            var asyncount = await _context.NewBlogs.CountAsync();
            var pagesToLoad = (int)Math.Ceiling(asyncount / totalPages);
            var db = await _context.NewBlogs.OrderByDescending(c => c.BlogCreated)
                .Skip((Pages - 1) * (int)(totalPages)).Take((int)totalPages).ToListAsync();

            var response = new CreateBlogsPagination
            {
                CreatedBlogs = db,
                CurrentPage = Pages,
                TotalPages = pagesToLoad
            };
            //var map = _mapper.Map<List<CreateBlogDTO>>(response);
            return Ok(response);
        }

        //To create new Blog

        [HttpPost("CreateBlog")]
        public async Task<ActionResult> CreateNewBlog(CreateBlog createBlog)
        {
            var db = await _appInterface.CreateNewBlog(createBlog);
            var map = _mapper.Map<CreateBlogDTO>(db);
            return Ok(map);
        }

        //return the creator of the post or blog 

        [HttpGet("CreatorGuid")]
        public async Task<ActionResult> GetSpecificUsername(Guid FromGuid)
        { 
            var userName = await _context.NewUser.Where( user => user.UserID  == FromGuid).Select( user => user.UserName).SingleOrDefaultAsync();
            if(userName != null)
            {
                return Ok(userName);
            }
            return BadRequest("User Name Not Found");
        }

   

        //Get Blog's details 

        [HttpGet("MyBlogs")]
        //we used the object to return the instance of the new table items with out the creating a new table 
        public async Task<ActionResult<List<object>>> GetAllBlogs(Guid BlogGuid)
        {
            var exits = await _context.NewBlogs.Where(c => c.BlogCreator == BlogGuid.ToString()).FirstOrDefaultAsync();

           if(exits != null)
           {
                //we are getting a temp obj using table data so that we can push it to the frontend 
                List<CreateBlog> deleteBlog = await _context.NewBlogs.Where( c => c.BlogCreator == BlogGuid.ToString())
                    .Select(c => new CreateBlog
                    {
                        BlogName = c.BlogName,
                        BlogSubTitle = c.BlogSubTitle
                    })
                    .ToListAsync();

                return Ok(deleteBlog);
           }
            return BadRequest("Data not found");
        }

        //registering new user 
        [HttpPost("RegisterUser")]
        public async Task<ActionResult<List<RegisterUser>>> Register( RegisterUser registerUser)
        {
            var db = await _context.RegisterUser.AddAsync(registerUser);
            await _context.SaveChangesAsync();
            if(db.Entity != null)
            {
                return Ok(db.Entity);
            }
            return BadRequest("Can't upload data");
        }

        //get registered users
        [HttpGet("UserProfile")]
        public async Task<ActionResult<RegisterUser>> GetUserPrfoiel ( Guid guid )
        {
            var db = await _context.RegisterUser.
                Where(c => c.UserGuid == guid).
                Select(
                c => new RegisterUser 
                { username = c.username, 
                about = c.about,
                email = c.email,
                profileImg = c.profileImg,
                }).FirstOrDefaultAsync();

            if( db != null)
            {
                return Ok(db);
            }
            return BadRequest("Data not found");
        }


        //Blogs Created By the single user's
        [HttpGet("GetSingleUserBlogs")]
        public async Task<ActionResult<List<CreateBlog>>> GetUserBlogs(Guid guid)
        {
            var blog = await _context.NewBlogs.Where(c => c.BlogCreator == guid.ToString()).Select(c => new CreateBlog
            {
                BlogID = c.BlogID,
                BlogName = c.BlogName,
                BlogSubTitle = c.BlogSubTitle,
                BlogDescription = c.BlogDescription,
                BlogImg= c.BlogImg,
            }).ToListAsync();
            if( blog != null)
            {
                return Ok(blog);
            }
            return BadRequest("Blogs not found");
        }
        //getNewly Generated user Guid 

        [HttpGet("GetNewSignUpGuid")]

        public async Task<ActionResult> GetRecentUserGuid()
        {
            var db = await _context.NewUser.OrderByDescending( c => c.DateTime).Select(c => c.UserID).FirstOrDefaultAsync();
            return Ok(db);
        }

        //Delete singleBlog 

        [HttpDelete("DeleteBlog")]
        public async Task<ActionResult> DeleteBlog(Guid blogGuid)
        {
            var blog = await _context.NewBlogs.FirstOrDefaultAsync(b => b.BlogID == blogGuid);

            if (blog == null)
            {
                return NotFound(); // Return "Not Found" when the item is not found
            }

            _context.NewBlogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //get single blog using guid from the db 

        [HttpGet("GetSingleBlog")]

        public async Task<ActionResult> GetSinlgeblog(Guid blogGuid)
        {
            var db = await _context.NewBlogs.Where( c => c.BlogID == blogGuid ).Select(c => c.BlogImg).ToListAsync();
            if(db != null)
            {
                return Ok(db);
            }
            return BadRequest("Data not found");
        }

        //updating blogs using blogGuid 
        [HttpPut("UpdateBlog")]
        public async Task<ActionResult> UpdateSingleBlog (CreateBlog createBlog)
        {
            var searchDb = await _context.NewBlogs.FindAsync(createBlog.BlogID);
            if(searchDb == null) {
                return BadRequest("No such data found");
            }
            // Update only the specified fields
            searchDb.BlogName = createBlog.BlogName;
            searchDb.BlogSubTitle = createBlog.BlogSubTitle;
            searchDb.BlogDescription = createBlog.BlogDescription;
            searchDb.BlogCreated = createBlog.BlogCreated;
            searchDb.BlogCreator = createBlog.BlogCreator;
            searchDb.BlogCreatorName = createBlog.BlogCreatorName;
            searchDb.BlogImg = createBlog.BlogImg;
            await _context.SaveChangesAsync();

            return Ok(searchDb);
        }

    }
}
