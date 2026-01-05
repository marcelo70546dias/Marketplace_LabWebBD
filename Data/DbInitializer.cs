using Microsoft.AspNetCore.Identity;
using Marketplace_LabWebBD.Models;

namespace Marketplace_LabWebBD.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Criar roles se não existirem
            string[] roles = { "Comprador", "Vendedor", "Administrador" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            // Criar primeiro administrador se não existir nenhum
            var adminRole = await roleManager.FindByNameAsync("Administrador");
            var adminsInRole = await userManager.GetUsersInRoleAsync("Administrador");

            if (!adminsInRole.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "Marcelo.Admin1",
                    Email = "marsdias31@gmail.com",
                    Nome = "Marcelo Dias",
                    Contacto = "915795787",
                    Morada = "Rua de S. Francisco 14, 4805-070, Braga",
                    Data_Registo = DateOnly.FromDateTime(DateTime.Now),
                    Email_Validado = true,
                    EmailConfirmed = true,
                    Status = "Ativo",
                    Bloqueado = false
                };

                var result = await userManager.CreateAsync(adminUser, "Marcelo.Admin1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                    Console.WriteLine("Primeiro administrador criado com sucesso: marsdias31@gmail.com");
                }
                else
                {
                    Console.WriteLine("Erro ao criar administrador:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
        }
    }
}
