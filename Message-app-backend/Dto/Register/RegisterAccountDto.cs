using Message_app_backend.Shared;

namespace Message_app_backend.Dto.Register
{
    public class RegisterAccountDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GenderEnum Gender { get; set; }
        public string Avatar { get; set; }
        public string Otp { get; set; }
    }
}
