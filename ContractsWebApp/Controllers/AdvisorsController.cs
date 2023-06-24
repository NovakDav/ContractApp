using ContractsWebApp.DBContext;
using ContractsWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ContractsWebApp.Controllers
{
    public class AdvisorsController : Controller
    {
        private readonly AppDbContext _context;

        public AdvisorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Advisor")]
        public IActionResult Index(string search)
        {
            
            List<User> advisors = _context.Users.ToList();

            advisors = _context.Users
            .Where(u => u.Role == UserRole.Advisor)
            .ToList();

            if (!string.IsNullOrEmpty(search))
            {
                advisors = advisors.Where(client =>
                    client.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }


            return View("~/Views/Advisor/Index.cshtml", advisors);
        }

        [HttpGet]
        [Route("Advisors/ExportToCsv")]
        public IActionResult ExportToCsv()
        {
            var clients = _context.Users
            .Where(u => u.Role == UserRole.Advisor)
            .ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("ID,First Name,Last Name,Email,Phone");

            foreach (var client in clients)
            {
                csvBuilder.AppendLine($"{client.Id},{client.FirstName},{client.LastName},{client.Email},{client.PhoneNumber}");
            }

            byte[] csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(csvBytes, "text/csv", "advisors.csv");
        }
    }
}
