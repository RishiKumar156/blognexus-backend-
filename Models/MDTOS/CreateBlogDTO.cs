namespace API.Models.MDTOS
{
    public class CreateBlogDTO
    {
        public string BlogCreator { get; set; }
        public string BlogName { get; set; }
        public string BlogSubTitle { get; set; }
        public string BlogDescription { get; set; }
        public string BlogImg { get; set; }
        public DateTime BlogCreated { get; set; } = DateTime.Now;
    }
}
