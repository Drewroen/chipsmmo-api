using System.ComponentModel.DataAnnotations;

namespace ChipsMMO.Models.Request
{
    public class CreateAccountRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
