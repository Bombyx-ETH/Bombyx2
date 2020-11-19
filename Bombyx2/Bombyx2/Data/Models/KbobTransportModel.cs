namespace Bombyx2.Data.Models
{
    public class KbobTransportModel
    {
        public string IdKbob { get; set; }
        public string NameEnglish { get; set; }
        public string NameGerman { get; set; }
        public string NameFrench { get; set; }
        public string Size { get; set; }
        public string Units { get; set; }
        public double UBPOperation { get; set; }
        public double UBPFahrInfrastr { get; set; }
        public double TotalOperation { get; set; }
        public double TotalFahrInfrastr { get; set; }
        public double REOperation { get; set; }
        public double REFahrInfrastr { get; set; }
        public double NEOperation { get; set; }
        public double NEFahrInfrastr { get; set; }
        public double GHGOperation { get; set; }
        public double GHGFahrInfrastr { get; set; }
    }
}
