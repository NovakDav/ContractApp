using ContractsWebApp.DBContext;
using ContractsWebApp.Models;
using ContractsWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ContractsWebApp.Controllers
{
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Client")]
        public IActionResult Index(string search)
        {

            List<User> clients = _context.Users.ToList();

            clients = _context.Users
            .Where(u => u.Role == UserRole.Client)
            .ToList();


            if (!string.IsNullOrEmpty(search))
            {
                clients = clients.Where(client =>
                    client.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return View("~/Views/Client/Index.cshtml", clients);
        }

        [HttpPost]
        [Route("Clients/Delete")]
        public IActionResult Delete(string id)
        {
            var client = _context.Users.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Users.Remove(client);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Clients/ChangeRole")]
        public IActionResult ChangeRole(string id,string role)
        {
            var client = _context.Users.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            if(role == "Advisor") {
                client.Role = UserRole.Advisor;
                _context.SaveChanges();
                return RedirectToAction("Index", "Advisor");

            }
            if(role == "Client") {             
                client.Role = UserRole.Client;
                _context.SaveChanges();
                return RedirectToAction("Index", "Client");
            }
            

            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("Clients/Details/{id}")]
        public IActionResult Details(string id)
        {
            var client = _context.Users.FirstOrDefault(u => u.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            var participantContracts = _context.Participants
                .Where(p => p.ParticipantID == id)
                .Join(_context.Contracts,
                    participant => participant.ContractID,
                    contract => contract.Id,
                    (participant, contract) => contract);

            var managerContracts = _context.Contracts
                .Where(c => c.ManagerId == id);

            var contracts = participantContracts.Concat(managerContracts).ToList();

            var model = new ClientDetailsViewModel
            {
                Client = client,
                Contracts = contracts
            };

            return View("~/Views/Client/Detail.cshtml", model);
        }




        [HttpGet]
        [Route("Client/EditProfile")]
        public IActionResult EditProfile()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
            EditProfileViewModel model = new EditProfileViewModel
            {
                Email = user.Email, 
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View("~/Views/Client/EditProfile.cshtml", model);
        }

        [HttpPost]
        [Route("Client/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel updatedProfile)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

                user.Email = updatedProfile.Email;
                user.LastName=updatedProfile.LastName;
                user.FirstName=updatedProfile.FirstName;
                user.PhoneNumber = updatedProfile.PhoneNumber;

                _context.SaveChanges();

                return View("~/Views/Client/Detail.cshtml", user);
            }

            return View(updatedProfile);
        }

        [HttpGet]
        [Route("Clients/ExportToCsv")]
        public IActionResult ExportToCsv()
        {
            var clients = _context.Users
            .Where(u => u.Role == UserRole.Client)
            .ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("ID,First Name,Last Name,Email,Phone");

            foreach (var client in clients)
            {
                csvBuilder.AppendLine($"{client.Id},{client.FirstName},{client.LastName},{client.Email},{client.PhoneNumber}");
            }

            byte[] csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(csvBytes, "text/csv", "clients.csv");
        }

    }
}
