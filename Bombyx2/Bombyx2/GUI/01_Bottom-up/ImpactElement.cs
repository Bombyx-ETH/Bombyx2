using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Bombyx2.GUI._01_Bottom_up
{
    public class ImpactElement : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private string[] FunctionsList = new string[] { "External wall", "Internal wall", "Floor", "Ceiling", "Roof", "Other" };

        public ImpactElement()
          : base("3: Element impact",
                 "Element impact",
                 "Calculates impacts of PE, GWP, UBP",
                 "Bombyx 2",
                 "1: Bottom-up")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Component properties", "Component properties", "List of component properties", GH_ParamAccess.list);
            pManager[0].DataMapping = GH_DataMapping.Flatten;
            pManager.AddTextParameter("Thermal resistivity", "Thermal resistivity", "By selecting element's Thermal resistivity, air resistance will be added to the U value.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Surface area (square meters)", "Surface area (m\xB2)", "Manual value", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LCA factors (text)", "LCA factors (text)", "Element Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("LCA factors (values)", "LCA factors (values)", "Element Properties (values)", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value", "U value (W/m2*K)", "U Value", GH_ParamAccess.item);
            pManager.AddNumberParameter("UA value", "UA value (W/K)", "Area U Value", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            var component = new List<double>();
            var funct = "";
            var area = 0d;

            if (!DA.GetDataList(0, component)) { return; }

            if (Params.Input[0].SourceCount == 1 && Params.Input[1].SourceCount == 0)
            {
                CreateSelectionList(FunctionsList, "Thermal resistivity", 1, 200, 50);
                ExpireSolution(true);
            }

            if (!DA.GetData(1, ref funct)) { return; }
            if (!DA.GetData(2, ref area)) { return; }

            var valueSets = component.Select((x, i) => new { Index = i, Value = x })
                                     .GroupBy(x => x.Index / (16 + 1)) // +1 for BiogenicCarbon
                                     .Select(x => x.Select(v => v.Value).ToList())
                                     .ToList();

            var results = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P)", 0 },
                { "UBP13 Replacements (P)", 0 },
                { "UBP13 End of Life (P)", 0 },
                { "PE Total Embodied (kWh oil-eq)", 0 },
                { "PE Total Replacements (kWh oil-eq)", 0 },
                { "PE Total End of Life (kWh oil-eq)", 0 },
                { "PE Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Renewable End of Life (kWh oil-eq)", 0 },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0 },
                { "PE Non Renewable Replacements (kWh oil-eq)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq)", 0 },
                { "U value", 0 },
                { "Biogenic Carbon Storage (kg CO₂-eq)", 0 }
            };

            foreach (var item in valueSets)
            {
                results["UBP13 Embodied (P)"] += item[0];
                results["UBP13 Replacements (P)"] += item[1];
                results["UBP13 End of Life (P)"] += item[2];
                results["PE Total Embodied (kWh oil-eq)"] += item[3];
                results["PE Total Replacements (kWh oil-eq)"] += item[4];
                results["PE Total End of Life (kWh oil-eq)"] += item[5];
                results["PE Renewable Embodied (kWh oil-eq)"] += item[6];
                results["PE Renewable Replacements (kWh oil-eq)"] += item[7];
                results["PE Renewable End of Life (kWh oil-eq)"] += item[8];
                results["PE Non Renewable Embodied (kWh oil-eq)"] += item[9];
                results["PE Non Renewable Replacements (kWh oil-eq)"] += item[10];
                results["PE Non Renewable End of Life (kWh oil-eq)"] += item[11];
                results["Green House Gasses Embodied (kg CO\x2082-eq)"] += item[12];
                results["Green House Gasses Replacements (kg CO\x2082-eq)"] += item[13];
                results["Green House Gasses End of Life (kg CO\x2082-eq)"] += item[14];
                results["U value"] += item[15];
                results["Biogenic Carbon Storage (kg CO₂-eq)"] += item[16];
            }

            results["UBP13 Embodied (P)"] = Math.Round(results["UBP13 Embodied (P)"] * area, 2);
            results["UBP13 Replacements (P)"] = Math.Round(results["UBP13 Replacements (P)"] * area, 2);
            results["UBP13 End of Life (P)"] = Math.Round(results["UBP13 End of Life (P)"] * area, 2);
            results["PE Total Embodied (kWh oil-eq)"] = Math.Round(results["PE Total Embodied (kWh oil-eq)"] * area, 2);
            results["PE Total Replacements (kWh oil-eq)"] = Math.Round(results["PE Total Replacements (kWh oil-eq)"] * area, 2);
            results["PE Total End of Life (kWh oil-eq)"] = Math.Round(results["PE Total End of Life (kWh oil-eq)"] * area, 2);
            results["PE Renewable Embodied (kWh oil-eq)"] = Math.Round(results["PE Renewable Embodied (kWh oil-eq)"] * area, 2);
            results["PE Renewable Replacements (kWh oil-eq)"] = Math.Round(results["PE Renewable Replacements (kWh oil-eq)"] * area, 2);
            results["PE Renewable End of Life (kWh oil-eq)"] = Math.Round(results["PE Renewable End of Life (kWh oil-eq)"] * area, 2);
            results["PE Non Renewable Embodied (kWh oil-eq)"] = Math.Round(results["PE Non Renewable Embodied (kWh oil-eq)"] * area, 2);
            results["PE Non Renewable Replacements (kWh oil-eq)"] = Math.Round(results["PE Non Renewable Replacements (kWh oil-eq)"] * area, 2);
            results["PE Non Renewable End of Life (kWh oil-eq)"] = Math.Round(results["PE Non Renewable End of Life (kWh oil-eq)"] * area, 2);
            results["Green House Gasses Embodied (kg CO\x2082-eq)"] = Math.Round(results["Green House Gasses Embodied (kg CO\x2082-eq)"] * area, 2);
            results["Green House Gasses Replacements (kg CO\x2082-eq)"] = Math.Round(results["Green House Gasses Replacements (kg CO\x2082-eq)"] * area, 2);
            results["Green House Gasses End of Life (kg CO\x2082-eq)"] = Math.Round(results["Green House Gasses End of Life (kg CO\x2082-eq)"] * area, 2);
            results["U value"] = Math.Round(1 / results["U value"], 4);
            results["Biogenic Carbon Storage (kg CO₂-eq)"] = Math.Round(results["Biogenic Carbon Storage (kg CO₂-eq)"] * area, 2);

            var resultValues = results.Values.ToList();

            DA.SetDataList(0, results);
            DA.SetDataList(1, resultValues);

            if (funct.Equals("Window"))
            {
                DA.SetData(2, 1 / results["U value"]);
                DA.SetData(3, (1 / results["U value"]) * area);
            }
            else if (funct.Equals("Internal wall") || funct.Equals("External wall") || funct.Equals("Roof") || funct.Equals("Ceiling"))
            {
                DA.SetData(2, results["U value"] + 0.17);
                DA.SetData(3, (results["U value"] + 0.17) * area);
            }
            else if (funct.Equals("Floor"))
            {
                DA.SetData(2, results["U value"] + 0.13);
                DA.SetData(3, (results["U value"] + 0.13) * area);
            }
            else if (funct.Equals("Other"))
            {
                DA.SetData(2, results["U value"]);
                DA.SetData(3, (results["U value"]) * area);
            }
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

        protected override Bitmap Icon => Icons.impactElement;

        public override Guid ComponentGuid => new Guid("3d00d88e-289d-4e2c-88fc-2fbf5ae59e82"); 
    }
}