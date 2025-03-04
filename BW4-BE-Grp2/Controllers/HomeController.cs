using System.Diagnostics;
using BW4_BE_Grp2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace BW4_BE_Grp2.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult Index()
        {
            List<Categoria> categorie = new List<Categoria>();
            List<Prodotto> prodotti = new List<Prodotto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Fetch categorie
                string categoryQuery = "SELECT IdCategoria, NomeCategoria, ImmagineCategoria FROM Categorie";
                using (SqlCommand cmd = new SqlCommand(categoryQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categorie.Add(new Categoria
                        {
                            IdCategoria = reader.GetInt32(0),
                            NomeCategoria = reader.GetString(1),
                            ImmagineCategoria = reader.GetString(2)
                        });
                    }
                }

                // Fetch prodotti
                string productQuery = "SELECT TOP 10 IdProdotto, Nome, Prezzo, Immagine, Brand FROM Prodotti ORDER BY NEWID()";
                using (SqlCommand cmd = new SqlCommand(productQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prodotti.Add(new Prodotto
                        {
                            IdProdotto = reader.GetGuid(0),
                            Nome = reader.GetString(1),
                            Prezzo = reader.GetDecimal(2),
                            Immagine = reader.GetString(3),
                            Brand = reader.GetString(4)
                        });
                    }
                }
            }

            ViewBag.Prodotti = prodotti;
            return View(categorie);
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
}