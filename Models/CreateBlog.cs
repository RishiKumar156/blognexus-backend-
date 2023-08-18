using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class CreateBlog
    {
        [Key]
        public Guid BlogID { get; set; }
        public string BlogCreator { get; set; } 
        public string BlogName { get; set; }
        public string BlogSubTitle { get; set; }
        public string BlogDescription { get; set;}
        public string BlogImg { get; set; }
        public string BlogCreatorName { get; set; }
        public DateTime BlogCreated { get; set; } = DateTime.Now;
    }
}
