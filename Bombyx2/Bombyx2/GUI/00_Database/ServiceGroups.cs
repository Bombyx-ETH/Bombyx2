using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bombyx2.GUI._00_Database
{
    public class ServiceGroups : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private readonly string[] ServicesList = new string[] {
            "31: Heating systems",
            "32: Ventilation systems",
            "33: Sanitary",
            "34: Electrical systems" };

        public ServiceGroups()
          : base("3: Systems Groups",
                 "Systems Groups",
                 "Returns the list of building systems from the KBOB database",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate (Button)", "Activate (Button)", "Connect a Button to the first \ninput parameter(Activate) and \nclick it to show inputs.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Systems groups", "Systems groups", "Systems groups", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Systems", "Systems", "Systems", GH_ParamAccess.list);
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
                CreateSelectionList(ServicesList, "Systems groups", 1, 200, 70);
            }

            if (!DA.GetData(1, ref group)) { return; }

            var newParam = group.Split(':');
            var output = KbobMaterialsDataAccess.GetKbobServicesList(newParam[0] + "%");

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

        protected override Bitmap Icon => Icons.kbobServiceGroups;

        public override Guid ComponentGuid => new Guid("08eb8081-582e-4b68-a8f7-bb72c836f06e"); 
    }
}