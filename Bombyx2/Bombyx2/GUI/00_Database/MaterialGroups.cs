using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bombyx2.GUI._00_Database
{
    public class MaterialGroups : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private readonly string[] GROUPS = new string[] {
            "01: Concrete",
            "02: Brick",
            "03: Other massive building materials",
            "04: Mortar and plaster",
            "05: Windows, solar shading and facade cladding",
            "06: Metal building materials",
            "07: Wood and wooden materials",
            "08: Adhesives and joint sealants",
            "09: Geomembranes and protective films",
            "10: Thermal insulation",
            "11: Flooring",
            "12: Doors",
            "13: Pipes",
            "14: Paints, coatings",
            "15: Plastics",
            "21: Kitchen fixtures and furniture",
            "00: Preparatory works" };

        public MaterialGroups()
          : base("1: Material Groups",
                 "Material Groups",
                 "Returns KBOB material groups from the database.",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate (Button)", "Activate (Button)", "Connect a Button to the first \ninput parameter(Activate) and \nclick it to show inputs.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Material groups", "Material groups", "Material groups", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Materials", "Materials", "Materials", GH_ParamAccess.list);
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
                CreateSelectionList(GROUPS, "Material groups", 1, 330, 75);
            }

            if (!DA.GetData(1, ref group)) { return; }

            var newParam = group.Split(':');
            var output = KbobMaterialsDataAccess.GetKbobMaterialsList(newParam[0] + "%");
            output.Sort();
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

        protected override Bitmap Icon => Icons.kbobMaterialGroups;

        public override Guid ComponentGuid => new Guid("7a4def93-8453-4289-93ad-ee0d59c214b8"); 
        
    }
}