namespace BW4_BE_Grp2.Models
{
    public class AddViewModel
    {
        public Guid IdProdotto { get; set; }
        public string? Nome { get; set; }
        public string? Brand { get; set; }
        public decimal Prezzo { get; set; }
        public string? Descrizione { get; set; }
        public string? Immagine { get; set; }
        public string? ImmagineAlt { get; set; }
        public int IdCategoria { get; set; }

        public List<Categoria>? ListaCategorie { get; set; }

    }
}
