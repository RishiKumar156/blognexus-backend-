using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegisterUser
    {
        [Key]
        public Guid UserGuid { get; set; }
        public string username { get; set; }
        public string about { get; set; }
        public string email { get; set; }
        public string profileImg { get; set; }
    }
}
