﻿using System;
using System.Collections.Generic;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;
using Bombyx2.Data.Access;
using System.Linq;

namespace Bombyx2.GUI._10_BuildingLevel
{
    public class BuildingOutput : GH_Component
    {
        public BuildingOutput()
          : base("3.3: Building Output",
                 "Building output",
                 "Results on a building level",
                 "Bombyx 2",
                 "3: Top-down")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element values", "Element values", "Element values", GH_ParamAccess.list);
            pManager.AddTextParameter("Selected components", "Selected components", "Selected components", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("Remaining components", "Remaining components", "Remaining components", GH_ParamAccess.list);
            pManager[2].Optional = true;
            pManager[2].DataMapping = GH_DataMapping.Flatten;
            pManager.AddNumberParameter("Reference study period (years)", "RSP (years)", "Reference study period (years)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Net floor area (square meters)", "NFA (m\xB2)", "Net floor area (square meters)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //pManager.AddNumberParameter("(Avg) Global warming potential", "(Avg) GWP (kg CO\x2082-eq/m\xB2 a)", "(Avg) Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Avg) PE Total", "(Avg) PE Total (kWh oil-eq a)", "(Avg) PE Total (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Avg) PE Renewable", "(Avg) PE Renewable (kWh oil-eq a)", "(Avg) PE Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Avg) PE Non-Renewable", "(Avg) PE Non-Renewable (kWh oil-eq a)", "(Avg) PE Non-Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Avg) UBP impact", "(Avg) UBP (P/m\xB2 a)", "(Avg) UBP (P/m\xB2 a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("-","----------------------------------------------", "-", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) Global warming potential", "(Selected) GWP (kg CO\x2082-eq/m\xB2 a)", "(Selected) Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) PE Total", "(Selected) PE Total (kWh oil-eq a)", "(Selected) PE Total (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) PE Renewable", "(Selected) PE Renewable (kWh oil-eq a)", "(Selected) PE Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) PE Non-Renewable", "(Selected) PE Non-Renewable (kWh oil-eq a)", "(Selected) PE Non-Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("(Selected) UBP impact", "(Selected) UBP (P/m\xB2 a)", "(Selected) UBP (P/m\xB2 a)", GH_ParamAccess.item);
            //pManager.AddNumberParameter("-", "----------------------------------------------", "-", GH_ParamAccess.item);
            pManager.AddTextParameter("Minimum values (text)", "Minimum values (text)", "Minimum values (text)", GH_ParamAccess.list);
            pManager.AddTextParameter("Minimum values (values)", "Minimum values (values)", "Minimum values (values)", GH_ParamAccess.list);

            pManager.AddTextParameter("Maximum values (text)", "Maximum values (text)", "Maximum values (text)", GH_ParamAccess.list);
            pManager.AddTextParameter("Maximum values (values)", "Maximum values (values)", "Maximum values (values)", GH_ParamAccess.list);

            pManager.AddTextParameter("Average values (text)", "Average values (text)", "Average values (text)", GH_ParamAccess.list);
            pManager.AddTextParameter("Average values (values)", "Average values (values)", "Average values (values)", GH_ParamAccess.list);

            pManager.AddTextParameter("Selected values (text)", "Selected values (text)", "Selected values (text)", GH_ParamAccess.list);
            pManager.AddTextParameter("Selected values (values)", "Selected values (values)", "Selected values (values)", GH_ParamAccess.list);
            //pManager.AddNumberParameter("-", "----------------------------------------------", "-", GH_ParamAccess.item);
            //pManager.AddTextParameter(" (All) LCA factors (text)", "(All) LCA factors (text)", "(All) Building Properties (text)", GH_ParamAccess.list);
            //pManager.AddNumberParameter(" (All) LCA factors (values)", "(All) LCA factors (values)", "(All) Building Properties (values)", GH_ParamAccess.list);
            //pManager.AddNumberParameter("-", "----------------------------------------------", "-", GH_ParamAccess.item);

            //pManager.AddTextParameter(" (Selected) LCA factors (text)", "(Selected) LCA factors (text)", "(Selected) Building Properties (text)", GH_ParamAccess.item);
            //pManager.AddNumberParameter(" (Selected) LCA factors (values)", "(Selected) LCA factors (values)", "(Selected) Building Properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputAll = new List<BuildingLevelModel>();
            DA.GetDataList(0, inputAll);

            var inputSpecific = new List<string>();
            DA.GetDataList(1, inputSpecific);

            var specificComponents = new List<BuildingLevelModel>();
            foreach (var item in inputSpecific)
            {
                var splitted = item.Split(':');
                specificComponents.Add(BuildingLevelDataAccess.GetSpecificForBuilding(splitted[0], splitted[1].Trim()));
            }

            var remainingComponents = new List<BuildingLevelModel>();
            DA.GetDataList(2, remainingComponents);

            var RSP = 0d;
            if (!DA.GetData(3, ref RSP)) { return; }
            var NFA = 0d;
            if (!DA.GetData(4, ref NFA)) { return; }

            var rspNFA = RSP * NFA;

            var splited = SplitList(inputAll, 3);
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
            //summ specific
            var ghgSumSpecific = 0d;
            var peTotalSumSpecific = 0d;
            var peRenewableSumSpecific = 0d;
            var peNonRenewableSumSpecific = 0d;
            var UBPSumSpecific = 0d;
            //summ specific remains
            var ghgSumSpecificRemain = 0d;
            var peTotalSumSpecificRemain = 0d;
            var peRenewableSumSpecificRemain = 0d;
            var peNonRenewableSumSpecificRemain = 0d;
            var UBPSumSpecificRemain = 0d;

            if (remainingComponents.Count != 0)
            {
                ghgSumSpecificRemain = remainingComponents.Average(ghg1 => ghg1.GHGEmbodied) + remainingComponents.Average(ghg2 => ghg2.GHGEoL);
                peTotalSumSpecificRemain = remainingComponents.Average(tot1 => tot1.TotalEmbodied) + remainingComponents.Average(tot2 => tot2.TotalEoL);
                peRenewableSumSpecificRemain = remainingComponents.Average(ren1 => ren1.RenewableEmbodied) + remainingComponents.Average(ren2 => ren2.RenewableEoL);
                peNonRenewableSumSpecificRemain = remainingComponents.Average(non1 => non1.NonRenewableEmbodied) + remainingComponents.Average(non2 => non2.NonRenewableEoL);
                UBPSumSpecificRemain = remainingComponents.Average(ubp1 => ubp1.UBPEmbodied) + remainingComponents.Average(ubp2 => ubp2.UBPEoL);
            }

            foreach (var list in splited)
            {
                ghgSumMin += (list[0].GHGEmbodied + list[0].GHGEoL);
                peTotalSumMin += (list[0].TotalEmbodied + list[0].TotalEoL);
                peRenewableSumMin += (list[0].RenewableEmbodied + list[0].RenewableEoL);
                peNonRenewableSumMin += (list[0].NonRenewableEmbodied + list[0].NonRenewableEoL);
                UBPSumMin += (list[0].UBPEmbodied + list[0].UBPEoL);

                ghgSumMax += (list[1].GHGEmbodied + list[1].GHGEoL);
                peTotalSumMax += (list[1].TotalEmbodied + list[1].TotalEoL);
                peRenewableSumMax += (list[1].RenewableEmbodied + list[1].RenewableEoL);
                peNonRenewableSumMax += (list[1].NonRenewableEmbodied + list[1].NonRenewableEoL);
                UBPSumMax += (list[1].UBPEmbodied + list[1].UBPEoL);

                ghgSumAvg += (list[2].GHGEmbodied + list[2].GHGEoL);
                peTotalSumAvg += (list[2].TotalEmbodied + list[2].TotalEoL);
                peRenewableSumAvg += (list[2].RenewableEmbodied + list[2].RenewableEoL);
                peNonRenewableSumAvg += (list[2].NonRenewableEmbodied + list[2].NonRenewableEoL);
                UBPSumAvg += (list[2].UBPEmbodied + list[2].UBPEoL);
            }

            foreach (var item in specificComponents)
            {
                ghgSumSpecific += (item.GHGEmbodied + item.GHGEoL);
                peTotalSumSpecific += (item.TotalEmbodied + item.TotalEoL);
                peRenewableSumSpecific += (item.RenewableEmbodied + item.RenewableEoL);
                peNonRenewableSumSpecific += (item.NonRenewableEmbodied + item.NonRenewableEoL);
                UBPSumSpecific += (item.UBPEmbodied + item.UBPEoL);
            }

            var resultsMin = new Dictionary<string, double>
            {
                { "(Min) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round(ghgSumMin / rspNFA, 4) },
                { "(Min) PE Total (kWh oil-eq/m\xB2 a)", Math.Round(peTotalSumMin / rspNFA, 4) },
                { "(Min) PE Renewable (kWh oil-eq/m\xB2 a)", Math.Round(peRenewableSumMin / rspNFA, 4) },
                { "(Min) PE Non-Renewable (kWh oil-eq/m\xB2 a)", Math.Round(peNonRenewableSumMin / rspNFA, 4) },
                { "(Min) UBP (P/m\xB2 a)", Math.Round(UBPSumMin / rspNFA, 4) },
            };

            var resultsMax = new Dictionary<string, double>
            {
                { "(Max) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round(ghgSumMax / rspNFA, 4) },
                { "(Max) PE Total (kWh oil-eq/m\xB2 a)", Math.Round(peTotalSumMax / rspNFA, 4) },
                { "(Max) PE Renewable (kWh oil-eq/m\xB2 a)", Math.Round(peRenewableSumMax / rspNFA, 4) },
                { "(Max) PE Non-Renewable (kWh oil-eq/m\xB2 a)", Math.Round(peNonRenewableSumMax / rspNFA, 4) },
                { "(Max) UBP (P/m\xB2 a)", Math.Round(UBPSumMax / rspNFA, 2) },
            };

            var resultsAvg = new Dictionary<string, double>
            {
                { "(Avg) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round(ghgSumAvg / rspNFA, 4) },
                { "(Avg) PE Total (kWh oil-eq/m\xB2 a)", Math.Round(peTotalSumAvg / rspNFA, 4) },
                { "(Avg) PE Renewable (kWh oil-eq/m\xB2 a)", Math.Round(peRenewableSumAvg / rspNFA, 4) },
                { "(Avg) PE Non-Renewable (kWh oil-eq/m\xB2 a)", Math.Round(peNonRenewableSumAvg / rspNFA, 4) },
                { "(Avg) UBP (P/m\xB2 a)", Math.Round(UBPSumAvg / rspNFA, 4) },
            };

            var resultsSpecific = new Dictionary<string, double>
            {
                { "(Avg) GWP (kg CO\x2082-eq/m\xB2 a)", Math.Round((ghgSumSpecific + ghgSumSpecificRemain) / rspNFA, 4) },
                { "(Avg) PE Total (kWh oil-eq/m\xB2 a)", Math.Round((peTotalSumSpecific + peTotalSumSpecificRemain) / rspNFA, 4) },
                { "(Avg) PE Renewable (kWh oil-eq/m\xB2 a)", Math.Round((peRenewableSumSpecific + peRenewableSumSpecificRemain) / rspNFA, 4) },
                { "(Avg) PE Non-Renewable (kWh oil-eq/m\xB2 a)", Math.Round((peNonRenewableSumSpecific + peNonRenewableSumSpecificRemain) / rspNFA, 4) },
                { "(Avg) UBP (P/m\xB2 a)", Math.Round((UBPSumSpecific + UBPSumSpecificRemain) / rspNFA, 4) },
            };

            var resultsMinValues = resultsMin.Values.ToList();
            DA.SetDataList(0, resultsMin);
            DA.SetDataList(1, resultsMinValues);

            var resultsMaxValues = resultsMax.Values.ToList();
            DA.SetDataList(2, resultsMax);
            DA.SetDataList(3, resultsMaxValues);

            var resultsAvgValues = resultsAvg.Values.ToList();
            DA.SetDataList(4, resultsAvg);
            DA.SetDataList(5, resultsAvgValues);

            var resultsSpecificValues = resultsSpecific.Values.ToList();
            DA.SetDataList(6, resultsSpecific);
            DA.SetDataList(7, resultsSpecificValues);
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