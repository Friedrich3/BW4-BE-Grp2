using System.ComponentModel.DataAnnotations;

namespace BW4_BE_Grp2.Models
{
    public class Prodotto
    {
        [Key]
        public Guid IdProdotto { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public decimal Prezzo { get; set; }

        [Required]
        public string Immagine { get; set; }

        [Required]
        public string Brand { get; set; }
    }
}
