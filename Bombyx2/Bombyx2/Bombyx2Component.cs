using System;
using Grasshopper.Kernel;

namespace Bombyx2
{
    public class Bombyx2Component : GH_Component
    {
        public Bombyx2Component()
          : base("Bombyx 2", 
                 "Bombyx 2",
                 "Bombyx description",
                 "Bombyx 2", 
                 "Base")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
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
            get { return new Guid("8823b024-6994-4ed5-b19b-627733283d75"); }
        }
    }
}
