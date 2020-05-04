using System;
using System.Collections.Generic;
using System.Linq;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._00_MaterialLevel.KBOB
{
    public class KbobService : GH_Component
    {
        public KbobService()
          : base("KBOB Building Services",
                 "KBOB Services",
                 "Returns selected KBOB building services from database",
                 "Bombyx 2",
                 "KBOB")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Selected service", "Service", "Selected service from materials list", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Service properties (text)", "Service\nproperties (text)", "Material properties (text)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Service properties (values)", "Service\nproperties (values)", "Material properties (values)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string input = null;
            if (!DA.GetData(0, ref input)) { return; }

            var newParam = input.Split(':');
            var service = KbobMaterialsDataAccess.GetKbobService(newParam[0]);
            var output = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2 a)", service.UBP13Embodied },
                { "UBP13 End of Life (P/m\xB2 a)", service.UBP13EoL },
                { "PE Total Embodied (kWh oil-eq)", service.TotalEmbodied },
                { "PE Total End of Life (kWh oil-eq)", service.TotalEoL },
                { "PE Renewable Embodied (kWh oil-eq)", service.RenewableEmbodied },
                { "PE Renewable End of Life (kWh oil-eq)", service.RenewableEoL },
                { "PE Non Renewable Embodied (kWh oil-eq)", service.NonRenewableEmbodied },
                { "PE Non Renewable End of Life (kWh oil-eq)", service.NonRenewableEoL },
                { "Green House Gases Embodied (kg CO\x2082-eq/m\xB2 a)", service.GHGEmbodied },
                { "Green House Gases End of Life (kg CO\x2082-eq/m\xB2 a)", service.GHGEoL },
            };

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.kbobService;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("1c4ca054-d5be-4305-955f-95769eeb9956"); }
        }
    }
}