namespace BW4_BE_Grp2.Models
{
    public class OrderViewModel
    {
        public Guid IdProdotto { get; set; }
        public decimal PrezzoUnita { get; set; }
        public int Quantita { get; set; }
        public Guid IdCarrello { get; set; }

    }
}
