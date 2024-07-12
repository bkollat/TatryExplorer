using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TatryExplorer.Models
{
    public class FavoriteTrail
    {
        public int Id { get; set; }

        [Required]
        public int TrailId { get; set; }

        [Required]
        public string? UserId { get; set; }

        public Trail? Trail { get; set; }
        public IdentityUser? User { get; set; }
    }
}
