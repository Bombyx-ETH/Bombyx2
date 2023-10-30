using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Bombyx2.Data.Access;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._10_BuildingLevel
{
    public class SpecificComponent : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        public SpecificComponent()
          : base("3.4: Specific component",
                 "Specific component",
                 "Select a specific component for the element",
                 "Bombyx 2",
                 "3: Top-down")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List of components", "List of components", "List of components", GH_ParamAccess.list);
            pManager.AddTextParameter("Building inputs", "Building inputs", "Building inputs", GH_ParamAccess.list);
            pManager.AddTextParameter("Component eBKP", "Component eBKP", "Component eBKP", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Specific component", "Specific\ncomponent", "Specific component", GH_ParamAccess.list);
            pManager.AddGenericParameter("Remaining components", "Remaining\ncomponents", "Remaining components", GH_ParamAccess.list);
            pManager.AddTextParameter("Building inputs", "Building inputs", "Building inputs", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            var input = new List<BuildingLevelModel>();
            if (!DA.GetDataList(0, input)) { return; }

            var buildingInputs = new List<string>();
            if (!DA.GetDataList(1, buildingInputs)) { return; }

            string component = "";
            var components = new List<string>();
            foreach (var item in input)
            {
                components.Add(item.eBKP);
            }

            if (Params.Input[0].SourceCount == 1 && Params.Input[2].SourceCount == 0)
            {
                CreateDropDownList(components.Distinct().ToArray(), "eBKP", 2, 150, -10);
                ExpireSolution(true);
            }

            if (!DA.GetData(2, ref component)) { return; }

            var output = BuildingLevelDataAccess.GetComponentsForElement(component, buildingInputs);

            var results = new List<BuildingLevelModel>();
            foreach (var item in input)
            {
                if(!item.eBKP.Equals(component))
                {
                    results.Add(item);
                }
            }

            DA.SetDataList(0, output);
            DA.SetDataList(1, results);
            DA.SetDataList(2, buildingInputs);
        }

        private void CreateDropDownList(string[] values, string nick, int inputParam, int offsetX, int offsetY)
        {
            GH_DocumentIO docIO = new GH_DocumentIO();
            docIO.Document = new GH_Document();

            GH_ValueList vl = new GH_ValueList();
            vl.ListItems.Clear();

            foreach (string item in values)
            {
                GH_ValueListItem vi = new GH_ValueListItem(item, String.Format("\"{0}\"", item));
                vl.ListItems.Add(vi);
            }

            vl.NickName = nick;
            GH_Document doc = OnPingDocument();
            if (docIO.Document == null) return;

            docIO.Document.AddObject(vl, false, inputParam);
            PointF currPivot = Params.Input[inputParam].Attributes.Pivot;
            vl.Attributes.Pivot = new PointF(currPivot.X - offsetX, currPivot.Y + offsetY);

            docIO.Document.SelectAll();
            docIO.Document.ExpireSolution();
            docIO.Document.MutateAllIds();
            IEnumerable<IGH_DocumentObject> objs = docIO.Document.Objects;
            doc.MergeDocument(docIO.Document);
            Component.Params.Input[inputParam].AddSource(vl);
            doc.DeselectAll();
        }

        protected override Bitmap Icon
        {
            get
            {
                return Icons.specificComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("af1cd9d4-5a05-4c69-9160-4d5406ad1a8d"); }
        }
    }
}