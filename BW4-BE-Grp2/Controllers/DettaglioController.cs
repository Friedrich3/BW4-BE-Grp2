using BW4_BE_Grp2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BW4_BE_Grp2.Controllers
{
    public class DettaglioController : Controller
    {

        //campo privato readonly che può essere valorizzato solo all'interno del costruttore.
        private readonly string? _connectionString;

        public DettaglioController()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {


            var productsList = new ProdottoDettaglioViewModel()
            {
                ProdottoDettagliato = new List<ProdottoDettaglioModel>()
            };
            
             
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Prodotti WHERE IdProdotto = '1BA8513E-4B83-4D45-856E-11E6A5D99F0A';";

                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            productsList.ProdottoDettagliato.Add(
                              new ProdottoDettaglioModel()
                              {
                                  productId = reader.GetGuid(0),
                                  Nome = reader.GetString(1),
                                  Brand = reader.GetString(2),
                                  Prezzo = reader.GetDecimal(3),
                                  Descrizione = reader.GetString(4),
                                  Immagine = reader.GetString(5),
                                  ImmagineAlt = reader.GetString(6)
                              });
                         
                        }
                    }
                }
            }

            /*if (productsList.ProdottoDettaglio.HasValue)
            {
                // return Content($"ID Prodotto trovato: {productId.Value}");
                ViewBag.ProductId = productId.Value.ToString();
                ViewBag.Nome = this.ToString();
                ViewBag.Brand = this.ToString();
                ViewBag.Prezzo = this.ToString();
                ViewBag.Descrizione = this.ToString();
                ViewBag.Immagine = this.ToString();
                ViewBag.ImmagineAlt = this.ToString();
                return View(productId.Value);
            }
            else
            {
                ViewBag.ProductId = "Nessun prodotto trovato.";
                return Content(ViewBag.ProductId);

            }*/

            return View(productsList);

        }
    }
}
