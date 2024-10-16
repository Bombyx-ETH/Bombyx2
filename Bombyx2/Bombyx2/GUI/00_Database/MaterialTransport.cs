﻿using Bombyx2.Data.Access;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Bombyx2.GUI._00_Database
{
    public class MaterialTransport : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private List<KbobTransportModel> KbobTransports = new List<KbobTransportModel>();

        public MaterialTransport()
          : base("0.6: Transport Material",
                 "Transport Material",
                 "Returns selected KBOB material transport details from database.",
                 "Bombyx 2",
                 "0: Database")
        {
            Message = "Bombyx v" + Config.Version;
            KbobTransports = KbobMaterialsDataAccess.GetTransport("material");
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Distance", "Distance (km)", "Distance in kilometers", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mass", "Mass (t)", "Mass in metric tonnes", GH_ParamAccess.item);
            pManager.AddTextParameter("Transport type", "Transport type", "Select the type of transport", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Truck percentage", "Truck (%)", "Truck percentage (e.g. 30). Default is 0.", GH_ParamAccess.item, 0d);
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

            var distance = 0d;
            var mass = 0d;

            if (!DA.GetData(0, ref distance)) { return; }
            if (!DA.GetData(1, ref mass)) { return; }

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

            var percent = 0d;
            if (!DA.GetData(3, ref percent)) { return; }

            var majorDistance = (mass * distance) * ((100 - percent) / 100);
            var minorDistance = (mass * distance) * (percent / 100);

            var averageTruck = new Dictionary<string, double>
            {
                { "UBPOperation", (percent > 0 && percent <= 100) ? KbobTransports[12].UBPOperation * minorDistance : 0 },
                { "none1", 0 },
                { "UBPFahrInfrastr", (percent > 0 && percent <= 100) ? KbobTransports[12].UBPFahrInfrastr * minorDistance : 0 },
                { "TotalOperation", (percent > 0 && percent <= 100) ? KbobTransports[12].TotalOperation * minorDistance : 0 },
                { "none2", 0 },
                { "TotalFahrInfrastr", (percent > 0 && percent <= 100) ? KbobTransports[12].TotalFahrInfrastr * minorDistance : 0 },
                { "REOperation", (percent > 0 && percent <= 100) ? KbobTransports[12].REOperation * minorDistance : 0 },
                { "none3", 0 },
                { "REFahrInfrastr", (percent > 0 && percent <= 100) ? KbobTransports[12].REFahrInfrastr * minorDistance : 0 },
                { "NEOperation", (percent > 0 && percent <= 100) ? KbobTransports[12].NEOperation * minorDistance : 0 },
                { "none4", 0 },
                { "NEFahrInfrastr", (percent > 0 && percent <= 100) ? KbobTransports[12].NEFahrInfrastr * minorDistance : 0 },
                { "GHGOperation", (percent > 0 && percent <= 100) ? KbobTransports[12].GHGOperation * minorDistance : 0 },
                { "none5", 0 },
                { "GHGFahrInfrastr", (percent > 0 && percent <= 100) ? KbobTransports[12].GHGFahrInfrastr * minorDistance : 0 },
                { "none6", 0 }
            };

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

            foreach (var item in KbobTransports)
            {
                if (item.NameEnglish.Equals(transport))
                {
                    output["UBP Operation"] = results["UBPOperation"] = Math.Round((item.UBPOperation * majorDistance) + averageTruck["UBPOperation"], 3);
                    output["UBP Vehicle and Infrastrcture"] = results["UBPFahrInfrastr"] = Math.Round((item.UBPFahrInfrastr * majorDistance) + averageTruck["UBPFahrInfrastr"], 3);
                    output["Total Operation (kWh oil-eq)"] = results["TotalOperation"] = Math.Round((item.TotalOperation * majorDistance) + averageTruck["TotalOperation"], 3);
                    output["Total Vehicle and Infrastrcture (kWh oil-eq)"] = results["TotalFahrInfrastr"] = Math.Round((item.TotalFahrInfrastr * majorDistance) + averageTruck["TotalFahrInfrastr"], 3);
                    output["Renewable Operation (kWh oil-eq)"] = results["REOperation"] = Math.Round((item.REOperation * majorDistance) + averageTruck["REOperation"], 3);
                    output["Renewable Vehicle and Infrastrcture (kWh oil-eq)"] = results["REFahrInfrastr"] = Math.Round((item.REFahrInfrastr * majorDistance) + averageTruck["REFahrInfrastr"], 3);
                    output["Non Renewable Operation (kWh oil-eq)"] = results["NEOperation"] = Math.Round((item.NEOperation * majorDistance) + averageTruck["NEOperation"], 3);
                    output["Non Renewable Vehicle and Infrastrcture (kWh oil-eq)"] = results["NEFahrInfrastr"] = Math.Round((item.NEFahrInfrastr * majorDistance) + averageTruck["NEFahrInfrastr"], 3);
                    output["GHG Operation (kg CO2-eq)"] = results["GHGOperation"] = Math.Round((item.GHGOperation * majorDistance) + averageTruck["GHGOperation"], 3);
                    output["GHG Vehicle and Infrastrcture (kg CO2-eq)"] = results["GHGFahrInfrastr"] = Math.Round((item.GHGFahrInfrastr * majorDistance) + averageTruck["GHGFahrInfrastr"], 3);
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

        protected override Bitmap Icon => Icons.kbobTransportMaterial;

        public override Guid ComponentGuid => new Guid("8bc45edb-8794-488f-b1b2-487e03b36bff");
    }
}