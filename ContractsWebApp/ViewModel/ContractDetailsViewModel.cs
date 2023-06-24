using ContractsWebApp.Models;

namespace ContractsWebApp.ViewModel
{
    public class ContractDetailsViewModel
    {
        public Contract Contract { get; set; }
        public List<User> Participants { get; set; }
    }
}
