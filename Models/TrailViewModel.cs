using System.Collections.Generic;

namespace TatryExplorer.Models
{
    public class TrailViewModel
    {
        public Trail Trail { get; set; }
        public bool IsFavorite { get; set; }
        public int FavoriteCount { get; set; }
    }
}