#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace BankAccount.Models;


public class LoginUser
{
    [Key]
    public int UserId { get; set; }
    [Required]
    [EmailAddress]
    public string EmailLog { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string PasswordLog { get; set; }
   
}