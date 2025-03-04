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
            List<Prodotto> prodotti = new List<Prodotto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT IdProdotto, Nome, Prezzo, Immagine, Brand FROM Prodotti ORDER BY Nome ASC";

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
                }
            }
            return RedirectToAction("Index");
        }


        [HttpGet("Admin/Edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var EditProduct = new EditViewModel();

            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Prodotti WHERE IdProdotto = @IdProdotto; ";
                await using (SqlCommand command = new SqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("IdProdotto", id);
                    await using (SqlDataReader reader = await command.ExecuteReaderAsync()) 
                    {
                        while (await reader.ReadAsync()) 
                        {
                            EditProduct.IdProdotto = reader.GetGuid(0);
                            EditProduct.Nome = reader.GetString(1);
                            EditProduct.Brand = reader.GetString(2);
                            EditProduct.Prezzo = reader.GetDecimal(3);
                            EditProduct.Descrizione = reader.GetString(4);
                            EditProduct.Immagine = reader.GetString(5);
                            EditProduct.ImmagineAlt = reader.GetString(6);
                            EditProduct.IdCategoria = reader.GetInt32(7);
                        }
                    }
                }
            }
            //CONNESSIONE AL DB PER RIPRENDERE LE LISTE
            var CategoryList = new List<Categoria>();
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
                            CategoryList.Add(new Categoria() { IdCategoria = reader.GetInt32(0), NomeCategoria = reader.GetString(1) });
                        }
                    }
                }
            }
            ViewBag.Categorie = CategoryList;
            return View(EditProduct);
        }

        [HttpPost("Admin/EditSave/{id:guid}")]
        public async Task<IActionResult> EditSave(Guid id, EditViewModel editViewModel) 
        {
            await using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Prodotti SET Nome=@nome, Brand=@brand , Prezzo=@prezzo, Descrizione=@descrizione, Immagine=@immagine, ImmagineAlt=@immagineAlt, IdCategoria=@idcategoria WHERE IdProdotto=@idprodotto";
                await using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idprodotto", id);
                    command.Parameters.AddWithValue("@nome", editViewModel.Nome);
                    command.Parameters.AddWithValue("@brand", editViewModel.Brand);
                    command.Parameters.AddWithValue("@prezzo", editViewModel.Prezzo);
                    command.Parameters.AddWithValue("@descrizione", editViewModel.Descrizione);
                    command.Parameters.AddWithValue("@immagine", editViewModel.Immagine);
                    command.Parameters.AddWithValue("@immagineAlt", editViewModel.ImmagineAlt);
                    command.Parameters.AddWithValue("@idcategoria", editViewModel.IdCategoria);

                    int response = await command.ExecuteNonQueryAsync();
                    //TODO Cambiare gestione errore DB
                    if (response == 0)
                    { Console.WriteLine("Errore"); }
                }
            }
            return RedirectToAction("Index");
        }


        [HttpGet("Admin/Delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await using (SqlConnection connection = new SqlConnection(_connectionString)) {
            await connection.OpenAsync();
            var query = "DELETE FROM Prodotti WHERE IdProdotto = @IdProdotto;";
                await using (SqlCommand command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@IdProdotto",id);
                    int risposta = await command.ExecuteNonQueryAsync();

                    //TODO Cambiare gestione errore DB
                    if (risposta == 0) { Console.WriteLine("Errore"); }
                }


            }

            return RedirectToAction("Index");
        }
    }
}
