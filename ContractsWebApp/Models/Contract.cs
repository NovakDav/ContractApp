using ContractsWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Contract
{
    public string Id { get; set; }

    [Required]
    public string ContractNumber { get; set; }

    [Required]
    public string Institution { get; set; }

    [Required]
    public string ManagerId { get; set; }
    public User Manager { get; set; }

    [Required]
    public DateTime DateSigned { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    
}
