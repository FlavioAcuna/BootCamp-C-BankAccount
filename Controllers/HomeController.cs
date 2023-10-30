using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("user/create")]
    public IActionResult RegisterUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("DashBoard");
        }
        else
        {
            return View("Index");
        }
    }
    [HttpPost("validar")]
    public IActionResult ValidaUser(LoginUser userLogin)
    {

        if (ModelState.IsValid)
        {
            User? UserExist = _context.users.FirstOrDefault(u => u.Email == userLogin.EmailLog);
            if (UserExist == null)
            {
                ModelState.AddModelError("Email", "Correo o contraseña invalidos");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(userLogin, UserExist.Password, userLogin.PasswordLog);
            if (result == 0)
            {
                ModelState.AddModelError("Email", "Correo o contraseña invalidos");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", UserExist.UserId);
            return RedirectToAction("DashBoard");
        }
        else
        {
            return View("Index");
        }
    }
    [SessionCheck]
    [HttpGet("accounts/")]
    public IActionResult Dashboard()
    {
        int? UserId = HttpContext.Session.GetInt32("UserId");
        List<Transaction> NewlistTrans = _context.Transactions.Include(t => t.Creator).Where(t => t.UserId == UserId).ToList();
        ViewBag.ListaTrans = NewlistTrans;
        ViewBag.SaldoBanco = _context.Transactions.Include(t => t.Creator).Where(t => t.UserId == UserId).Sum(t => t.Amount);
        ViewBag.NameAccount = _context.users.Where(t => t.UserId == UserId).First().Nombre;
        ViewBag.ApellidoAcc = _context.users.Where(t => t.UserId == UserId).First().Apellido;
        return View("Dashboard");
    }
    [HttpPost("deposit")]
    public IActionResult DepositRetira(Transaction newTrans)
    {
        int? UserId = HttpContext.Session.GetInt32("UserId");
        List<Transaction> NewlistTrans = _context.Transactions.Include(t => t.Creator).Where(t => t.UserId == UserId).ToList();
        ViewBag.ListaTrans = NewlistTrans;
        ViewBag.SaldoBanco = _context.Transactions.Include(t => t.Creator).Where(t => t.UserId == UserId).Sum(t => t.Amount);
        double saldoTotal = _context.Transactions.Include(t => t.Creator).Where(t => t.UserId == UserId).Sum(t => t.Amount);
        ViewBag.NameAccount = _context.users.Where(t => t.UserId == UserId).First().Nombre;
        ViewBag.ApellidoAcc = _context.users.Where(t => t.UserId == UserId).First().Apellido;
        ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
        if (ModelState.IsValid)
        {

            if (newTrans.Amount > 0)
            {
                newTrans.UserId = (int)UserId;
                _context.Add(newTrans);
                _context.SaveChanges();
                ModelState.AddModelError("Amount", "Deposito Exitoso");
                return RedirectToAction("Dashboard", UserId);
            }
            else
            {
                if (newTrans.Amount + saldoTotal < 0)
                {
                    ModelState.AddModelError("Amount", "No se puede Retirar mas que tu saldo actual");
                    return View("Dashboard");
                }
                else
                {
                    newTrans.UserId = (int)UserId;
                    saldoTotal += newTrans.Amount;
                    _context.Add(newTrans);
                    _context.SaveChanges();
                    ModelState.AddModelError("Amount", "Retiro Exitoso");
                    return RedirectToAction("Dashboard", UserId);
                }

            }

        }
        else
        {
            return View("Dashboard");
        }


    }



    [HttpPost("Logout")]
    public IActionResult LogOut()
    {
        HttpContext.Session.SetString("UserId", "");
        HttpContext.Session.Clear();
        return View("index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        //Encontrar la sesion 
        int? UserId = context.HttpContext.Session.GetInt32("UserId");
        if (UserId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}