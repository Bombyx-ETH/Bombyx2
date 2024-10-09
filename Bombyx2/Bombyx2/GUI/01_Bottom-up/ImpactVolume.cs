using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Bombyx2.GUI._01_Bottom_up
{
    public class ImpactVolume : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        public ImpactVolume()
          : base("1.3: Volume impact",
                 "Volume impact",
                 "Calculates impacts of PE, GWP, UBP for a given material and volume",
                 "Bombyx 2",
                 "1: Bottom-up")
        {
            Message = "Bombyx v" + Config.Version;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Material", "Material properties", "Material properties", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Reference study period", "RSP (years)", "Reference study period", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference service life", "RSL (years)", "Reference service life", GH_ParamAccess.item);
            pManager.AddNumberParameter("Quantity (m\xB3 or m\xB2)", "Quantity (m\xB3 or m\xB2)", "Square meters for materials defined by surface area (Groups 00, 11, 12, 14, 21). Cubic meters for all other groups.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LCA factors (text)", "LCA factors (text)", "Element Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("LCA factors (values)", "LCA factors (values)", "Element Properties (values)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Component = this;
            GrasshopperDocument = OnPingDocument();

            // 01 - MATERIAL -> LAYER

            var material = new List<double>();
            if (!DA.GetDataList(0, material)) { return; }
            var thickness = 1; 

            double AreaDensity;
            if (material[0] == 0)
            {
                AreaDensity = 1;
            }
            else
            {
                AreaDensity = material[0] * thickness;
            }

            var output_layer = new Dictionary<string, double>
            {
                { "Density (kg/m\xB3)", (double?)material[0] ?? -1 },
                { "UBP Embodied * (density * thickness) (P/m²)", Math.Round(material[1] * AreaDensity, 3) },
                { "UBP End of Life * (density * thickness) (P/m²)", Math.Round(material[2] * AreaDensity, 3) },
                { "PE Total Embodied * (density * thickness) (kWh oil-eq/m²)", Math.Round(material[3] * AreaDensity, 3) },
                { "PE Total End of Life * (density * thickness) (kWh oil-eq/m²)", Math.Round(material[4] * AreaDensity, 3) },
                { "PE Renewable Embodied * (density * thickness) (kWh oil-eq/m²)", Math.Round(material[5] * AreaDensity, 3) },
                { "PE Renewable End of Life * (density * thickness) (kWh oil-eq/m²)", Math.Round(material[6] * AreaDensity, 3) },
                { "PE Non Renewable Embodied * (density * thickness) (kWh oil-eq/m²)", Math.Round(material[7] * AreaDensity, 3) },
                { "PE Non Renewable End of Life * (density * thickness) (kWh oil-eq/m²)", Math.Round(material[8] * AreaDensity, 3) },
                { "Green House Gases Embodied * (density * thickness) (kg CO₂-eq/m²)", Math.Round(material[9] * AreaDensity, 3) },
                { "Green House Gases End of Life * (density * thickness) (kg CO₂-eq/m²)", Math.Round(material[10] * AreaDensity, 3) },
                { "R value = (thickness / thermal conductivity) (m2*K/W)", 0 },
                { "Biogenic Carbon Storage * (density * thickness) (kg CO₂-eq/m²)", material[12] * AreaDensity }
            };

            var outputValues_layer = output_layer.Values.ToList();

            // 02 - LAYER -> COMPONENT

            var layer = outputValues_layer;
            var RSP = 0;
            if (!DA.GetData(1, ref RSP)) { return; }
            var RSL = 0;
            if (!DA.GetData(2, ref RSL)) { return; }

            var valueSets = layer.Select((x, i) => new { Index = i, Value = x })
                                 .GroupBy(x => x.Index / (12 + 1)) // +1 for the BiogenicCarbon
                                 .Select(x => x.Select(v => v.Value).ToList())
                                 .ToList();

            double repNum = 0;
            double tmp = ((double)RSP / (double)RSL) - 1;
            if (RSL != 0 && RSP != 0)
            {
                repNum = Math.Ceiling(tmp);
            }
            if (repNum < 0)
            {
                repNum = 0;
            }
            if (repNum == 0)
            {
                repNum = 0;
            }

            var results = new Dictionary<string, double>
            {
                { "UBP Embodied (P/m²)", 0 },
                { "UBP Replacements (P/m²)", 0 },
                { "UBP End of Life (P/m²)", 0 },
                { "PE Total Embodied (kWh oil-eq/m²)", 0 },
                { "PE Total Replacements (kWh oil-eq/m²)", 0 },
                { "PE Total End of Life (kWh oil-eq/m²)", 0 },
                { "PE Renewable Embodied (kWh oil-eq/m²)", 0 },
                { "PE Renewable Replacements (kWh oil-eq/m²)", 0 },
                { "PE Renewable End of Life (kWh oil-eq/m²)", 0 },
                { "PE Non Renewable Embodied (kWh oil-eq/m²)", 0 },
                { "PE Non Renewable Replacements (kWh oil-eq/m²)", 0 },
                { "PE Non Renewable End of Life (kWh oil-eq/m²)", 0 },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m²)", 0 },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m²)", 0 },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m²)", 0 },
                { "R value", 0 },
                { "Biogenic Carbon Storage (kg CO₂-eq/m²)", 0 }

            };

            foreach (var item in valueSets)
            {
                results["UBP Embodied (P/m²)"] += item[1];
                results["UBP Replacements (P/m²)"] += ((item[1] + item[2]) * repNum);
                results["UBP End of Life (P/m²)"] += item[2];
                results["PE Total Embodied (kWh oil-eq/m²)"] += item[3];
                results["PE Total Replacements (kWh oil-eq/m²)"] += ((item[3] + item[4]) * repNum);
                results["PE Total End of Life (kWh oil-eq/m²)"] += item[4];
                results["PE Renewable Embodied (kWh oil-eq/m²)"] += item[5];
                results["PE Renewable Replacements (kWh oil-eq/m²)"] += ((item[5] + item[6]) * repNum);
                results["PE Renewable End of Life (kWh oil-eq/m²)"] += item[6];
                results["PE Non Renewable Embodied (kWh oil-eq/m²)"] += item[7];
                results["PE Non Renewable Replacements (kWh oil-eq/m²)"] += ((item[7] + item[8]) * repNum);
                results["PE Non Renewable End of Life (kWh oil-eq/m²)"] += item[8];
                results["Green House Gasses Embodied (kg CO\x2082-eq/m²)"] += item[9];
                results["Green House Gasses Replacements (kg CO\x2082-eq/m²)"] += ((item[9] + item[10]) * repNum);
                results["Green House Gasses End of Life (kg CO\x2082-eq/m²)"] += item[10];
                results["R value"] = Math.Round(results["R value"] + item[11], 4);
                results["Biogenic Carbon Storage (kg CO₂-eq/m²)"] += item[12];
            }

            var resultValues_component = results.Values.ToList();

            // 03 - COMPONENT -> ELEMENT

            var component = resultValues_component;
            var volume = 0d;

            if (!DA.GetData(3, ref volume)) { return; }

            var valueSets_element = component.Select((x, i) => new { Index = i, Value = x })
                                     .GroupBy(x => x.Index / (16 + 1)) // +1 for BiogenicCarbon
                                     .Select(x => x.Select(v => v.Value).ToList())
                                     .ToList();

            var results_element = new Dictionary<string, double>
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

            foreach (var item in valueSets_element)
            {
                results_element["UBP Embodied (P)"] += item[0];
                results_element["UBP Replacements (P)"] += item[1];
                results_element["UBP End of Life (P)"] += item[2];
                results_element["PE Total Embodied (kWh oil-eq)"] += item[3];
                results_element["PE Total Replacements (kWh oil-eq)"] += item[4];
                results_element["PE Total End of Life (kWh oil-eq)"] += item[5];
                results_element["PE Renewable Embodied (kWh oil-eq)"] += item[6];
                results_element["PE Renewable Replacements (kWh oil-eq)"] += item[7];
                results_element["PE Renewable End of Life (kWh oil-eq)"] += item[8];
                results_element["PE Non Renewable Embodied (kWh oil-eq)"] += item[9];
                results_element["PE Non Renewable Replacements (kWh oil-eq)"] += item[10];
                results_element["PE Non Renewable End of Life (kWh oil-eq)"] += item[11];
                results_element["Green House Gasses Embodied (kg CO\x2082-eq)"] += item[12];
                results_element["Green House Gasses Replacements (kg CO\x2082-eq)"] += item[13];
                results_element["Green House Gasses End of Life (kg CO\x2082-eq)"] += item[14];
                results_element["U value"] += item[15];
                results_element["Biogenic Carbon Storage (kg CO₂-eq)"] += item[16];
            }

            results_element["UBP Embodied (P)"] = Math.Round(results_element["UBP Embodied (P)"] * volume, 3);
            results_element["UBP Replacements (P)"] = Math.Round(results_element["UBP Replacements (P)"] * volume, 3);
            results_element["UBP End of Life (P)"] = Math.Round(results_element["UBP End of Life (P)"] * volume, 3);
            results_element["PE Total Embodied (kWh oil-eq)"] = Math.Round(results_element["PE Total Embodied (kWh oil-eq)"] * volume, 3);
            results_element["PE Total Replacements (kWh oil-eq)"] = Math.Round(results_element["PE Total Replacements (kWh oil-eq)"] * volume, 3);
            results_element["PE Total End of Life (kWh oil-eq)"] = Math.Round(results_element["PE Total End of Life (kWh oil-eq)"] * volume, 3);
            results_element["PE Renewable Embodied (kWh oil-eq)"] = Math.Round(results_element["PE Renewable Embodied (kWh oil-eq)"] * volume, 3);
            results_element["PE Renewable Replacements (kWh oil-eq)"] = Math.Round(results_element["PE Renewable Replacements (kWh oil-eq)"] * volume, 3);
            results_element["PE Renewable End of Life (kWh oil-eq)"] = Math.Round(results_element["PE Renewable End of Life (kWh oil-eq)"] * volume, 3);
            results_element["PE Non Renewable Embodied (kWh oil-eq)"] = Math.Round(results_element["PE Non Renewable Embodied (kWh oil-eq)"] * volume, 3);
            results_element["PE Non Renewable Replacements (kWh oil-eq)"] = Math.Round(results_element["PE Non Renewable Replacements (kWh oil-eq)"] * volume, 3);
            results_element["PE Non Renewable End of Life (kWh oil-eq)"] = Math.Round(results_element["PE Non Renewable End of Life (kWh oil-eq)"] * volume, 3);
            results_element["Green House Gasses Embodied (kg CO\x2082-eq)"] = Math.Round(results_element["Green House Gasses Embodied (kg CO\x2082-eq)"] * volume, 3);
            results_element["Green House Gasses Replacements (kg CO\x2082-eq)"] = Math.Round(results_element["Green House Gasses Replacements (kg CO\x2082-eq)"] * volume, 3);
            results_element["Green House Gasses End of Life (kg CO\x2082-eq)"] = Math.Round(results_element["Green House Gasses End of Life (kg CO\x2082-eq)"] * volume, 3);
            //results_element["U value"] = Math.Round(1 / results_element["U value"], 4);
            results_element["U value"] = 0d;
            results_element["Biogenic Carbon Storage (kg CO₂-eq)"] = Math.Round(results_element["Biogenic Carbon Storage (kg CO₂-eq)"] * volume, 3);

            var resultValues_element = results_element.Values.ToList();

            DA.SetDataList(0, results_element);
            DA.SetDataList(1, resultValues_element);

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

        protected override Bitmap Icon => Icons.bombyxLogo;

        public override Guid ComponentGuid => new Guid("6328640f-2214-4e06-996a-97d6377bd9eb"); 
    }
}