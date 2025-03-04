using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using BW4_BE_Grp2.Models;

namespace BW4_BE_Grp2.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _connectionString;

        public AdminController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<Prodotto> prodotti = new List<Prodotto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT IdProdotto, Nome, Prezzo, Immagine, Brand FROM Prodotti";

                using (SqlCommand cmd = new SqlCommand(query, conn))
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

            return View(prodotti);
        }
    }
}
