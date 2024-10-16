﻿namespace Bombyx2.Data.Models
{
    public class KbobMaterialModel
    {
        public int Id { get; set; }
        public string NameEnglish { get; set; }
        public string NameGerman { get; set; }
        public string NameFrench { get; set; }
        public string IdKbob { get; set; }
        public string IdDisposal { get; set; }
        public string Disposal { get; set; }
        public double? Density { get; set; }
        public string DensityUnit { get; set; }
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
        public double? ThermalCond { get; set; }
        public double? BiogenicCarbon { get; set; }
    }
}
