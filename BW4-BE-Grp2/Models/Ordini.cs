using System.ComponentModel.DataAnnotations;

namespace BW4_BE_Grp2.Models
{
    public class Ordini
    {
        [Key]
        public Guid IdProdotto { get; set; }

        [Required]
        public Guid IdCarrello { get; set; }

        [Required]
        public decimal PrezzoUnita { get; set; }

        [Required]
        public int Quantita { get; set; }

        // Relazione con il modello Prodotto
        public Prodotto? Prodotto { get; set; }
    }
}
