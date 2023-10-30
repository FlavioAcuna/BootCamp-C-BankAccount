#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Models;


public class User
{
    [Key]
    public int UserId { get; set; }
    [Required]
    [MinLength(2, ErrorMessage = "El nombre de tener al menos 2 caracteres")]
    public string Nombre { get; set; }
    [Required]
    [MinLength(2, ErrorMessage = "El apellido de tener al menos 2 caracteres")]
    public string Apellido { get; set; }
    [Required]
    [EmailAddress]
    [EmailUnico]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "La contraseña de contener al menos 8 caracteres")]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [NotMapped]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public List<Transaction> CreatedTransaction { get; set; } = new List<Transaction>();
}