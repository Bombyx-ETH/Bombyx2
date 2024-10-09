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
          : base("1.3: Element impact",
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
            var R_ie = 0d;
            var R_element = 0d;
            var R_tot = 0d;

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
                { "UBP Embodied (P)", 0 },
                { "UBP Replacements (P)", 0 },
                { "UBP End of Life (P)", 0 },
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
                results["UBP Embodied (P)"] += item[0];
                results["UBP Replacements (P)"] += item[1];
                results["UBP End of Life (P)"] += item[2];
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

            R_element = results["U value"];

            switch (funct) // Based on SIA 180
            {
                case "Window":
                    R_ie = 0d;
                    break;
                case "Internal wall":
                    R_ie = 0.250;
                    break;
                case "External wall":
                    R_ie = 0.165;
                    break;
                case "Roof":
                    R_ie = 0.165;
                    break;
                case "Ceiling":
                    R_ie = 0.292;
                    break;
                case "Floor":
                    R_ie = 0.292;
                    break;
                case "Other":
                    R_ie = 0d;
                    break;
                default:
                    R_ie = 0d;
                    break;
            }

            R_tot = R_element + R_ie;

            var U_value = Math.Round(1 / R_tot,4);
            if (R_element == 0) { U_value = 9999; }

            results["UBP Embodied (P)"] = Math.Round(results["UBP Embodied (P)"] * area, 3);
            results["UBP Replacements (P)"] = Math.Round(results["UBP Replacements (P)"] * area, 3);
            results["UBP End of Life (P)"] = Math.Round(results["UBP End of Life (P)"] * area, 3);
            results["PE Total Embodied (kWh oil-eq)"] = Math.Round(results["PE Total Embodied (kWh oil-eq)"] * area, 3);
            results["PE Total Replacements (kWh oil-eq)"] = Math.Round(results["PE Total Replacements (kWh oil-eq)"] * area, 3);
            results["PE Total End of Life (kWh oil-eq)"] = Math.Round(results["PE Total End of Life (kWh oil-eq)"] * area, 3);
            results["PE Renewable Embodied (kWh oil-eq)"] = Math.Round(results["PE Renewable Embodied (kWh oil-eq)"] * area, 3);
            results["PE Renewable Replacements (kWh oil-eq)"] = Math.Round(results["PE Renewable Replacements (kWh oil-eq)"] * area, 3);
            results["PE Renewable End of Life (kWh oil-eq)"] = Math.Round(results["PE Renewable End of Life (kWh oil-eq)"] * area, 3);
            results["PE Non Renewable Embodied (kWh oil-eq)"] = Math.Round(results["PE Non Renewable Embodied (kWh oil-eq)"] * area, 3);
            results["PE Non Renewable Replacements (kWh oil-eq)"] = Math.Round(results["PE Non Renewable Replacements (kWh oil-eq)"] * area, 3);
            results["PE Non Renewable End of Life (kWh oil-eq)"] = Math.Round(results["PE Non Renewable End of Life (kWh oil-eq)"] * area, 3);
            results["Green House Gasses Embodied (kg CO\x2082-eq)"] = Math.Round(results["Green House Gasses Embodied (kg CO\x2082-eq)"] * area, 3);
            results["Green House Gasses Replacements (kg CO\x2082-eq)"] = Math.Round(results["Green House Gasses Replacements (kg CO\x2082-eq)"] * area, 3);
            results["Green House Gasses End of Life (kg CO\x2082-eq)"] = Math.Round(results["Green House Gasses End of Life (kg CO\x2082-eq)"] * area, 3);
            results["U value"] = U_value;
            results["Biogenic Carbon Storage (kg CO₂-eq)"] = Math.Round(results["Biogenic Carbon Storage (kg CO₂-eq)"] * area, 3);

            var resultValues = results.Values.ToList();

            DA.SetDataList(0, results);
            DA.SetDataList(1, resultValues);

            DA.SetData(2, U_value);
            DA.SetData(3, U_value * area);


            //if (funct.Equals("Window")) // Old version of calculations, incorrect models
            //{
            //    DA.SetData(2, 1 / results["U value"]);
            //    DA.SetData(3, (1 / results["U value"]) * area);
            //}
            //else if (funct.Equals("Internal wall") || funct.Equals("External wall") || funct.Equals("Roof") || funct.Equals("Ceiling"))
            //{
            //    DA.SetData(2, results["U value"] + 0.17);
            //    DA.SetData(3, (results["U value"] + 0.17) * area);
            //}
            //else if (funct.Equals("Floor"))
            //{
            //    DA.SetData(2, results["U value"] + 0.13);
            //    DA.SetData(3, (results["U value"] + 0.13) * area);
            //}
            //else if (funct.Equals("Other"))
            //{
            //    DA.SetData(2, results["U value"]);
            //    DA.SetData(3, (results["U value"]) * area);
            //}
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