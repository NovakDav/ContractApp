using System.ComponentModel.DataAnnotations;

namespace ContractsWebApp.ViewModel
{
    public class EditProfileViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }


}
