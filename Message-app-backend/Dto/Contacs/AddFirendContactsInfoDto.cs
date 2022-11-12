using Message_app_backend.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Message_app_backend.Dto.Contacs
{
    public class AddFirendContactsInfoDto
    {
        public int UserId { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GenderEnum Gender { get; set; }
    }
}
