using ContractsWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractsWebApp.ViewModel
{
    public class ClientDetailsViewModel
    {
        public User Client { get; set; }
        public List<Contract> Contracts { get; set; }
    }
}
