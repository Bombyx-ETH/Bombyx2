using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bombyx2.GUI._00_Database
{
    public class Material : GH_Component
    {
        public Material()
          : base("2: Material",
                 "Material",
                 "Returns selected KBOB material details from database",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Selected material", "Material", "Selected material from materials list", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Material properties (text)", "Material\nproperties (text)", "Material properties (text)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Material properties (values)", "Material\nproperties (values)", "Material properties (values)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string input = null;
            if (!DA.GetData(0, ref input)) { return; }

            var newParam = input.Split(':');
            var material = KbobMaterialsDataAccess.GetKbobMaterial(newParam[0]);
            var kbobid = material.IdKbob.Split('.');
            var groupsToSkip = new List<string> { "00", "11", "14", "12", "21" };
            bool contains = groupsToSkip.Contains(kbobid[0], StringComparer.OrdinalIgnoreCase);
            if (contains)
            {
                Message = "Bombyx v" + Config.Version;
                Message += "\nLayer not needed.";
            }
            else
            {
                Message = "Bombyx v" + Config.Version;
            }

            var output = new Dictionary<string, double>
            {
                { contains ? "Area-mass, Layer not needed (kg/m2)" : "Density (kg/m3)", material.Density ?? 0d },
                { "UBP13 Embodied (P/" + material.DensityUnit + ")", Math.Round(material.UBP13Embodied, 2) },
                { "UBP13 End of Life (P/" + material.DensityUnit + ")", Math.Round(material.UBP13EoL, 2) },
                { "PE Total Embodied (kWh oil-eq/" + material.DensityUnit + ")", Math.Round(material.TotalEmbodied, 2) },
                { "PE Total End of Life (kWh oil-eq/" + material.DensityUnit + ")", Math.Round(material.TotalEoL, 2) },
                { "PE Renewable Embodied (kWh oil-eq/" + material.DensityUnit + ")", Math.Round(material.RenewableEmbodied, 2) },
                { "PE Renewable End of Life (kWh oil-eq/" + material.DensityUnit + ")", Math.Round(material.RenewableEoL, 2) },
                { "PE Non Renewable Embodied (kWh oil-eq/" + material.DensityUnit + ")", Math.Round(material.NonRenewableEmbodied, 2) },
                { "PE Non Renewable End of Life (kWh oil-eq/" + material.DensityUnit + ")", Math.Round(material.NonRenewableEoL, 2) },
                { "Green House Gases Embodied (kg CO\x2082-eq/" + material.DensityUnit + ")", Math.Round(material.GHGEmbodied, 2) },
                { "Green House Gases End of Life (kg CO\x2082-eq/" + material.DensityUnit + ")", Math.Round(material.GHGEoL, 2) },
                { "Thermal Conductivity (W/m*K)", material.ThermalCond ?? -1d },
                { "Biogenic Carbon Storage (kg CO\x2082-eq/" + material.DensityUnit + ")", Math.Round((material.BiogenicCarbon ?? 0d) * 3.67, 2) }
            };

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
        }

        protected override System.Drawing.Bitmap Icon => Icons.kbobMaterial;
            
        public override Guid ComponentGuid => new Guid("12978b7f-d20d-41d6-8df9-01e7faf19e9a"); 
    }
}