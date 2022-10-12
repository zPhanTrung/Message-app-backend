using System.ComponentModel.DataAnnotations;

namespace Message_app_backend.Entities
{
    public class UserInfo
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
