namespace BW4_BE_Grp2.Models
{
    public class ProdottoDettaglioModel
    {

        public Guid? productId;
        public string? Nome;
        public string? Brand;
        public decimal? Prezzo;
        public string? Descrizione;
        public string? Immagine;
        public string? ImmagineAlt;

        public int Quantita { get; set; } = 1;
    }
}