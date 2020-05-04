using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._02_ImpactLevel
{
    public class ImpactBuilding : GH_Component
    {
        public ImpactBuilding()
          : base("Building impact",
                 "Building impact",
                 "Calculates CO2 impact of the building",
                 "Bombyx 2",
                 "Impacts")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("LCA Factors\nElement", "LCA Factors\n(Element)", "List of LCA factors", GH_ParamAccess.list);
            pManager.AddNumberParameter("Reference study period (years)", "RSP (years)", "Reference study period (years)", GH_ParamAccess.item);
            pManager.AddNumberParameter("NFA (square meters)", "NFA (m\xB2)", "NFA (square meters)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Global warming potential", "GWP (kg CO\x2082-eq/m\xB2 a)", "Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("PE Total", "PE Total (kWh oil-eq)", "PE Total (kWh oil-eq)", GH_ParamAccess.item);
            pManager.AddNumberParameter("PE Renewable", "PE Renewable (kWh oil-eq)", "PE Renewable (kWh oil-eq)", GH_ParamAccess.item);
            pManager.AddNumberParameter("PE Non-Renewable", "PE Non-Renewable (kWh oil-eq)", "PE Non-Renewable (kWh oil-eq)", GH_ParamAccess.item);
            pManager.AddNumberParameter("UBP impact", "UBP (P/m\xB2 a)", "UBP (P/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddTextParameter("LCA factors (text)", "LCA factors (text)", "Building Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("LCA factors (values)", "LCA factors (values)", "Building Properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var element = new List<double>();
            if (!DA.GetDataList(0, element)) { return; }
            var RSP = 0d;
            if (!DA.GetData(1, ref RSP)) { return; }
            var NFA = 0d;
            if (!DA.GetData(2, ref NFA)) { return; }

            var rspNFA = RSP * NFA;

            var valueSets = element.Select((x, i) => new { Index = i, Value = x })
                                   .GroupBy(x => x.Index / 16)
                                   .Select(x => x.Select(v => v.Value).ToList())
                                   .ToList();

            var results = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2 a)", 0 },
                { "UBP13 Replacements (P/m\xB2 a)", 0 },
                { "UBP13 End of Life (P/m\xB2 a)", 0 },
                { "PE Total Embodied (kWh oil-eq)", 0 },
                { "PE Total Replacements (kWh oil-eq)", 0 },
                { "PE Total End of Life (kWh oil-eq)", 0 },
                { "PE Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Renewable End of Life (kWh oil-eq)", 0 },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Non Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2 a)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2 a)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2 a)", 0 },
                { "U value", 0 }
            };

            foreach (var item in valueSets)
            {
                results["UBP13 Embodied (P/m\xB2 a)"] += item[0];
                results["UBP13 Replacements (P/m\xB2 a)"] += item[1];
                results["UBP13 End of Life (P/m\xB2 a)"] += item[2];
                results["PE Total Embodied (kWh oil-eq)"] += item[3];
                results["PE Total Replacements (kWh oil-eq)"] += item[4];
                results["PE Total End of Life (kWh oil-eq)"] += item[5];
                results["PE Renewable Embodied (kWh oil-eq)"] += item[6];
                results["PE Renewable Replacements (kWh oil-eq)"] += item[7];
                results["PE Renewable End of Life (kWh oil-eq)"] += item[8];
                results["PE Non Renewable Embodied (kWh oil-eq)"] += item[9];
                results["PE Non Renewable Replacements (kWh oil-eq)"] += item[10];
                results["PE Non Renewable End of Life (kWh oil-eq)"] += item[11];
                results["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2 a)"] += item[12];
                results["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2 a)"] += item[13];
                results["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2 a)"] += item[14];
                results["U value"] += item[15];
            }

            results["UBP13 Embodied (P/m\xB2 a)"] /= rspNFA;
            results["UBP13 Replacements (P/m\xB2 a)"] /= rspNFA;
            results["UBP13 End of Life (P/m\xB2 a)"] /= rspNFA;
            results["PE Total Embodied (kWh oil-eq)"] /= rspNFA;
            results["PE Total Replacements (kWh oil-eq)"] /= rspNFA;
            results["PE Total End of Life (kWh oil-eq)"] /= rspNFA;
            results["PE Renewable Embodied (kWh oil-eq)"] /= rspNFA;
            results["PE Renewable Replacements (kWh oil-eq)"] /= rspNFA;
            results["PE Renewable End of Life (kWh oil-eq)"] /= rspNFA;
            results["PE Non Renewable Embodied (kWh oil-eq)"] /= rspNFA;
            results["PE Non Renewable Replacements (kWh oil-eq)"] /= rspNFA;
            results["PE Non Renewable End of Life (kWh oil-eq)"] /= rspNFA;
            results["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2 a)"] /= rspNFA;
            results["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2 a)"] /= rspNFA;
            results["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2 a)"] /= rspNFA;

            var gwp = Math.Round((results["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2 a)"] +
                                  results["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2 a)"] +
                                  results["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2 a)"]), 4);
            var total = Math.Round((results["PE Total Embodied (kWh oil-eq)"] +
                                    results["PE Total Replacements (kWh oil-eq)"] +
                                    results["PE Total End of Life (kWh oil-eq)"]), 4);
            var ubp = Math.Round((results["UBP13 Embodied (P/m\xB2 a)"] +
                                  results["UBP13 Replacements (P/m\xB2 a)"] +
                                  results["UBP13 End of Life (P/m\xB2 a)"]), 4);
            var renew = Math.Round((results["PE Renewable Embodied (kWh oil-eq)"] +
                                    results["PE Renewable Replacements (kWh oil-eq)"] +
                                    results["PE Renewable End of Life (kWh oil-eq)"]), 4);
            var nonrenew = Math.Round((results["PE Non Renewable Embodied (kWh oil-eq)"] +
                                       results["PE Non Renewable Replacements (kWh oil-eq)"] +
                                       results["PE Non Renewable End of Life (kWh oil-eq)"]), 4);

            DA.SetData(0, gwp);
            DA.SetData(1, total);
            DA.SetData(2, renew);
            DA.SetData(3, nonrenew);
            DA.SetData(4, ubp);

            var resultValues = results.Values.ToList();

            DA.SetDataList(5, results);
            DA.SetDataList(6, resultValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.impactBuilding;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fd32cdf4-f071-4ad2-946e-b26e288f4b2c"); }
        }
    }
}