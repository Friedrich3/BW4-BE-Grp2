using System.Diagnostics;
using BW4_BE_Grp2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace BW4_BE_Grp2.Controllers
{
    public class ProdottiInEvidenzaController : Controller
    {
        private readonly string _connectionString;

        public ProdottiInEvidenzaController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult AppleProducts()
        {
            List<Prodotto> prodotti = GetProductsByBrand("Apple");
            return View(prodotti);
        }

        public IActionResult SamsungProducts()
        {
            List<Prodotto> prodotti = GetProductsByBrand("Samsung");
            return View(prodotti);
        }

        private List<Prodotto> GetProductsByBrand(string brand)
        {
            List<Prodotto> prodotti = new List<Prodotto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT IdProdotto, Nome, Prezzo, Immagine, Brand FROM Prodotti WHERE Brand = @Brand ORDER BY Nome ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Brand", brand);
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
            }

            return prodotti;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}