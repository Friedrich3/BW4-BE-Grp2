using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BW4_BE_Grp2.Controllers
{
    public class CarrelloItem
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public decimal Prezzo { get; set; }
        public int Quantita { get; set; }
        public string? ImmagineUrl { get; set; }
    }

    public class CarrelloController : Controller
    {
        private static List<CarrelloItem> carrello = new List<CarrelloItem>(){
                new CarrelloItem { Id = 1, Nome = "Prodotto 1", Prezzo = 10.99m, Quantita = 1, ImmagineUrl = "m.media-amazon.com/images/I/61-SKW5hv2L.jpg" },
                new CarrelloItem { Id = 2, Nome = "Prodotto 2", Prezzo = 20.50m, Quantita = 1, ImmagineUrl = "m.media-amazon.com/images/I/61-SKW5hv2L.jpg" },
                new CarrelloItem { Id = 3, Nome = "Prodotto 3", Prezzo = 15.00m, Quantita = 1, ImmagineUrl = "m.media-amazon.com/images/I/61-SKW5hv2L.jpg" }
            };

        // Finto database
        public static class FintoDatabase
        {
            public static List<CarrelloItem> ProdottiDisponibili = new List<CarrelloItem>
            {
                new CarrelloItem { Id = 1, Nome = "Prodotto 1", Prezzo = 10.99m, Quantita = 1, ImmagineUrl = "m.media-amazon.com/images/I/61-SKW5hv2L.jpg" },
                new CarrelloItem { Id = 2, Nome = "Prodotto 2", Prezzo = 20.50m, Quantita = 1, ImmagineUrl = "m.media-amazon.com/images/I/61-SKW5hv2L.jpg" },
                new CarrelloItem { Id = 3, Nome = "Prodotto 3", Prezzo = 15.00m, Quantita = 1, ImmagineUrl = "m.media-amazon.com/images/I/61-SKW5hv2L.jpg" }
            };
        }

        public IActionResult Index()
        {
            var totale = carrello.Sum(item => item.Prezzo * item.Quantita);
            ViewBag.Totale = totale;
            return View(carrello);
        }

        [HttpPost]
        public IActionResult Aggiungi(int id)
        {
            var prodotto = FintoDatabase.ProdottiDisponibili.FirstOrDefault(p => p.Id == id);

            if (prodotto != null)
            {
                var itemCarrello = carrello.FirstOrDefault(i => i.Id == id);

                if (itemCarrello != null)
                {
                    itemCarrello.Quantita++;
                }
                else
                {
                    carrello.Add(new CarrelloItem
                    {
                        Id = prodotto.Id,
                        Nome = prodotto.Nome,
                        Prezzo = prodotto.Prezzo,
                        ImmagineUrl = prodotto.ImmagineUrl,
                        Quantita = 1
                    });
                }
            }

            return RedirectToAction("Index");
        }
    }
}
