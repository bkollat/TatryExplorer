using System.Collections.Generic;

namespace TatryExplorer.ViewModels
{
    public class AdminViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}