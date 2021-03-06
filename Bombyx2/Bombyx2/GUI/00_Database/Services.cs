﻿using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bombyx2.GUI._00_Database
{
    public class Services : GH_Component
    {
        public Services()
          : base("Building Services",
                 "Building Services",
                 "Returns selected KBOB building services from database",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Selected service", "Service", "Selected service from materials list", GH_ParamAccess.item);
            pManager.AddNumberParameter("Energy reference area (square meters)", "ERA (m\xB2)", "energy reference area", GH_ParamAccess.item);
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
            var area = 0d;
            if (!DA.GetData(1, ref area)) { return; }

            var newParam = input.Split(':');
            var service = KbobMaterialsDataAccess.GetKbobService(newParam[0]);
            var output = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2 a)", service.UBP13Embodied * area },
                { "UBP13 Replacements (P/m\xB2)", 0},
                { "UBP13 End of Life (P/m\xB2 a)", service.UBP13EoL * area },
                { "PE Total Embodied (kWh oil-eq)", service.TotalEmbodied * area },
                { "PE Total Replacements (kWh oil-eq)", 0 },
                { "PE Total End of Life (kWh oil-eq)", service.TotalEoL * area },
                { "PE Renewable Embodied (kWh oil-eq)", service.RenewableEmbodied * area },
                { "PE Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Renewable End of Life (kWh oil-eq)", service.RenewableEoL * area },
                { "PE Non Renewable Embodied (kWh oil-eq)", service.NonRenewableEmbodied * area },
                { "PE Non Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq)", service.NonRenewableEoL * area },
                { "Green House Gases Embodied (kg CO\x2082-eq/m\xB2 a)", service.GHGEmbodied * area },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gases End of Life (kg CO\x2082-eq/m\xB2 a)", service.GHGEoL * area },
                { "U value", 0 }
            };

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
        }

        protected override System.Drawing.Bitmap Icon => Icons.kbobService;

        public override Guid ComponentGuid => new Guid("0111948c-cda6-419b-98c9-1c609f42f4a5"); 
    }
}