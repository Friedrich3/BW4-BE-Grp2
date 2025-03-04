using BW4_BE_Grp2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BW4_BE_Grp2.Controllers
{


    public class AdminController : Controller
    {
        private readonly string _connectionString;
    
        public AdminController()
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


        public async Task<IActionResult> Add() 
        {
            var modello = new AddViewModel()
            {
                ListaCategorie = new List<Categoria>()
            };

            await using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                await connection.OpenAsync();
                var query = "SELECT IdCategoria, NomeCategoria FROM Categorie";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            modello.ListaCategorie.Add(new Categoria() { IdCategoria = reader.GetInt32(0), NomeCategoria = reader.GetString(1) });
                        }
                    }
                }
            }
                return View(modello);
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(AddViewModel addViewModel)
        {
            if (!ModelState.IsValid) { return RedirectToAction("Index"); }

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Prodotti VALUES (@IdProdotto, @Nome, @Brand, @Prezzo, @Descrizione, @Immagine, @ImmagineAlt, @IdCategoria) ";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdProdotto",Guid.NewGuid());
                    command.Parameters.AddWithValue("@Nome", addViewModel.Nome);
                    command.Parameters.AddWithValue("@Brand", addViewModel.Brand);
                    command.Parameters.AddWithValue("@Prezzo", addViewModel.Prezzo);
                    command.Parameters.AddWithValue("@Descrizione", addViewModel.Descrizione);
                    command.Parameters.AddWithValue("@Immagine", addViewModel.Immagine);
                    command.Parameters.AddWithValue("@ImmagineAlt", addViewModel.ImmagineAlt);
                    command.Parameters.AddWithValue("@IdCategoria", addViewModel.IdCategoria);
                   
                    int righeAggiunte = await command.ExecuteNonQueryAsync();
                    Console.WriteLine(righeAggiunte);
                }
            }
            return RedirectToAction("Index");
        }



        public IActionResult Edit()
        {
            return View();
        }
    }
}
