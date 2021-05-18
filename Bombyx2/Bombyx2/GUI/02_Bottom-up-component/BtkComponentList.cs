using System;
using System.Collections.Generic;
using System.Drawing;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._01_ComponentLevel
{
    public class BtkComponentList : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private string[] ComponentList;

        public BtkComponentList()
          : base("1: Component Groups",
                 "Component Groups",
                 "Returns list of Bauteilkatalog component groups from database.",
                 "Bombyx 2",
                 "2: Bottom-up-component")
        {
            ComponentList = BtkComponentsDataAccess.GetBtkComponentsGroups().ToArray();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate (Button)", "Activate (Button)", "Connect a Button to the first \ninput parameter(Activate) and \nclick it to show inputs.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Component groups", "Component groups", "Component groups", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Components", "Components", "Components", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            bool input = false;
            var group = "";
            if (!DA.GetData(0, ref input)) { return; }

            if (input && Params.Input[1].SourceCount == 0)
            {
                CreateSelectionList(ComponentList, "Component groups", 1, 305, 90);
            }

            if (!DA.GetData(1, ref group)) { return; }

            var newParam = group.Split(':');
            var output = BtkComponentsDataAccess.GetBtkComponentsList(newParam[0]);

            DA.SetDataList(0, output);
        }

        private void CreateSelectionList(string[] values, string nick, int inputParam, int offsetX, int offsetY)
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
            vl.Attributes.Pivot = new PointF(Component.Attributes.Bounds.X - offsetX, Component.Attributes.Bounds.Y + offsetY);

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
                return Icons.btkComponentGroups;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("c1c8da7d-685a-4dd0-9a93-b6536a823445"); }
        }
    }
}