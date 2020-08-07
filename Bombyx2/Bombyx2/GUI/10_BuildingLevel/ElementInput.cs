using System;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._10_BuildingLevel
{
    public class ElementInput : GH_Component
    {
        public ElementInput()
          : base("Element Input",
                 "Element input",
                 "Select element for building level",
                 "Bombyx 2",
                 "Building level")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
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
            get { return new Guid("cfda6d8b-9331-4dda-b0d0-d210dfdcc28b"); }
        }
    }
}