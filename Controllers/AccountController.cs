using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Marketplace_LabWebBD.Data;
using Marketplace_LabWebBD.Models;
using Marketplace_LabWebBD.ViewModels;
using Marketplace_LabWebBD.Services;

namespace Marketplace_LabWebBD.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Contacto = model.Contacto,
                    Morada = model.Morada,
                    Data_Registo = DateOnly.FromDateTime(DateTime.Now),
                    Email_Validado = false,
                    Status = "Ativo",
                    Bloqueado = false
                };

                // Se o utilizador selecionou Administrador, guardar como pendente
                if (model.Role == "Administrador")
                {
                    user.RolePendente = "Administrador";
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Apenas atribuir role se NÃO for Administrador (que precisa aprovação)
                    if (model.Role != "Administrador")
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    // Fazer login automático se não for admin pendente
                    if (model.Role != "Administrador")
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", GetControllerByRole(model.Role));
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Registo efetuado com sucesso! O seu pedido para ser Administrador está pendente de aprovação.";
                        return RedirectToAction("Login");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/RegisterBuyer
        [HttpGet]
        public IActionResult RegisterBuyer()
        {
            return View();
        }

        // POST: /Account/RegisterBuyer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterBuyer(RegisterCompradorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Nome = model.Nome,
                    Contacto = model.Contacto,
                    Morada = $"{model.Morada}, {model.Codigo_Postal}, {model.Distrito}",
                    Data_Registo = DateOnly.FromDateTime(DateTime.Now),
                    Email_Validado = false,
                    Status = "Ativo",
                    Bloqueado = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Atribuir role Comprador
                    await _userManager.AddToRoleAsync(user, "Comprador");

                    // Criar perfil Comprador
                    var comprador = new Comprador
                    {
                        ID_Utilizador = user.Id,
                        Notificacoes_Email = model.NotificacoesEmail,
                        Notificacoes_Push = model.NotificacoesPush
                    };
                    _context.Compradors.Add(comprador);
                    await _context.SaveChangesAsync();

                    // Gerar token de confirmação de email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: HttpContext.Request.Scheme);

                    // Enviar email de confirmação
                    await _emailService.SendEmailConfirmationAsync(
                        user.Email,
                        user.Nome,
                        confirmationLink!);

                    TempData["SuccessMessage"] = "Registo efetuado com sucesso! Verifique o seu email para confirmar a conta.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            if (userId == 0 || token == null)
            {
                TempData["ErrorMessage"] = "Link de confirmação inválido.";
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "Utilizador não encontrado.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.Email_Validado = true;
                await _userManager.UpdateAsync(user);

                // Determinar o tipo de utilizador para o email de boas-vindas
                var roles = await _userManager.GetRolesAsync(user);
                string userType = "Utilizador";

                if (roles.Any())
                {
                    userType = roles.First();
                }
                else if (user.VendorApprovalStatus == "Pending")
                {
                    userType = "Vendedor";
                }

                // Enviar email de boas-vindas
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Nome, userType);

                // Mensagem personalizada baseada no tipo
                if (user.VendorApprovalStatus == "Pending")
                {
                    TempData["SuccessMessage"] = "Email confirmado com sucesso! Aguarde a aprovação de um administrador para começar a vender.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Email confirmado com sucesso! Pode agora fazer login.";
                }

                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Erro ao confirmar email. O link pode ter expirado.";
            return RedirectToAction("Login");
        }

        // GET: /Account/ResendConfirmationEmail
        [HttpGet]
        public IActionResult ResendConfirmationEmail()
        {
            return View();
        }

        // POST: /Account/ResendConfirmationEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
            {
                // Não revelar se o utilizador existe ou já está confirmado (segurança)
                TempData["SuccessMessage"] = "Se o email existir e não estiver confirmado, receberá um novo email de confirmação.";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, token = token },
                protocol: HttpContext.Request.Scheme);

            await _emailService.SendEmailConfirmationAsync(user.Email, user.Nome, confirmationLink!);

            TempData["SuccessMessage"] = "Email de confirmação reenviado. Verifique a sua caixa de entrada.";
            return RedirectToAction("Login");
        }

        // GET: /Account/RegisterSeller
        [HttpGet]
        public IActionResult RegisterSeller()
        {
            return View();
        }

        // POST: /Account/RegisterSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSeller(RegisterVendedorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Nome = model.Nome,
                    Contacto = model.Contacto,
                    Morada = $"{model.Morada}, {model.Codigo_Postal}, {model.Distrito}",
                    Data_Registo = DateOnly.FromDateTime(DateTime.Now),
                    Email_Validado = false,
                    Status = "Ativo",
                    Bloqueado = false,
                    VendorApprovalStatus = "Pending" // Aguarda aprovação
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // NÃO atribuir role Vendedor ainda - precisa de aprovação
                    // NÃO criar entrada na tabela Vendedor ainda

                    // Gerar token de confirmação de email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: HttpContext.Request.Scheme);

                    // Enviar email de confirmação
                    await _emailService.SendEmailConfirmationAsync(
                        user.Email,
                        user.Nome,
                        confirmationLink!);

                    TempData["SuccessMessage"] = "Registo efetuado com sucesso! Verifique o seu email para confirmar a conta. Após confirmação, aguarde a aprovação de um administrador para começar a vender.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Procurar utilizador por email ou username
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Email);
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email ou password incorretos");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    // 1. Verificar se email está confirmado (para TODOS exceto Admin)
                    if (!user.EmailConfirmed && !roles.Contains("Admin"))
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty,
                            "Por favor confirme o seu email antes de fazer login. Verifique a sua caixa de entrada.");
                        ViewBag.ShowResendLink = true;
                        ViewBag.UserEmail = model.Email;
                        return View(model);
                    }

                    // 2. Verificar estado de aprovação para VENDEDORES
                    if (roles.Contains("Vendedor") || user.VendorApprovalStatus == "Pending" || user.VendorApprovalStatus == "Rejected")
                    {
                        if (user.VendorApprovalStatus == "Pending")
                        {
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(string.Empty,
                                "A sua conta de vendedor está pendente de aprovação por um administrador. Será notificado por email quando a aprovação for concluída.");
                            return View(model);
                        }
                        else if (user.VendorApprovalStatus == "Rejected")
                        {
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(string.Empty,
                                $"A sua conta de vendedor foi rejeitada. Motivo: {user.Motivo_Rejeicao_Vendedor ?? "Não especificado"}");
                            return View(model);
                        }
                    }

                    // 3. Verificar se tem role pendente (para Admins)
                    if (!string.IsNullOrEmpty(user.RolePendente))
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "A sua conta está pendente de aprovação.");
                        return View(model);
                    }

                    // 4. Verificar se está bloqueado
                    if (user.Bloqueado == true)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "A sua conta está bloqueada. Motivo: " + (user.Motivo_Bloqueio ?? "Não especificado"));
                        return View(model);
                    }

                    if (roles.Any())
                    {
                        var role = roles.First();

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", GetControllerByRole(role));
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty,
                        "Conta bloqueada temporariamente devido a múltiplas tentativas falhadas. Tente novamente mais tarde.");
                    return View(model);
                }

                if (result.IsNotAllowed)
                {
                    // Email não confirmado ou conta não aprovada
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Por favor confirme o seu email antes de fazer login. Verifique a sua caixa de entrada.");
                        ViewBag.ShowResendLink = true;
                        ViewBag.UserEmail = model.Email;
                        return View(model);
                    }

                    ModelState.AddModelError(string.Empty, "O seu acesso não está autorizado. Entre em contacto com o suporte.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Email ou password incorretos");
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Helper para determinar o controller baseado no role
        private string GetControllerByRole(string role)
        {
            return role switch
            {
                "Comprador" => "Comprador",
                "Vendedor" => "Vendedor",
                "Administrador" => "Admin",
                _ => "Home"
            };
        }
    }
}
