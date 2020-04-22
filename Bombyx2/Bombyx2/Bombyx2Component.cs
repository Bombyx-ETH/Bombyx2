using System;
using System.Collections.Generic;
using Bombyx2.Data.Access;
using Bombyx2.Data.Models;
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

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("test", "test", "test", GH_ParamAccess.list);
            pManager.AddTextParameter("materials", "materials", "materials", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Class1> persons = new List<Class1>();
            persons = SQLiteDataAccess.LoadPersons();
            List<string> output = new List<string>();

            List<KbobMaterial> materials = new List<KbobMaterial>();
            materials = KbobMaterialsDataAccess.LoadKbobMaterials();
            List<string> outputMats = new List<string>();

            foreach (var item in persons)
            {
                output.Add(item.Name);
            }

            foreach (var item in materials)
            {
                outputMats.Add(item.NameEnglish);
            }

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputMats);
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
