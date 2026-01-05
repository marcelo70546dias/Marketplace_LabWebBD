using Marketplace_LabWebBD.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                // Verificar se já existem marcas
                if (context.Marcas.Any())
                {
                    Console.WriteLine("Marcas já existem na base de dados. A inicialização de marcas foi ignorada.");
                }
                else
                {
                    // Inserir marcas e modelos
                    InsertMarcasModelos(context);
                }

                // Verificar e inserir combustíveis
                if (!context.Combustivels.Any())
                {
                    InsertCombustiveis(context);
                }
                else
                {
                    Console.WriteLine("Combustíveis já existem na base de dados.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar base de dados: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Detalhes: {ex.InnerException.Message}");
                }
            }
        }

        private static void InsertMarcasModelos(ApplicationDbContext context)
        {

            // Array de marcas e seus modelos
            var marcasComModelos = new Dictionary<string, string[]>
            {
                { "Peugeot", new[] { "208", "2008", "308", "3008", "5008" } },
                { "Renault", new[] { "Clio", "Captur", "Megane", "Austral", "Arkana" } },
                { "Volkswagen", new[] { "Polo", "Golf", "T-Roc", "Tiguan", "ID.3" } },
                { "Mercedes-Benz", new[] { "Classe A", "Classe C", "Classe E", "GLA", "GLC" } },
                { "BMW", new[] { "Série 1", "Série 3", "Série 5", "X1", "X3" } },
                { "Audi", new[] { "A1", "A3", "A4", "Q3", "Q5" } },
                { "Citroën", new[] { "C3", "C4", "C5 Aircross", "Berlingo", "Ami" } },
                { "Dacia", new[] { "Sandero", "Duster", "Jogger", "Spring", "Logan" } },
                { "SEAT", new[] { "Ibiza", "Leon", "Arona", "Ateca", "Tarraco" } },
                { "Opel", new[] { "Corsa", "Astra", "Mokka", "Crossland", "Grandland" } },
                { "Fiat", new[] { "500", "Panda", "Tipo", "500X", "600" } },
                { "Skoda", new[] { "Fabia", "Octavia", "Kamiq", "Karoq", "Kodiaq" } },
                { "Volvo", new[] { "EX30", "XC40", "XC60", "V40", "V60" } },
                { "Toyota", new[] { "Yaris", "Corolla", "C-HR", "RAV4", "Yaris Cross" } },
                { "Hyundai", new[] { "i10", "i20", "i30", "Kauai", "Tucson" } },
                { "Kia", new[] { "Picanto", "Stonic", "Ceed", "Sportage", "EV6" } },
                { "Nissan", new[] { "Micra", "Juke", "Qashqai", "X-Trail", "Leaf" } },
                { "Ford", new[] { "Fiesta", "Focus", "Puma", "Kuga", "Mustang Mach-E" } },
                { "Tesla", new[] { "Model 3", "Model Y", "Model S", "Model X", "Cybertruck" } },
                { "Mazda", new[] { "Mazda2", "Mazda3", "CX-30", "CX-5", "MX-5" } }
            };

            // Inserir marcas e modelos
            try
            {
                foreach (var marcaData in marcasComModelos)
                {
                    var marca = new Marca
                    {
                        Nome = marcaData.Key
                    };

                    context.Marcas.Add(marca);
                    context.SaveChanges(); // Guardar para obter o ID_Marca

                    // Inserir modelos desta marca
                    foreach (var modeloNome in marcaData.Value)
                    {
                        var modelo = new Modelo
                        {
                            Nome = modeloNome,
                            ID_Marca = marca.ID_Marca
                        };

                        context.Modelos.Add(modelo);
                    }

                    context.SaveChanges();
                }

                Console.WriteLine($"✓ {marcasComModelos.Count} marcas e {marcasComModelos.Sum(m => m.Value.Length)} modelos inseridos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Erro ao inserir marcas e modelos: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Detalhes: {ex.InnerException.Message}");
                }
            }
        }

        private static void InsertCombustiveis(ApplicationDbContext context)
        {
            var combustiveis = new[]
            {
                "Gasolina",
                "Gasóleo",
                "Elétrico",
                "Híbrido Gasolina",
                "Híbrido Gasóleo",
                "Híbrido Plug-in",
                "GPL",
                "GNV",
                "Hidrogénio"
            };

            try
            {
                foreach (var tipo in combustiveis)
                {
                    context.Combustivels.Add(new Combustivel { Tipo = tipo });
                }

                context.SaveChanges();
                Console.WriteLine($"✓ {combustiveis.Length} tipos de combustível inseridos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Erro ao inserir combustíveis: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Detalhes: {ex.InnerException.Message}");
                }
            }
        }
    }
}
