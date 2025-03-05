﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BW4_BE_Grp2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BW4_BE_Grp2.Controllers
{
    public class CarrelloController : Controller
    {
        private readonly string _connectionString;

        public CarrelloController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IActionResult Index()
        {
            List<Ordini> carrello = new List<Ordini>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT O.IdProdotto, O.IdCarrello, O.PrezzoUnita, O.Quantita, P.Nome, P.Immagine FROM Ordini AS O INNER JOIN Prodotti AS P ON O.IdProdotto = P.IdProdotto";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        carrello.Add(new Ordini
                        {
                            IdProdotto = reader.GetGuid(0),
                            IdCarrello = reader.GetGuid(1),
                            PrezzoUnita = reader.GetDecimal(2),
                            Quantita = reader.GetInt32(3),
                            Prodotto = new Prodotto()
                            {
                                Nome = reader.GetString(4),
                                Immagine = reader.GetString(5)
                            }
                        });
                    }
                }
            }

            return View(carrello);
        }

        [HttpPost]
        public IActionResult Rimuovi(Guid idProdotto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Ordini WHERE IdProdotto = @IdProdotto";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdProdotto", idProdotto);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AggiungiAlCarrello(Guid idProdotto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            DECLARE @IdCarrello UNIQUEIDENTIFIER;
            SELECT TOP 1 @IdCarrello = IdCarrello FROM Carrello;

            IF EXISTS (SELECT 1 FROM Ordini WHERE IdProdotto = @IdProdotto AND IdCarrello = @IdCarrello)
                UPDATE Ordini SET Quantita = Quantita + 1 WHERE IdProdotto = @IdProdotto AND IdCarrello = @IdCarrello
            ELSE
                INSERT INTO Ordini (IdProdotto, IdCarrello, PrezzoUnita, Quantita)
                SELECT @IdProdotto, @IdCarrello, Prezzo, 1 FROM Prodotti WHERE IdProdotto = @IdProdotto";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdProdotto", idProdotto);
                    cmd.ExecuteNonQuery();
                }
            }

            // Ottiene URL della pagina precedente
            string referer = Request.Headers["Referer"].ToString();

            // Se il referer è vuoto, reindirizza alla home page
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Home");
            }

            // Altrimenti, reindirizza alla pagina precedente
            return Redirect(referer);
        }

        [HttpGet("Carrello/RemoveQuantity/{item:guid}/{cart:guid}")]
        public IActionResult RemoveQuantity(Guid item, Guid cart)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "UPDATE Ordini SET Quantita = Quantita -1 WHERE (IdProdotto = @idProdotto AND IdCarrello=@idCarrello) AND Quantita > 1";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@idProdotto", item);
                    command.Parameters.AddWithValue("@idCarrello", cart);
                    int risposta = command.ExecuteNonQuery();
                    //TODO Aggiungere controllo in caso di errore
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Carrello/AddQuantity/{item:guid}/{cart:guid}")]
        public IActionResult AddQuantity(Guid item, Guid cart)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = "UPDATE Ordini SET Quantita = Quantita +1 WHERE (IdProdotto = @idProdotto AND IdCarrello=@idCarrello)";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@idProdotto", item);
                    command.Parameters.AddWithValue("@idCarrello", cart);
                    int risposta = command.ExecuteNonQuery();
                    //TODO Aggiungere controllo in caso di errore
                }
            }
            return RedirectToAction("Index");
        }
    }
}
