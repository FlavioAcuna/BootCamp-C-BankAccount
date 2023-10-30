using System.ComponentModel.DataAnnotations;

namespace BankAccount.Models;
public class EmailUnico : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("El correo es requerido");
        }
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        if (_context.users.Any(e => e.Email == value.ToString()))
        {
            Console.WriteLine("Error en la validacion correo");
            return new ValidationResult("El correo debe ser unico");
        }
        else
        {
            Console.WriteLine("Correo valido");
            return ValidationResult.Success;
        }
    }
}