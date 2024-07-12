using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TatryExplorer.Models
{
    public class Trail
    {
        public int Id { get; set; }

        [Required]
        public string? Nazwa { get; set; }

        [Required]
        public double Długość { get; set; }

        [Required]
        public string? PoziomTrudności { get; set; }

        [Required]
        public string? OpisSzlaku { get; set; }

        public string? UserId { get; set; }

        public IdentityUser? User { get; set; }

        public ICollection<FavoriteTrail>? FavoriteTrails { get; set; }
        public int VisitCount { get; set; } = 0;
    }
}
