using System;
using System.Collections.Generic;
using System.Linq;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._00_MaterialLevel.KBOB
{
    public class KbobMaterial : GH_Component
    {
        public KbobMaterial()
          : base("KBOB Building Material",
                 "KBOB Material",
                 "Returns selected KBOB material details from database",
                 "Bombyx 2",
                 "KBOB")
        {
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
            var output = new Dictionary<string, double>
            {
                { "Density (kg/m\xB3)", material.Density ?? -1d },
                { "UBP13 Embodied (P/m\xB2)", Math.Round(material.UBP13Embodied, 2) },
                { "UBP13 End of Life (P/m\xB2)", Math.Round(material.UBP13EoL, 2) },
                { "PE Total Embodied (kWh oil-eq)", Math.Round(material.TotalEmbodied, 2) },
                { "PE Total End of Life (kWh oil-eq)", Math.Round(material.TotalEoL, 2) },
                { "PE Renewable Embodied (kWh oil-eq)", Math.Round(material.RenewableEmbodied, 2) },
                { "PE Renewable End of Life (kWh oil-eq)", Math.Round(material.RenewableEoL, 2) },
                { "PE Non Renewable Embodied (kWh oil-eq)", Math.Round(material.NonRenewableEmbodied, 2) },
                { "PE Non Renewable End of Life (kWh oil-eq)", Math.Round(material.NonRenewableEoL, 2) },
                { "Green House Gases Embodied (kg CO\x2082-eq/m\xB2)", Math.Round(material.GHGEmbodied, 2) },
                { "Green House Gases End of Life (kg CO\x2082-eq/m\xB2)", Math.Round(material.GHGEoL, 2) },
                { "Thermal Conductivity (W/m*K)", material.ThermalCond ?? -1d }
            };

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.kbobMaterial;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a9a13be9-4723-40ef-b1bc-f64dbd1f3cab"); }
        }
    }
}