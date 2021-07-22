using System.ComponentModel.DataAnnotations;

namespace ChipsMMO.Models.Request
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
