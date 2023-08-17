using Bombyx2.Data.Access;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Bombyx2.GUI._00_Database
{
    public class PeopleTransport : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private List<KbobTransportModel> KbobTransports = new List<KbobTransportModel>();

        public PeopleTransport()
          : base("7: Transport People",
                 "Transport People",
                 "Returns selected KBOB people transport details from database.",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
            KbobTransports = KbobMaterialsDataAccess.GetTransport("people");
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("No. of people", "No. of people", "No. of people", GH_ParamAccess.item);
            pManager.AddNumberParameter("Avg distance", "Avg distance (km)", "Average distance in kilometers", GH_ParamAccess.item);
            pManager.AddTextParameter("Transport type", "Transport type", "Select the type of transport", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("No. of trips", "No. of trips", "Number of trips, default is 1", GH_ParamAccess.item, 1);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Transport (text)", "Transport (text)", "Transport properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Transport (values)", "Transport (values)", "Transport properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            var people = 0d;
            var distance = 0d;

            if (!DA.GetData(0, ref people)) { return; }
            if (!DA.GetData(1, ref distance)) { return; }

            var transport = "";
            var types = new List<string>();
            foreach (var item in KbobTransports)
            {
                types.Add(item.NameEnglish);
            }

            if (Params.Input[0].SourceCount == 1 && Params.Input[2].SourceCount == 0)
            {
                CreateDropDownList(types.Distinct().ToArray(), "Type", 2, 300, -10);
                ExpireSolution(true);
            }

            if (!DA.GetData(2, ref transport)) { return; }

            var trips = 0d;
            if (!DA.GetData(3, ref trips)) { return; }

            var results = new Dictionary<string, double>
            {
                { "UBPOperation", 0 },
                { "none1", 0 },
                { "UBPFahrInfrastr", 0 },
                { "TotalOperation", 0 },
                { "none2", 0 },
                { "TotalFahrInfrastr", 0 },
                { "REOperation", 0 },
                { "none3", 0 },
                { "REFahrInfrastr", 0 },
                { "NEOperation", 0 },
                { "none4", 0 },
                { "NEFahrInfrastr", 0 },
                { "GHGOperation", 0 },
                { "none5", 0 },
                { "GHGFahrInfrastr", 0 },
                { "none6", 0 },
                { "Biogenic Carbon Storage (kg CO₂-eq/m²)", 0 }
            };

            var output = new Dictionary<string, double>
            {
                { "UBP Operation", 0 },
                { "UBP Vehicle and Infrastrcture", 0 },
                { "Total Operation (kWh oil-eq)", 0 },
                { "Total Vehicle and Infrastrcture (kWh oil-eq)", 0 },
                { "Renewable Operation (kWh oil-eq)", 0 },
                { "Renewable Vehicle and Infrastrcture (kWh oil-eq)", 0 },
                { "Non Renewable Operation (kWh oil-eq)", 0 },
                { "Non Renewable Vehicle and Infrastrcture (kWh oil-eq)", 0 },
                { "GHG Operation (kg CO2-eq)", 0 },
                { "GHG Vehicle and Infrastrcture (kg CO2-eq)", 0 },
                { "Biogenic Carbon Storage (kg CO₂-eq/m²)", 0}
            };

            var peopleDistanceTrips = people * distance * trips;

            foreach (var item in KbobTransports)
            {
                if (item.NameEnglish.Equals(transport))
                {
                    output["UBP Operation"] = results["UBPOperation"] = Math.Round(item.UBPOperation * peopleDistanceTrips, 2);
                    output["UBP Vehicle and Infrastrcture"] = results["UBPFahrInfrastr"] = Math.Round(item.UBPFahrInfrastr * peopleDistanceTrips, 2);
                    output["Total Operation (kWh oil-eq)"] = results["TotalOperation"] = Math.Round(item.TotalOperation * peopleDistanceTrips, 2);
                    output["Total Vehicle and Infrastrcture (kWh oil-eq)"] = results["TotalFahrInfrastr"] = Math.Round(item.TotalFahrInfrastr * peopleDistanceTrips, 2);
                    output["Renewable Operation (kWh oil-eq)"] = results["REOperation"] = Math.Round(item.REOperation * peopleDistanceTrips, 2);
                    output["Renewable Vehicle and Infrastrcture (kWh oil-eq)"] = results["REFahrInfrastr"] = Math.Round(item.REFahrInfrastr * peopleDistanceTrips, 2);
                    output["Non Renewable Operation (kWh oil-eq)"] = results["NEOperation"] = Math.Round(item.NEOperation * peopleDistanceTrips, 2);
                    output["Non Renewable Vehicle and Infrastrcture (kWh oil-eq)"] = results["NEFahrInfrastr"] = Math.Round(item.NEFahrInfrastr * peopleDistanceTrips, 2);
                    output["GHG Operation (kg CO2-eq)"] = results["GHGOperation"] = Math.Round(item.GHGOperation * peopleDistanceTrips, 2);
                    output["GHG Vehicle and Infrastrcture (kg CO2-eq)"] = results["GHGFahrInfrastr"] = Math.Round(item.GHGFahrInfrastr * peopleDistanceTrips, 2);
                }
            }

            var outputValues = results.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
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

        protected override System.Drawing.Bitmap Icon => Icons.kbobTransportPeople;

        public override Guid ComponentGuid => new Guid("1e4bbcd7-9c8b-4c99-b73b-a9e6d556a608");        
    }
}