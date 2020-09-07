using System;
using System.Collections.Generic;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._10_BuildingLevel
{
    public class BuildingOutput : GH_Component
    {
        public BuildingOutput()
          : base("Building Output",
                 "Building output",
                 "Results on a building level",
                 "Bombyx 2",
                 "Building level")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element values", "Element\nvalues", "Element values", GH_ParamAccess.list);
            pManager.AddTextParameter("Selected elements", "Selected\nelements", "Selected elements", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("(All) Global warming potential", "(All) GWP (kg CO\x2082-eq/m\xB2 a)", "(All) Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) PE Total", "(All) PE Total (kWh oil-eq a)", "(All) PE Total (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) PE Renewable", "(All) PE Renewable (kWh oil-eq a)", "(All) PE Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) PE Non-Renewable", "(All) PE Non-Renewable (kWh oil-eq a)", "(All) PE Non-Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) UBP impact", "(All) UBP (P/m\xB2 a)", "(All) UBP (P/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("-","----------------------------------------------", "-", GH_ParamAccess.item);
            pManager.AddTextParameter("Minimum values", "Minimum values", "Minimum values", GH_ParamAccess.list);
            pManager.AddTextParameter("Maximum values", "Maximum values", "Maximum values", GH_ParamAccess.list);
            pManager.AddTextParameter("Average values", "Average values", "Average values", GH_ParamAccess.list);
            pManager.AddNumberParameter("-", "----------------------------------------------", "-", GH_ParamAccess.item);
            pManager.AddTextParameter(" (All) LCA factors (text)", "(All) LCA factors (text)", "(All) Building Properties (text)", GH_ParamAccess.list);
            pManager.AddNumberParameter(" (All) LCA factors (values)", "(All) LCA factors (values)", "(All) Building Properties (values)", GH_ParamAccess.list);
            pManager.AddNumberParameter("-", "----------------------------------------------", "-", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) Global warming potential", "(Selected) GWP (kg CO\x2082-eq/m\xB2 a)", "(Selected) Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) PE Total", "(Selected) PE Total (kWh oil-eq a)", "(Selected) PE Total (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) PE Renewable", "(Selected) PE Renewable (kWh oil-eq a)", "(Selected) PE Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) PE Non-Renewable", "(Selected) PE Non-Renewable (kWh oil-eq a)", "(Selected) PE Non-Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) UBP impact", "(Selected) UBP (P/m\xB2 a)", "(Selected) UBP (P/m\xB2 a)", GH_ParamAccess.item);
            //pManager.AddTextParameter(" (Selected) LCA factors (text)", "(Selected) LCA factors (text)", "(Selected) Building Properties (text)", GH_ParamAccess.item);
            //pManager.AddNumberParameter(" (Selected) LCA factors (values)", "(Selected) LCA factors (values)", "(Selected) Building Properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var input = new List<BuildingLevelModel>();
            if (!DA.GetDataList(0, input)) { return; }

            var splited = SplitList(input, 4);
            //summ all
            var ghgSumAll = 0d;
            var peTotalSumAll = 0d;
            var peRenewableSumAll = 0d;
            var peNonRenewableSumAll = 0d;
            var UBPSumAll = 0d;
            //summ min
            var ghgSumMin = 0d;
            var peTotalSumMin = 0d;
            var peRenewableSumMin = 0d;
            var peNonRenewableSumMin = 0d;
            var UBPSumMin = 0d;
            //summ max
            var ghgSumMax = 0d;
            var peTotalSumMax = 0d;
            var peRenewableSumMax = 0d;
            var peNonRenewableSumMax = 0d;
            var UBPSumMax = 0d;
            //summ avg
            var ghgSumAvg = 0d;
            var peTotalSumAvg = 0d;
            var peRenewableSumAvg = 0d;
            var peNonRenewableSumAvg = 0d;
            var UBPSumAvg = 0d;

            foreach (var list in splited)
            {
                ghgSumAll += list[0].GHGEmbodied + list[0].GHGEoL;
                peTotalSumAll += list[0].TotalEmbodied + list[0].TotalEoL;
                peRenewableSumAll += list[0].RenewableEmbodied + list[0].RenewableEoL;
                peNonRenewableSumAll += list[0].NonRenewableEmbodied + list[0].NonRenewableEoL;
                UBPSumAll += list[0].UBP13Embodied + list[0].UBP13EoL;

                ghgSumMin += list[1].GHGEmbodied + list[1].GHGEoL;
                peTotalSumMin += list[1].TotalEmbodied + list[1].TotalEoL;
                peRenewableSumMin += list[1].RenewableEmbodied + list[1].RenewableEoL;
                peNonRenewableSumMin += list[1].NonRenewableEmbodied + list[1].NonRenewableEoL;
                UBPSumMin += list[1].UBP13Embodied + list[1].UBP13EoL;

                ghgSumMax += list[2].GHGEmbodied + list[2].GHGEoL;
                peTotalSumMax += list[2].TotalEmbodied + list[2].TotalEoL;
                peRenewableSumMax += list[2].RenewableEmbodied + list[2].RenewableEoL;
                peNonRenewableSumMax += list[2].NonRenewableEmbodied + list[2].NonRenewableEoL;
                UBPSumMax += list[2].UBP13Embodied + list[2].UBP13EoL;

                ghgSumAvg += list[3].GHGEmbodied + list[3].GHGEoL;
                peTotalSumAvg += list[3].TotalEmbodied + list[3].TotalEoL;
                peRenewableSumAvg += list[3].RenewableEmbodied + list[3].RenewableEoL;
                peNonRenewableSumAvg += list[3].NonRenewableEmbodied + list[3].NonRenewableEoL;
                UBPSumAvg += list[3].UBP13Embodied + list[3].UBP13EoL;
            }

            var resultsMin = new Dictionary<string, double>
            {
                { "(Min) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round(ghgSumMin, 2) },
                { "(Min) PE Total (kWh oil-eq a)", Math.Round(peTotalSumMin, 2) },
                { "(Min) PE Renewable (kWh oil-eq a)", Math.Round(peRenewableSumMin, 2) },
                { "(Min) PE Non-Renewable (kWh oil-eq a)", Math.Round(peNonRenewableSumMin, 2) },
                { "(Min) UBP (P/m\xB2 a)", Math.Round(UBPSumMin, 2) },
            };

            var resultsMax = new Dictionary<string, double>
            {
                { "(Max) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round(ghgSumMax, 2) },
                { "(Max) PE Total (kWh oil-eq a)", Math.Round(peTotalSumMax, 2) },
                { "(Max) PE Renewable (kWh oil-eq a)", Math.Round(peRenewableSumMax, 2) },
                { "(Max) PE Non-Renewable (kWh oil-eq a)", Math.Round(peNonRenewableSumMax, 2) },
                { "(Max) UBP (P/m\xB2 a)", Math.Round(UBPSumMax, 2) },
            };

            var resultsAvg = new Dictionary<string, double>
            {
                { "(Avg) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round(ghgSumAvg, 2) },
                { "(Avg) PE Total (kWh oil-eq a)", Math.Round(peTotalSumAvg, 2) },
                { "(Avg) PE Renewable (kWh oil-eq a)", Math.Round(peRenewableSumAvg, 2) },
                { "(Avg) PE Non-Renewable (kWh oil-eq a)", Math.Round(peNonRenewableSumAvg, 2) },
                { "(Avg) UBP (P/m\xB2 a)", Math.Round(UBPSumAvg, 2) },
            };

            DA.SetData(0, Math.Round(ghgSumAll, 2));
            DA.SetData(1, Math.Round(peTotalSumAll, 2));
            DA.SetData(2, Math.Round(peRenewableSumAll, 2));
            DA.SetData(3, Math.Round(peNonRenewableSumAll, 2));
            DA.SetData(4, Math.Round(UBPSumAll, 2));

            DA.SetDataList(6, resultsMin);
            DA.SetDataList(7, resultsMax);
            DA.SetDataList(8, resultsAvg);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.buildingOutput;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("2c0de49c-1310-40bb-bc01-d03bc6e47e59"); }
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}