namespace Bombyx2.Data.Models
{
    public class BtkComponentModel
    {
        public int Id { get; set; }
        public string ComponentCode { get; set; }
        public string SortCode { get; set; }
        public string CategoryEnglish { get; set; }
        public string CategoryGerman { get; set; }
        public string CategoryFrench { get; set; }
        public string CategoryTextEnglish { get; set; }
        public string CategoryTextGerman { get; set; }
        public string CategoryTextFrench { get; set; }
        public string ConstructionUnit { get; set; }
        public string ShortDescriptionEnglish { get; set; }
        public string ShortDescriptionGerman { get; set; }
        public string ShortDescriptionFrench { get; set; }
        public string PictureURL { get; set; }
        public int RSL { get; set; }
        public double LifeCyclePerYear { get; set; }
        public double Ufactor { get; set; }
        public double Cost { get; set; }
        public string CostUnit { get; set; }
        public int BuildingSizeSmall { get; set; }
        public int BuildingSizeMid { get; set; }
        public int BuildingSizeHighrise { get; set; }
        public int BuildingUsageSF { get; set; }
        public int BuildingUsageMF { get; set; }
        public int BuildingUsageOffice { get; set; }
        public int BuildingEnergyStandard { get; set; }
        public int BuildingEnergyAboveAvg { get; set; }
        public int BuildingEnergyPassivehouse { get; set; }
        public int StructMaterialConcrete { get; set; }
        public int StructMaterialWood { get; set; }
        public int StructMaterialBrick { get; set; }
        public int StructMaterialSteel { get; set; }
    }
}
