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
            pManager.AddTextParameter("(All) Global warming potential", "(All) GWP (kg CO\x2082-eq/m\xB2 a)", "(All) Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) PE Total", "(All) PE Total (kWh oil-eq a)", "(All) PE Total (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) PE Renewable", "(All) PE Renewable (kWh oil-eq a)", "(All) PE Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) PE Non-Renewable", "(All) PE Non-Renewable (kWh oil-eq a)", "(All) PE Non-Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("(All) UBP impact", "(All) UBP (P/m\xB2 a)", "(All) UBP (P/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddTextParameter(" (All) LCA factors (text)", "(All) LCA factors (text)", "(All) Building Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter(" (All) LCA factors (values)", "(All) LCA factors (values)", "(All) Building Properties (values)", GH_ParamAccess.item);
            pManager.AddNumberParameter("-", "-------------------------------------------------", "-", GH_ParamAccess.item);
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

            var ghgSum = 0d;

            foreach (var item in input)
            {
                ghgSum += item.GHGEmbodied + item.GHGEoL;
            }

            DA.SetData(0, ghgSum.ToString());
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
    }
}