using System.ComponentModel.DataAnnotations;

namespace BankAccount.Models;


public class Transaction
{
    [Key]
    public int TransactionId { get; set; }
    [Required]
    public double Amount { get; set; }

    public DateTime createdAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }

    public User? Creator { get; set; }
}