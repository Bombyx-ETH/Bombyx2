namespace Bombyx2.Data.Models
{
    public class BuildingLevelModel
    {
        public string eBKP { get; set; }
        public string ComponentID { get; set; }
        public string ComponentTitle { get; set; }
        public double Uvalue { get; set; }
        public double UBPEmbodied { get; set; }
        public double UBPEoL { get; set; }
        public double TotalEmbodied { get; set; }
        public double TotalEoL { get; set; }
        public double RenewableEmbodied { get; set; }
        public double RenewableEoL { get; set; }
        public double NonRenewableEmbodied { get; set; }
        public double NonRenewableEoL { get; set; }
        public double GHGEmbodied { get; set; }
        public double GHGEoL { get; set; }
    }
}
