using System;
using System.Collections.Generic;
using System.Linq;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._01_ComponentLevel
{
    public class BtkComponentWindow : GH_Component
    {
        public BtkComponentWindow()
          : base("2.3: Window Component",
                 "Window component",
                 "Returns Bauteilkatalog window component from database.",
                 "Bombyx 2",
                 "2: Bottom-up-component")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Component", "Component", "Selected component", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference study period", "RSP (years)", "Manual input of RSP (years)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference service life", "RSL (years)", "Manual input of RSL (years)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Surface area (m\xB2)", "Surface area (m\xB2)", "Manual input of surface area in meters", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Window properties (text)", "Window properties (text)", "Window properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Window properties (values)", "Window properties (values)", "Window properties (values)", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value (W/K)", "U value (W/K)", "U Value", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string input = null;
            if (!DA.GetData(0, ref input)) { return; }

            var RSP = 0;
            var RSL = 0;
            var area = 0d;
            var repNum = 0d;

            if (!DA.GetData(1, ref RSP)) { return; }
            if (!DA.GetData(2, ref RSL)) { return; }
            if (!DA.GetData(3, ref area)) { return; }

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

            var newParam = input.Split(':');
            var output = BtkComponentsDataAccess.GetBtkWindowComponent(newParam[0]);

            var sumResults = from row in output
                             group row by row.SortCode into rowSum
                             select new
                             {
                                 SortCodeGrp = rowSum.Key,
                                 UBPEmbodiedSum = rowSum.Sum(x => x.UBPEmbodied),
                                 UBPRepSum = rowSum.Sum(x => (x.UBPEmbodied + x.UBPEoL) * repNum),
                                 UBPEoLSum = rowSum.Sum(x => x.UBPEoL),
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
                                 GHGEoLEoLSum = rowSum.Sum(x => x.GHGEoL)
                             };

            var results = new Dictionary<string, double>
            {
                { "UBP Embodied (P/m\xB2)", 0 },
                { "UBP Replacements (P/m\xB2)", 0 },
                { "UBP End of Life (P/m\xB2)", 0 },
                { "PE Total Embodied (kWh oil-eq)", 0 },
                { "PE Total Replacements (kWh oil-eq)", 0 },
                { "PE Total End of Life (kWh oil-eq)", 0 },
                { "PE Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Renewable End of Life (kWh oil-eq)", 0 },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Non Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)", 0 },
                { "U value", 0 },
                { "g value", 0 }
            };

            foreach (var item in sumResults)
            {
                results["UBP Embodied (P/m\xB2)"] += item.UBPEmbodiedSum;
                results["UBP Replacements (P/m\xB2)"] += (item.UBPEmbodiedSum + item.UBPEoLSum) * repNum;
                results["UBP End of Life (P/m\xB2)"] += item.UBPEoLSum;
                results["PE Total Embodied (kWh oil-eq)"] += item.TotalEmbodiedSum;
                results["PE Total Replacements (kWh oil-eq)"] += (item.TotalEmbodiedSum + item.TotalEoLSum) * repNum;
                results["PE Total End of Life (kWh oil-eq)"] += item.TotalEoLSum;
                results["PE Renewable Embodied (kWh oil-eq)"] += item.RenewableEmbodiedSum;
                results["PE Renewable Replacements (kWh oil-eq)"] += (item.RenewableEmbodiedSum + item.RenewableEoLSum) * repNum;
                results["PE Renewable End of Life (kWh oil-eq)"] += item.RenewableEoLSum;
                results["PE Non Renewable Embodied (kWh oil-eq)"] += item.NonRenewableEmbodiedSum;
                results["PE Non Renewable Replacements (kWh oil-eq)"] += (item.NonRenewableEmbodiedSum + item.NonRenewableEoLSum) * repNum;
                results["PE Non Renewable End of Life (kWh oil-eq)"] += item.NonRenewableEoLSum;
                results["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)"] += item.GHGEmbodiedSum;
                results["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)"] += (item.GHGEmbodiedSum + item.GHGEoLEoLSum) * repNum;
                results["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)"] += item.GHGEoLEoLSum;
                results["U value"] = output[0].Uvalue;
                results["g value"] = output[0].Gvalue;
            }

            var resultValues = results.Values.ToList();

            DA.SetDataList(0, results);
            DA.SetDataList(1, resultValues);
            DA.SetData(2, (output[0].Uvalue + output[1].Uvalue) / area);

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.btkWindowComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("24145b54-3674-45a9-a88d-6b919c0add7b"); }
        }
    }
}