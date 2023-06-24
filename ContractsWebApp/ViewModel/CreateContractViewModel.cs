using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContractsWebApp.Models;

public class CreateContractViewModel
{
    [Required]
    public string ContractNumber { get; set; }

    [Required]
    public string Institution { get; set; }

    [Required]
    public DateTime DateSigned { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }


    public string ParticipantIds { get; set; }
}
