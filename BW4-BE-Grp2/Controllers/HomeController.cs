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

        //AGGIUNTA AL CARRELLO
        [HttpPost("Home/AddOrder/{id:guid}/{number:int}")]
        public IActionResult AggiungiAlCarrello(Guid id, int quantita)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"IF EXISTS (SELECT 1 FROM Ordini WHERE IdProdotto = @IdProdotto AND IdCarrello = (SELECT IdCarrello From Carrello))
                                    UPDATE Ordini Set Quantita = Quantita + @Quantita WHERE IdProdotto = @IdProdotto AND IdCarrello = (SELECT IdCarrello FROM Carrello);  
                              ELSE
                                    INSERT INTO Ordini VALUES (@IdProdotto, (SELECT IdCarrello From Carrello),(SELECT Prezzo FROM Prodotti WHERE IdProdotto = @IdProdotto), @Quantita);";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdProdotto", id);
                    command.Parameters.AddWithValue("@Quantita", quantita);
                    int risposta = command.ExecuteNonQuery();
                }
            }
            //TODO AGGIUNGERE CONTROLLO NELLA VISTA DI SUCCESSO IN CASO DI INSERIMENTO
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}