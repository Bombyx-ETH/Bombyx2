using System;
using System.Collections.Generic;
using System.Linq;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._01_ComponentLevel
{
    public class BtkComponent : GH_Component
    {
        public BtkComponent()
          : base("2: Component",
                 "Component",
                 "Returns Bauteilkatalog component from database.",
                 "Bombyx 2",
                 "2: Bottom-up-component")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Component", "Component", "Selected component", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference study period", "RSP (years)", "Manual input of RSP (years)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Component properties (text)", "Component properties (text)", "Component properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Component properties (values)", "Component properties (values)", "Component properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string input = null;
            if (!DA.GetData(0, ref input)) { return; }
            var RSP = 0;
            if (!DA.GetData(1, ref RSP)) { return; }

            var splitInput = input.Split(':').ToList();
            var resultsDB = BtkComponentsDataAccess.GetBtkComponent(splitInput[0]);
            //var RSL = int.Parse(splitInput.Last());
            var RSL = 20;

            double repNum = 0;
            double tmp = (RSP / RSL) - 1;
            if (RSL != 0 && RSP != 0)
            {
                repNum = Math.Ceiling(tmp);
            }
            if (repNum < 0)
            {
                repNum = 0;
            }
            if (repNum == 0)
            {
                repNum = 0;
            }

            var sumResults = from row in resultsDB
                             group row by row.SortCode.Trim() into rowSum
                             select new
                             {
                                 SortCodeGrp = rowSum.Key,
                                 UBP13EmbodiedSum = rowSum.Sum(x => x.UBP13Embodied),
                                 UBP13RepSum = rowSum.Sum(x => (x.UBP13Embodied + x.UBP13EoL) * repNum),
                                 UBP13EoLSum = rowSum.Sum(x => x.UBP13EoL),
                                 TotalEmbodiedSum = rowSum.Sum(x => x.TotalEmbodied),
                                 TotalRepSum = rowSum.Sum(x => (x.TotalEmbodied + x.TotalEoL) * repNum),
                                 TotalEoLSum = rowSum.Sum(x => x.TotalEoL),
                                 RenewableEmbodiedSum = rowSum.Sum(x => x.RenewableEmbodied),
                                 RenewableRepSum = rowSum.Sum(x => (x.RenewableEmbodied + x.RenewableEoL) * repNum),
                                 RenewableEoLSum = rowSum.Sum(x => x.RenewableEoL),
                                 NonRenewableEmbodiedSum = rowSum.Sum(x => x.NonRenewableEmbodied),
                                 NonRenewableRepSum = rowSum.Sum(x => (x.NonRenewableEmbodied + x.NonRenewableEoL) * repNum),
                                 NonRenewableEoLSum = rowSum.Sum(x => x.NonRenewableEoL),
                                 GHGEmbodiedSum = rowSum.Sum(x => x.GHGEmbodied),
                                 GHGRepSum = rowSum.Sum(x => (x.GHGEmbodied + x.GHGEoL) * repNum),
                                 GHGEoLEoLSum = rowSum.Sum(x => x.GHGEoL),
                                 ResistanceSum = rowSum.Sum(x => x.Resistance)
                             };

            var results = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", 0 },
                { "UBP13 Replacements (P/m\xB2)", 0 },
                { "UBP13 End of Life (P/m\xB2)", 0 },
                { "Total Embodied (kWh oil-eq)", 0 },
                { "Total Replacements (kWh oil-eq)", 0 },
                { "Total End of Life (kWh oil-eq)", 0 },
                { "Renewable Embodied (kWh oil-eq)", 0 },
                { "Renewable Replacements (kWh oil-eq)", 0 },
                { "Renewable End of Life (kWh oil-eq)", 0 },
                { "Non Renewable Embodied (kWh oil-eq)", 0 },
                { "Non Renewable Replacements (kWh oil-eq)", 0 },
                { "Non Renewable End of Life (kWh oil-eq)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)", 0 },
                { "R value", 0 }
            };

            foreach (var item in sumResults)
            {
                results["UBP13 Embodied (P/m\xB2)"] += item.UBP13EmbodiedSum;
                results["UBP13 Replacements (P/m\xB2)"] += item.UBP13RepSum;
                results["UBP13 End of Life (P/m\xB2)"] += item.UBP13EoLSum;
                results["Total Embodied (kWh oil-eq)"] += item.TotalEmbodiedSum;
                results["Total Replacements (kWh oil-eq)"] += item.TotalRepSum;
                results["Total End of Life (kWh oil-eq)"] += item.TotalEoLSum;
                results["Renewable Embodied (kWh oil-eq)"] += item.RenewableEmbodiedSum;
                results["Renewable Replacements (kWh oil-eq)"] += item.RenewableRepSum;
                results["Renewable End of Life (kWh oil-eq)"] += item.RenewableEoLSum;
                results["Non Renewable Embodied (kWh oil-eq)"] += item.NonRenewableEmbodiedSum;
                results["Non Renewable Replacements (kWh oil-eq)"] += item.NonRenewableRepSum;
                results["Non Renewable End of Life (kWh oil-eq)"] += item.NonRenewableEoLSum;
                results["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)"] += item.GHGEmbodiedSum;
                results["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)"] += item.GHGRepSum;
                results["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)"] += item.GHGEoLEoLSum;
                results["R value"] += item.ResistanceSum;
            }

            var resultValues = results.Values.ToList();

            DA.SetDataList(0, results);
            DA.SetDataList(1, resultValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.btkComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9b827ea8-d640-469a-81f3-ed46209156c4"); }
        }
    }
}