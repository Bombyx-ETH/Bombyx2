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
          : base("KBOB Material",
                 "KBOB Material",
                 "Returns KBOB material details from database",
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
            var output = new Dictionary<string, double>();

            output.Add("Density (kg/m\xB3)", material.Density ?? -1);
            output.Add("UBP13 Embodied (P/m\xB2 a)", material.UBP13Embodied);
            output.Add("UBP13 End of Life (P/m\xB2 a)", material.UBP13EoL);
            output.Add("Total Embodied (kWh oil-eq)", material.TotalEmbodied);
            output.Add("Total End of Life (kWh oil-eq)", material.TotalEoL);
            output.Add("Renewable Embodied (kWh oil-eq)", material.RenewableEmbodied);
            output.Add("Renewable End of Life (kWh oil-eq)", material.RenewableEoL);
            output.Add("Non Renewable Embodied (kWh oil-eq)", material.NonRenewableEmbodied);
            output.Add("Non Renewable End of Life (kWh oil-eq)", material.NonRenewableEoL);
            output.Add("Green House Gases Embodied (kg CO\x2082-eq/m\xB2 a)", material.GHGEmbodied);
            output.Add("Green House Gases End of Life (kg CO\x2082-eq/m\xB2 a)", material.GHGEoL);
            output.Add("Thermal Conductivity (W/m*K)", material.ThermalCond ?? -1);

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a9a13be9-4723-40ef-b1bc-f64dbd1f3cab"); }
        }
    }
}