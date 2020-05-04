using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._00_MaterialLevel.KBOB
{
    public class KbobEnergy : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private string[] EnergyList;

        public KbobEnergy()
          : base("KBOB Building Energy",
                 "KBOB energy",
                 "Returns selected KBOB energy from database",
                 "Bombyx 2",
                 "KBOB")
        {
            EnergyList = KbobMaterialsDataAccess.GetKbobEnergyList().ToArray();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Energy demand", "Energy\ndemand", "Energy demand", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Selected energy carrier", "Energy\ncarrier", "Selected energy carrier", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Energy properties (text)", "Energy\nproperties (text)", "Energy properties (text)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Energy properties (values)", "Energy\nproperties (values)", "Energy properties (values)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            var demand = new List<double>();
            string input = null;
            if (!DA.GetDataList(0, demand)) { return; }        

            if (Params.Input[0].SourceCount == 1 && Params.Input[1].SourceCount == 0)
            {
                CreateSelectionList(EnergyList, "Energy selection", 1, 330, 60);
                ExpireSolution(true);
            }

            if (!DA.GetData(1, ref input)) { return; }

            var demandSum = demand.Sum(x => Convert.ToDouble(x));
            var newParam = input.Split(':');
            var energy = KbobMaterialsDataAccess.GetKbobEnergy(newParam[0]);
            var output = new Dictionary<string, double>
            {
                { "Global warming potential (kg CO\x2082-eq/m\xB2 a)", energy.GHG * demandSum },
                { "PE Total (kWh oil-eq/m\xB2 a)", energy.PeTotal * demandSum },
                { "PE Renewable (kWh oil-eq/m\xB2 a)", energy.PeRenewable * demandSum },
                { "PE Non Renewable (kWh oil-eq/m\xB2 a)", energy.PeNonRenewable * demandSum },
                { "PE Renewable at Location (kWh oil-eq/m\xB2 a)", energy.PePeRenewableAtLocation * demandSum },
                { "UBP (P/m\xB2 a)", energy.UBP * demandSum }
            };

            ExpireSolution(true);

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
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
                return Icons.kbobEnergy;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8a5fe063-4971-4aa9-8efa-752e9f739fdc"); }
        }
    }
}