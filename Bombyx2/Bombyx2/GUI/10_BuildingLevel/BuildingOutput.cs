using System;
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
            pManager.AddTextParameter("Element", "Element", "Element", GH_ParamAccess.item);
            pManager.AddTextParameter("Selected element", "Selected element", "Selected element", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Global warming potential", "GWP (kg CO\x2082-eq/m\xB2 a)", "Global warming potential (kg CO\x2082-eq/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("PE Total", "PE Total (kWh oil-eq a)", "PE Total (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("PE Renewable", "PE Renewable (kWh oil-eq a)", "PE Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("PE Non-Renewable", "PE Non-Renewable (kWh oil-eq a)", "PE Non-Renewable (kWh oil-eq a)", GH_ParamAccess.item);
            pManager.AddNumberParameter("UBP impact", "UBP (P/m\xB2 a)", "UBP (P/m\xB2 a)", GH_ParamAccess.item);
            pManager.AddTextParameter("LCA factors (text)", "LCA factors (text)", "Building Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("LCA factors (values)", "LCA factors (values)", "Building Properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            get { return new Guid("2c0de49c-1310-40bb-bc01-d03bc6e47e59"); }
        }
    }
}