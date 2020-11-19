using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._00_MaterialLevel.KBOB
{
    public class KbobTransportPeople : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        public KbobTransportPeople()
          : base("KBOB Transport people",
                 "KBOB Transport people",
                 "Returns selected KBOB people transport details from database",
                 "Bombyx 2",
                 "KBOB")
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
            Component = this;
            GrasshopperDocument = OnPingDocument();



        }

        protected override Bitmap Icon
        {
            get
            {
                return Icons.kbobTransportPeople;
            }
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

        public override Guid ComponentGuid
        {
            get { return new Guid("f0884e87-a3fc-4e75-a739-8a7fe3d54366"); }
        }
    }
}