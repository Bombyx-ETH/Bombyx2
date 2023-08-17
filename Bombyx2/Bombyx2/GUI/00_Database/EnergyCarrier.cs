using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Rhino.Render.ChangeQueue.Light;

namespace Bombyx2.GUI._00_Database
{
    public class EnergyCarrier : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private string[] EnergyList;
        private int reset_counter;

        public EnergyCarrier()
          : base("5: Energy carrier",
                 "Energy carrier",
                 "Returns selected KBOB energy from database",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
            EnergyList = KbobMaterialsDataAccess.GetKbobEnergyList().ToArray();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Energy demand", "Demand", "Energy demand", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Selected energy carrier", "Carrier", "Selected energy carrier", GH_ParamAccess.item);
            pManager[1].Optional = true;
            reset_counter = 0;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Energy properties (text)", "Properties (text)", "Energy properties (text)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Energy properties (values)", "Properties (values)", "Energy properties (values)", GH_ParamAccess.list);
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
                { "Global warming potential (kg CO\x2082-eq/m\xB2 a)", Math.Round(energy.GHG * demandSum, 2) },
                { "PE Total (kWh oil-eq/m\xB2 a)", Math.Round(energy.PeTotal * demandSum, 2) },
                { "PE Renewable (kWh oil-eq/m\xB2 a)", Math.Round(energy.PeRenewable * demandSum, 2) },
                { "PE Non Renewable (kWh oil-eq/m\xB2 a)", Math.Round(energy.PeNonRenewable * demandSum, 2) },
                { "PE Renewable at Location (kWh oil-eq/m\xB2 a)", Math.Round(energy.PePeRenewableAtLocation * demandSum, 2) },
                { "UBP (P/m\xB2 a)", Math.Round(energy.UBP * demandSum, 2) }
            };

            if (reset_counter == 0)     // 2.0.9 Pedram: Was the problem. Set it such that it only 
            {                           // happens once to reset the component after the inputs are 
                ExpireSolution(true);   // connected to prevent the initial tree confusion.
                reset_counter = 1;     
            }                           

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

        protected override Bitmap Icon => Icons.kbobEnergy;

        public override Guid ComponentGuid => new Guid("c4d844b9-a4ba-4571-96d8-1f3efb0afdf5");         
    }
}