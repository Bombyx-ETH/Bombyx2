namespace Bombyx2.Data.Models
{
    public class KbobEnergyModel
    {
        public int Id { get; set; }
        public string NameEnglish { get; set; }
        public string NameGerman { get; set; }
        public string NameFrench { get; set; }
        public string IdKbob { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public double UBP { get; set; }
        public double PeTotal { get; set; }
        public double PeRenewable { get; set; }
        public double PeNonRenewable { get; set; }
        public double PePeRenewableAtLocation { get; set; }
        public double GHG { get; set; }
    }
}