using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bombyx2.GUI._01_Bottom_up
{
    public class ImpactBuilding : GH_Component
    {
        public ImpactBuilding()
          : base("5: Building impact",
                 "Building impact",
                 "Calculates CO2 impact of the building",
                 "Bombyx 2",
                 "1: Bottom-up")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("LCA Factors Element", "LCA Factors (Element)", "List of LCA factors", GH_ParamAccess.list);
            pManager[0].DataMapping = GH_DataMapping.Flatten;
            pManager.AddNumberParameter("Reference study period (years)", "RSP (years)", "Reference study period (years)", GH_ParamAccess.item);
            pManager.AddNumberParameter("NFA (square meters)", "NFA (m\xB2)", "NFA (square meters)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Global warming potential",
                "GWP (kg CO\x2082-eq/m² a)",
                "Global warming potential (kg CO\x2082-eq/m² a). Also includes biogenic carbon storage.", 
                GH_ParamAccess.item);

            pManager.AddNumberParameter("PE Total",
                "PE Total (kWh oil-eq/m² a)",
                "PE Total (kWh oil-eq/m² a)", 
                GH_ParamAccess.item);

            pManager.AddNumberParameter("PE Renewable",
                "PE Renewable (kWh oil-eq/m² a)",
                "PE Renewable (kWh oil-eq/m² a)", 
                GH_ParamAccess.item);

            pManager.AddNumberParameter("PE Non-Renewable",
                "PE Non-Renewable (kWh oil-eq/m² a)",
                "PE Non-Renewable (kWh oil-eq/m² a)", 
                GH_ParamAccess.item);

            pManager.AddNumberParameter("UBP impact",
                "UBP (P/m² a)",
                "UBP (P/m² a)", 
                GH_ParamAccess.item);

            pManager.AddTextParameter("LCA factors (text)", 
                "LCA factors (text)", 
                "Building Properties (text)", 
                GH_ParamAccess.item);

            pManager.AddNumberParameter("LCA factors (values)", 
                "LCA factors (values)", 
                "Building Properties (values)", 
                GH_ParamAccess.item);

            pManager.AddNumberParameter("Biogenic Carbon",
            "Biogenic Carbon Storage (kg CO₂-eq/m² a)",
            "Biogenic Carbon Storage (kg CO₂-eq/m² a)",
            GH_ParamAccess.item);
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
                                   .GroupBy(x => x.Index / (16 + 1)) // +1 for BiogenicCarbon
                                   .Select(x => x.Select(v => v.Value).ToList())
                                   .ToList();

            var results = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m² a)", 0 },
                { "UBP13 Replacements (P/m² a)", 0 },
                { "UBP13 End of Life (P/m² a)", 0 },
                { "PE Total Embodied (kWh oil-eq/m² a)", 0 },
                { "PE Total Replacements (kWh oil-eq/m² a)", 0 },
                { "PE Total End of Life (kWh oil-eq/m² a)", 0 },
                { "PE Renewable Embodied (kWh oil-eq/m² a)", 0 },
                { "PE Renewable Replacements (kWh oil-eq/m² a)", 0 },
                { "PE Renewable End of Life (kWh oil-eq/m² a)", 0 },
                { "PE Non Renewable Embodied (kWh oil-eq/m² a)", 0 },
                { "PE Non Renewable Replacements (kWh oil-eq/m² a)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq/m² a)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m² a)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m² a)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m² a)", 0 },
                //{ "U value", 0 }
                { "Biogenic Carbon Storage (kg CO₂-eq/m² a)", 0 }
            };

            foreach (var item in valueSets)
            {
                results["UBP13 Embodied (P/m² a)"] += item[0];
                results["UBP13 Replacements (P/m² a)"] += item[1];
                results["UBP13 End of Life (P/m² a)"] += item[2];
                results["PE Total Embodied (kWh oil-eq/m² a)"] += item[3];
                results["PE Total Replacements (kWh oil-eq/m² a)"] += item[4];
                results["PE Total End of Life (kWh oil-eq/m² a)"] += item[5];
                results["PE Renewable Embodied (kWh oil-eq/m² a)"] += item[6];
                results["PE Renewable Replacements (kWh oil-eq/m² a)"] += item[7];
                results["PE Renewable End of Life (kWh oil-eq/m² a)"] += item[8];
                results["PE Non Renewable Embodied (kWh oil-eq/m² a)"] += item[9];
                results["PE Non Renewable Replacements (kWh oil-eq/m² a)"] += item[10];
                results["PE Non Renewable End of Life (kWh oil-eq/m² a)"] += item[11];
                results["Green House Gasses Embodied (kg CO\x2082-eq/m² a)"] += item[12];
                results["Green House Gasses Replacements (kg CO\x2082-eq/m² a)"] += item[13];
                results["Green House Gasses End of Life (kg CO\x2082-eq/m² a)"] += item[14];
                //results["U value"] += item[15];
                results["Biogenic Carbon Storage (kg CO₂-eq/m² a)"] += item[16];
            }

            results["UBP13 Embodied (P/m² a)"] = Math.Round(results["UBP13 Embodied (P/m² a)"] / rspNFA, 2);
            results["UBP13 Replacements (P/m² a)"] = Math.Round(results["UBP13 Replacements (P/m² a)"] / rspNFA, 2);
            results["UBP13 End of Life (P/m² a)"] = Math.Round(results["UBP13 End of Life (P/m² a)"] / rspNFA, 2);
            results["PE Total Embodied (kWh oil-eq/m² a)"] = Math.Round(results["PE Total Embodied (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Total Replacements (kWh oil-eq/m² a)"] = Math.Round(results["PE Total Replacements (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Total End of Life (kWh oil-eq/m² a)"] = Math.Round(results["PE Total End of Life (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Renewable Embodied (kWh oil-eq/m² a)"] = Math.Round(results["PE Renewable Embodied (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Renewable Replacements (kWh oil-eq/m² a)"] = Math.Round(results["PE Renewable Replacements (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Renewable End of Life (kWh oil-eq/m² a)"] = Math.Round(results["PE Renewable End of Life (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Non Renewable Embodied (kWh oil-eq/m² a)"] = Math.Round(results["PE Non Renewable Embodied (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Non Renewable Replacements (kWh oil-eq/m² a)"] = Math.Round(results["PE Non Renewable Replacements (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["PE Non Renewable End of Life (kWh oil-eq/m² a)"] = Math.Round(results["PE Non Renewable End of Life (kWh oil-eq/m² a)"] / rspNFA, 2);
            results["Green House Gasses Embodied (kg CO\x2082-eq/m² a)"] = Math.Round(results["Green House Gasses Embodied (kg CO\x2082-eq/m² a)"] / rspNFA, 2);
            results["Green House Gasses Replacements (kg CO\x2082-eq/m² a)"] = Math.Round(results["Green House Gasses Replacements (kg CO\x2082-eq/m² a)"] / rspNFA, 2);
            results["Green House Gasses End of Life (kg CO\x2082-eq/m² a)"] = Math.Round(results["Green House Gasses End of Life (kg CO\x2082-eq/m² a)"] / rspNFA, 2);
            //results["U value"] = Math.Round(results["U value"], 4);
            results["Biogenic Carbon Storage (kg CO₂-eq/m² a)"] = Math.Round(results["Biogenic Carbon Storage (kg CO₂-eq/m² a)"] / rspNFA, 2);

            var gwp = Math.Round((results["Green House Gasses Embodied (kg CO\x2082-eq/m² a)"] +
                                  results["Green House Gasses Replacements (kg CO\x2082-eq/m² a)"] +
                                  results["Green House Gasses End of Life (kg CO\x2082-eq/m² a)"]), 4);
            var total = Math.Round((results["PE Total Embodied (kWh oil-eq/m² a)"] +
                                    results["PE Total Replacements (kWh oil-eq/m² a)"] +
                                    results["PE Total End of Life (kWh oil-eq/m² a)"]), 4);
            var ubp = Math.Round((results["UBP13 Embodied (P/m² a)"] +
                                  results["UBP13 Replacements (P/m² a)"] +
                                  results["UBP13 End of Life (P/m² a)"]), 4);
            var renew = Math.Round((results["PE Renewable Embodied (kWh oil-eq/m² a)"] +
                                    results["PE Renewable Replacements (kWh oil-eq/m² a)"] +
                                    results["PE Renewable End of Life (kWh oil-eq/m² a)"]), 4);
            var nonrenew = Math.Round((results["PE Non Renewable Embodied (kWh oil-eq/m² a)"] +
                                       results["PE Non Renewable Replacements (kWh oil-eq/m² a)"] +
                                       results["PE Non Renewable End of Life (kWh oil-eq/m² a)"]), 4);
            var biocarbon = Math.Round(results["Biogenic Carbon Storage (kg CO₂-eq/m² a)"], 2);

            DA.SetData(0, gwp);
            DA.SetData(1, total);
            DA.SetData(2, renew);
            DA.SetData(3, nonrenew);
            DA.SetData(4, ubp);

            var resultValues = results.Values.ToList();

            DA.SetDataList(5, results);
            DA.SetDataList(6, resultValues);

            DA.SetData(7, biocarbon);
        }

        protected override System.Drawing.Bitmap Icon => Icons.impactBuilding;

        public override Guid ComponentGuid => new Guid("ff1cec02-0734-49ee-8aae-d345403e9592"); 
    }
}