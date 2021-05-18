using Grasshopper.Kernel;
using System;

namespace Bombyx2.GUI._04_HeatingDemand
{
    public class GlazedElements
    {
        public double UA { get; set; }
        public string Orientation { get; set; }
        public double HorizontalAngle { get; set; }
        public double Angle1 { get; set; }
        public double Angle2 { get; set; }
        public double Angle3 { get; set; }
        public double GlazingPercent { get; set; }

    }

    public class Windows : GH_Component
    {
        public Windows()
          : base("Windows",
                 "Windows",
                 "Calculates heating demand.",
                 "Bombyx 2",
                 "4: Heating demand")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("U value", "U value", "U value", GH_ParamAccess.item);
            pManager.AddNumberParameter("Area", "Area", "Area", GH_ParamAccess.item);
            pManager.AddNumberParameter("G value", "G value", "G value", GH_ParamAccess.item);
            pManager.AddNumberParameter("Glazing percentage", "Glazing percentage", "Glazing percentage", GH_ParamAccess.item);
            pManager.AddNumberParameter("Horizontal angle", "Horizontal angle", "Horizontal angle", GH_ParamAccess.item);
            pManager.AddNumberParameter("angle1", "angle1", "angle1", GH_ParamAccess.item); 
            pManager.AddNumberParameter("angle2", "angle2", "angle2", GH_ParamAccess.item);
            pManager.AddNumberParameter("angle3", "angle3", "angle3", GH_ParamAccess.item);
            pManager.AddTextParameter("Orientation", "Orientation", "Orientation", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Glazed elements", "Glazed elements", "Glazed elements", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var output = new GlazedElements();

            var uvalue = 0d;
            if (!DA.GetData(0, ref uvalue)) { return; }
            var area = 0d;
            if (!DA.GetData(1, ref area)) { return; }
            var gvalue = 0d;
            if (!DA.GetData(2, ref gvalue)) { return; }
            var glazing = 0d;
            if (!DA.GetData(3, ref glazing)) { return; }
            var horizontal = 0d;
            if (!DA.GetData(4, ref horizontal)) { return; }
            var angle1 = 0d;
            if (!DA.GetData(1, ref angle1)) { return; }
            var angle2 = 0d;
            if (!DA.GetData(2, ref angle2)) { return; }
            var angle3 = 0d;
            if (!DA.GetData(3, ref angle3)) { return; }
            var orientation = "";
            if (!DA.GetData(4, ref orientation)) { return; }

            output.UA = uvalue * area;
            output.Orientation = orientation;
            output.HorizontalAngle = gvalue;
            output.Angle1 = glazing;
            output.Angle2 = horizontal;
            output.Angle3 = angle3;
            output.GlazingPercent = area * gvalue * glazing;

            DA.SetData(0, output);
        }

        protected override System.Drawing.Bitmap Icon => Icons.window;

        public override Guid ComponentGuid => new Guid("a4337462-17c7-4bb5-a512-900648c5e38d"); 
    }
}