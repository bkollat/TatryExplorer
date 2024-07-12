namespace TatryExplorer.ViewModels
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<RoleSelectionViewModel> AvailableRoles { get; set; }
    }

    public class RoleSelectionViewModel
    {
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}