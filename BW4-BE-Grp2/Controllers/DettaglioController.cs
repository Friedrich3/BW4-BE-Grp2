using BW4_BE_Grp2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BW4_BE_Grp2.Controllers
{
    public class DettaglioController : Controller
    {
        private readonly string? _connectionString;

        public DettaglioController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpGet("Dettaglio/{id:guid}")]
        public async Task<IActionResult> Index(Guid id)
        {
            var prodottoDettagliato = new ProdottoDettaglioModel();

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Prodotti WHERE IdProdotto = @IdProdotto;";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdProdotto", id);

                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            prodottoDettagliato = new ProdottoDettaglioModel()
                            {
                                productId = reader.GetGuid(0),
                                Nome = reader.GetString(1),
                                Brand = reader.GetString(2),
                                Prezzo = reader.GetDecimal(3),
                                Descrizione = reader.GetString(4),
                                Immagine = reader.GetString(5),
                                ImmagineAlt = reader.GetString(6)
                            };
                        }
                    }
                }
            }

            // Se il prodotto non viene trovato, mostra una pagina di errore o reindirizza
            if (prodottoDettagliato.productId == null)
            {
                return NotFound(); // Oppure return RedirectToAction("Error");
            }

            return View(prodottoDettagliato);
        }

        [HttpPost("Dettaglio/AddOrder/{id:guid}/{number:int}")]
        public IActionResult AddOrder(Guid id, int quantita )
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
            return RedirectToAction("Index", new {id = id });
        }
    }
}
