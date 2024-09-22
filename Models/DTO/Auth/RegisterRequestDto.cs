using System.ComponentModel.DataAnnotations;

namespace social_oc_api.Models.DTO.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
   
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }


    }
}
