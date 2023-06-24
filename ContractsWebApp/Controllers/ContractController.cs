using ContractsWebApp.DBContext;
using ContractsWebApp.Models;
using ContractsWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Text;

namespace ContractsWebApp.Controllers
{
    public class ContractController : Controller
    {
        private readonly AppDbContext _context;

        public ContractController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Contract")]
        public IActionResult Index(string search)
        {
            List<Contract> contracts = _context.Contracts.ToList();
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!HttpContext.User.IsInRole("Admin"))
            {
                var userContracts = _context.Participants
                    .Where(p => p.ParticipantID == userId)
                    .Select(p => p.ContractID)
                    .ToList();

                contracts = contracts
                    .Where(c => c.ManagerId == userId || userContracts.Contains(c.Id))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                contracts = contracts.Where(c =>
                    c.Id.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.ContractNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Institution.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.ManagerId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.DateSigned.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.ValidFrom.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.ValidTo.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return View("~/Views/Contract/Index.cshtml", contracts);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View("~/Views/Contract/Create.cshtml");

        }


        [HttpPost("Contract/Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contract = new Contract
                    {
                        Id = Guid.NewGuid().ToString(),
                        ContractNumber = model.ContractNumber,
                        Institution = model.Institution,
                        DateSigned = model.DateSigned,
                        ValidFrom = model.ValidFrom,
                        ValidTo = model.ValidTo
                    };

                    string managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    contract.ManagerId = managerId;

                    if (!string.IsNullOrEmpty(model.ParticipantIds))
                    {
                        string[] participantIds = model.ParticipantIds.Split(',');

                        _context.Contracts.Add(contract);
                        _context.SaveChanges();

                        foreach (var participantId in participantIds)
                        {
                            var participant = new Participant
                            {
                                ContractID = contract.Id,
                                ParticipantID = participantId
                            };

                            _context.Participants.Add(participant);
                        }

                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Contracts.Add(contract);
                        _context.SaveChanges();
                    }

                    return RedirectToAction("Index", "Contract");
                }
                catch (DbUpdateException ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", "An error occurred while saving the contract: " + errorMessage);
                }
            }

            return View(model);
        }


        [HttpGet]
        [Route("Contracts/Details/{id}")]
        public IActionResult Details(string id)
        {
            var contract = _context.Contracts.FirstOrDefault(c => c.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            var participants = _context.Participants
                .Where(p => p.ContractID == id)
                .Join(_context.Users,
                    participant => participant.ParticipantID,
                    user => user.Id,
                    (participant, user) => user)
                .ToList();

            var viewModel = new ContractDetailsViewModel
            {
                Contract = contract,
                Participants = participants
            };

            return View("~/Views/Contract/Detail.cshtml", viewModel);
        }

        [HttpGet]
        [Route("Contracts/ExportToCsv")]
        public IActionResult ExportToCsv()
        {
            var contracts = _context.Contracts.ToList();
            var participants = _context.Participants.ToList();
            var users = _context.Users.ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Contract ID,Contract Number,Institution,Manager,Date Signed,Valid From,Valid To");

            foreach (var contract in contracts)
            {
                csvBuilder.AppendLine($"{contract.Id},{contract.ContractNumber},{contract.Institution},{contract.ManagerId},{contract.DateSigned},{contract.ValidFrom},{contract.ValidTo}");
            }

            csvBuilder.AppendLine(); 

            csvBuilder.AppendLine("Participant ID,Participant Name,Contract ID");

            foreach (var participant in participants)
            {
                var participantUser = users.FirstOrDefault(u => u.Id == participant.ParticipantID);

                csvBuilder.AppendLine($"{participant.ParticipantID},{participantUser?.FirstName} {participantUser?.LastName},{participant.ContractID}");
            }

            byte[] csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(csvBytes, "text/csv", "contracts_and_participants.csv");
        }



    }
}
