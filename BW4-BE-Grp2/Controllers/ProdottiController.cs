using BW4_BE_Grp2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BW4_BE_Grp2.Controllers
{
    public class ProdottiController : Controller
    {
        private readonly string _connectionString;
        public ProdottiController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Category(int c, string cat)
        {
            List<Prodotto> prodotti = new List<Prodotto>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Fetch prodotti
                string productQuery = "SELECT P.IdProdotto, P.Nome, P.Prezzo, P.Immagine, P.Brand FROM Prodotti as P INNER JOIN  Categorie as C ON P.IdCategoria = C.IdCategoria  WHERE C.IdCategoria = @IdCategoria";
                using (SqlCommand cmd = new SqlCommand(productQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCategoria", c);
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
            ViewBag.NomeCategoria = cat;
            return View(prodotti);
        }
    }
}
