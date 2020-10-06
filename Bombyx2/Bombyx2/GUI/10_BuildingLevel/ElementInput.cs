using System;
using System.Collections.Generic;
using System.Drawing;
using Bombyx2.Data.Access;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._10_BuildingLevel
{
    public class ElementInput : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private string[] ELEMENT = new string[] { "Roof",
                                                  "Interior walls",
                                                  "Partition walls",
                                                  "Windows",
                                                  "Balcony",
                                                  "Technical equipment",
                                                  "Ceilings",
                                                  "Columns",
                                                  "Ext walls above ground",
                                                  "Ext walls under ground",
                                                  "Foundation" };

        public ElementInput()
          : base("Element Input",
                 "Element input",
                 "Select element for building level",
                 "Bombyx 2",
                 "Building level")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Building parameters", "Building parameters", "Building parameters", GH_ParamAccess.list);
            pManager.AddTextParameter("Element type", "Element type", "Element type", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Area (m\xB2)", "Area (m\xB2)", "Area in square meters", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Element values", "Element values", "Element values", GH_ParamAccess.list);
            pManager.AddGenericParameter("List of components", "List of components", "List of components", GH_ParamAccess.list);
            pManager.AddTextParameter("Building inputs", "Building inputs", "Building inputs", GH_ParamAccess.list);
            pManager.AddNumberParameter("-", "---------------------", "-", GH_ParamAccess.item);
            pManager.AddTextParameter("Minimum values", "Minimum values", "Minimum values", GH_ParamAccess.list);
            pManager.AddTextParameter("Maximum values", "Maximum values", "Maximum values", GH_ParamAccess.list);
            pManager.AddTextParameter("Average values", "Average values", "Average values", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            string element = "";

            var inputs = new List<string>();
            if (!DA.GetDataList(0, inputs)) { return; }

            if (Params.Input[0].SourceCount == 1 && Params.Input[1].SourceCount == 0)
            {
                CreateDropDownList(ELEMENT, "Element", 1, 230, -12);
            }

            if (!DA.GetData(1, ref element)) { return; }

            var area = 0d;
            if (!DA.GetData(2, ref area)) { return; }

            var resultsAll = new List<BuildingLevelModel>();
            var resultsMin = new List<BuildingLevelModel>();
            var resultsMax = new List<BuildingLevelModel>();
            var resultsAvg = new List<BuildingLevelModel>();
            var output = new List<BuildingLevelModel>();
            var outputAll = new BuildingLevelModel();
            var outputMin = new BuildingLevelModel();
            var outputMax = new BuildingLevelModel();
            var outputAvg = new BuildingLevelModel();

            switch (element)
            {
                case "Roof":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("Roof", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("Roof", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("Roof", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("Roof", inputs, "avg");
                    break;
                case "Interior walls":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("InteriorWalls", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("InteriorWalls", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("InteriorWalls", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("InteriorWalls", inputs, "avg");
                    break;
                case "Partition walls":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("PartitionWalls", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("PartitionWalls", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("PartitionWalls", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("PartitionWalls", inputs, "avg");
                    break;
                case "Windows":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("Windows", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("Windows", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("Windows", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("Windows", inputs, "avg");
                    break;
                case "Balcony":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("Balcony", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("Balcony", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("Balcony", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("Balcony", inputs, "avg");
                    break;
                case "Technical equipment":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("TechnicalEquipment", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("TechnicalEquipment", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("TechnicalEquipment", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("TechnicalEquipment", inputs, "avg");
                    break;
                case "Ceilings":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("Ceilings", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("Ceilings", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("Ceilings", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("Ceilings", inputs, "avg");
                    break;
                case "Columns":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("Columns", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("Columns", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("Columns", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("Columns", inputs, "avg");
                    break;
                case "Ext walls above ground":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsAboveGround", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsAboveGround", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsAboveGround", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsAboveGround", inputs, "avg");
                    break;
                case "Ext walls under ground":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsUnderGround", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsUnderGround", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsUnderGround", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsUnderGround", inputs, "avg");
                    break;
                case "Foundation":
                    resultsAll = BuildingLevelDataAccess.GetComponentsForBuilding("Foundation", inputs, "all");
                    resultsMin = BuildingLevelDataAccess.GetComponentsForBuilding("Foundation", inputs, "min");
                    resultsMax = BuildingLevelDataAccess.GetComponentsForBuilding("Foundation", inputs, "max");
                    resultsAvg = BuildingLevelDataAccess.GetComponentsForBuilding("Foundation", inputs, "avg");
                    break;
            }

            foreach (var item in resultsAll)
            {
                item.UBP13Embodied *= area;
                item.UBP13EoL *= area;
                item.TotalEmbodied *= area;
                item.TotalEoL *= area;
                item.RenewableEmbodied *= area;
                item.RenewableEoL *= area;
                item.NonRenewableEmbodied *= area;
                item.NonRenewableEoL *= area;
                item.GHGEmbodied *= area;
                item.GHGEoL *= area;
            }

            foreach (var item in resultsAll)
            {
                outputAll.UBP13Embodied += item.UBP13Embodied;
                outputAll.UBP13EoL += item.UBP13EoL;
                outputAll.TotalEmbodied += item.TotalEmbodied;
                outputAll.TotalEoL += item.TotalEoL;
                outputAll.RenewableEmbodied += item.RenewableEmbodied;
                outputAll.RenewableEoL += item.RenewableEoL;
                outputAll.NonRenewableEmbodied += item.NonRenewableEmbodied;
                outputAll.NonRenewableEoL += item.NonRenewableEoL;
                outputAll.GHGEmbodied += item.GHGEmbodied;
                outputAll.GHGEoL += item.GHGEoL;
                outputAll.Uvalue += item.Uvalue;
            }

            foreach (var item in resultsMin)
            {
                item.UBP13Embodied *= area;
                item.UBP13EoL *= area;
                item.TotalEmbodied *= area;
                item.TotalEoL *= area;
                item.RenewableEmbodied *= area;
                item.RenewableEoL *= area;
                item.NonRenewableEmbodied *= area;
                item.NonRenewableEoL *= area;
                item.GHGEmbodied *= area;
                item.GHGEoL *= area;
            }

            foreach (var item in resultsMin)
            {
                outputMin.UBP13Embodied += item.UBP13Embodied;
                outputMin.UBP13EoL += item.UBP13EoL;
                outputMin.TotalEmbodied += item.TotalEmbodied;
                outputMin.TotalEoL += item.TotalEoL;
                outputMin.RenewableEmbodied += item.RenewableEmbodied;
                outputMin.RenewableEoL += item.RenewableEoL;
                outputMin.NonRenewableEmbodied += item.NonRenewableEmbodied;
                outputMin.NonRenewableEoL += item.NonRenewableEoL;
                outputMin.GHGEmbodied += item.GHGEmbodied;
                outputMin.GHGEoL += item.GHGEoL;
                outputMin.Uvalue += item.Uvalue;
            }

            foreach (var item in resultsMax)
            {
                item.UBP13Embodied *= area;
                item.UBP13EoL *= area;
                item.TotalEmbodied *= area;
                item.TotalEoL *= area;
                item.RenewableEmbodied *= area;
                item.RenewableEoL *= area;
                item.NonRenewableEmbodied *= area;
                item.NonRenewableEoL *= area;
                item.GHGEmbodied *= area;
                item.GHGEoL *= area;
            }

            foreach (var item in resultsMax)
            {
                outputMax.UBP13Embodied += item.UBP13Embodied;
                outputMax.UBP13EoL += item.UBP13EoL;
                outputMax.TotalEmbodied += item.TotalEmbodied;
                outputMax.TotalEoL += item.TotalEoL;
                outputMax.RenewableEmbodied += item.RenewableEmbodied;
                outputMax.RenewableEoL += item.RenewableEoL;
                outputMax.NonRenewableEmbodied += item.NonRenewableEmbodied;
                outputMax.NonRenewableEoL += item.NonRenewableEoL;
                outputMax.GHGEmbodied += item.GHGEmbodied;
                outputMax.GHGEoL += item.GHGEoL;
                outputMax.Uvalue += item.Uvalue;
            }

            foreach (var item in resultsAvg)
            {
                item.UBP13Embodied *= area;
                item.UBP13EoL *= area;
                item.TotalEmbodied *= area;
                item.TotalEoL *= area;
                item.RenewableEmbodied *= area;
                item.RenewableEoL *= area;
                item.NonRenewableEmbodied *= area;
                item.NonRenewableEoL *= area;
                item.GHGEmbodied *= area;
                item.GHGEoL *= area;
            }

            foreach (var item in resultsAvg)
            {
                outputAvg.UBP13Embodied += item.UBP13Embodied;
                outputAvg.UBP13EoL += item.UBP13EoL;
                outputAvg.TotalEmbodied += item.TotalEmbodied;
                outputAvg.TotalEoL += item.TotalEoL;
                outputAvg.RenewableEmbodied += item.RenewableEmbodied;
                outputAvg.RenewableEoL += item.RenewableEoL;
                outputAvg.NonRenewableEmbodied += item.NonRenewableEmbodied;
                outputAvg.NonRenewableEoL += item.NonRenewableEoL;
                outputAvg.GHGEmbodied += item.GHGEmbodied;
                outputAvg.GHGEoL += item.GHGEoL;
                outputAvg.Uvalue += item.Uvalue;
            }

            var outputMinText = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", 0d },
                { "UBP13 End of Life (P/m\xB2)", 0d },
                { "PE Total Embodied (kWh oil-eq)", 0d },
                { "PE Total End of Life (kWh oil-eq)", 0d },
                { "PE Renewable Embodied (kWh oil-eq)", 0d },
                { "PE Renewable End of Life (kWh oil-eq)", 0d },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0d },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0d },
                { "Green House Gases Embodied (kg CO\x2082-eq/m\xB2)", 0d },
                { "Green House Gases End of Life (kg CO\x2082-eq/m\xB2)", 0d },
                { "U value (W/m2*K)", 0d }
            };

            foreach (var item in resultsMin)
            {
                outputMinText["UBP13 Embodied (P/m\xB2)"] = Math.Round(outputMinText["UBP13 Embodied (P/m\xB2)"] + item.UBP13Embodied, 2);
                outputMinText["UBP13 End of Life (P/m\xB2)"] = Math.Round(outputMinText["UBP13 End of Life (P/m\xB2)"] + item.UBP13EoL, 2);
                outputMinText["PE Total Embodied (kWh oil-eq)"] = Math.Round(outputMinText["PE Total Embodied (kWh oil-eq)"] + item.TotalEmbodied, 2);
                outputMinText["PE Total End of Life (kWh oil-eq)"] = Math.Round(outputMinText["PE Total End of Life (kWh oil-eq)"] + item.TotalEoL, 2);
                outputMinText["PE Renewable Embodied (kWh oil-eq)"] = Math.Round(outputMinText["PE Renewable Embodied (kWh oil-eq)"] + item.RenewableEmbodied, 2);
                outputMinText["PE Renewable End of Life (kWh oil-eq)"] = Math.Round(outputMinText["PE Renewable End of Life (kWh oil-eq)"] + item.RenewableEoL, 2);
                outputMinText["PE Non Renewable Embodied (kWh oil-eq)"] = Math.Round(outputMinText["PE Non Renewable Embodied (kWh oil-eq)"] + item.NonRenewableEmbodied, 2);
                outputMinText["PE Non Renewable End of Life (kWh oil-eq)"] = Math.Round(outputMinText["PE Non Renewable End of Life (kWh oil-eq)"] + item.NonRenewableEoL, 2);
                outputMinText["Green House Gases Embodied (kg CO\x2082-eq/m\xB2)"] = Math.Round(outputMinText["Green House Gases Embodied (kg CO\x2082-eq/m\xB2)"] + item.GHGEmbodied, 2);
                outputMinText["Green House Gases End of Life (kg CO\x2082-eq/m\xB2)"]  = Math.Round(outputMinText["Green House Gases End of Life (kg CO\x2082-eq/m\xB2)"] + item.GHGEoL, 2);
                outputMinText["U value (W/m2*K)"] = Math.Round(outputMinText["U value (W/m2*K)"] + item.Uvalue, 4);
            }

            var outputMaxText = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", 0d },
                { "UBP13 End of Life (P/m\xB2)", 0d },
                { "PE Total Embodied (kWh oil-eq)", 0d },
                { "PE Total End of Life (kWh oil-eq)", 0d },
                { "PE Renewable Embodied (kWh oil-eq)", 0d },
                { "PE Renewable End of Life (kWh oil-eq)", 0d },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0d },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0d },
                { "Green House Gases Embodied (kg CO\x2082-eq/m\xB2)", 0d },
                { "Green House Gases End of Life (kg CO\x2082-eq/m\xB2)", 0d },
                { "U value (W/m2*K)", 0d }
            };

            foreach (var item in resultsMax)
            {
                outputMaxText["UBP13 Embodied (P/m\xB2)"] = Math.Round(outputMaxText["UBP13 Embodied (P/m\xB2)"] + item.UBP13Embodied, 2);
                outputMaxText["UBP13 End of Life (P/m\xB2)"] = Math.Round(outputMaxText["UBP13 End of Life (P/m\xB2)"] + item.UBP13EoL, 2);
                outputMaxText["PE Total Embodied (kWh oil-eq)"] = Math.Round(outputMaxText["PE Total Embodied (kWh oil-eq)"] + item.TotalEmbodied, 2);
                outputMaxText["PE Total End of Life (kWh oil-eq)"] = Math.Round(outputMaxText["PE Total End of Life (kWh oil-eq)"] + item.TotalEoL, 2);
                outputMaxText["PE Renewable Embodied (kWh oil-eq)"] = Math.Round(outputMaxText["PE Renewable Embodied (kWh oil-eq)"] + item.RenewableEmbodied, 2);
                outputMaxText["PE Renewable End of Life (kWh oil-eq)"] = Math.Round(outputMaxText["PE Renewable End of Life (kWh oil-eq)"] + item.RenewableEoL, 2);
                outputMaxText["PE Non Renewable Embodied (kWh oil-eq)"] = Math.Round(outputMaxText["PE Non Renewable Embodied (kWh oil-eq)"] + item.NonRenewableEmbodied, 2);
                outputMaxText["PE Non Renewable End of Life (kWh oil-eq)"] = Math.Round(outputMaxText["PE Non Renewable End of Life (kWh oil-eq)"] + item.NonRenewableEoL, 2);
                outputMaxText["Green House Gases Embodied (kg CO\x2082-eq/m\xB2)"] = Math.Round(outputMaxText["Green House Gases Embodied (kg CO\x2082-eq/m\xB2)"] + item.GHGEmbodied, 2);
                outputMaxText["Green House Gases End of Life (kg CO\x2082-eq/m\xB2)"] = Math.Round(outputMaxText["Green House Gases End of Life (kg CO\x2082-eq/m\xB2)"] + item.GHGEoL, 2);
                outputMaxText["U value (W/m2*K)"] = Math.Round(outputMaxText["U value (W/m2*K)"] + item.Uvalue, 4);
            }

            var outputAvgText = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", 0d },
                { "UBP13 End of Life (P/m\xB2)", 0d },
                { "PE Total Embodied (kWh oil-eq)", 0d },
                { "PE Total End of Life (kWh oil-eq)", 0d },
                { "PE Renewable Embodied (kWh oil-eq)", 0d },
                { "PE Renewable End of Life (kWh oil-eq)", 0d },
                { "PE Non Renewable Embodied (kWh oil-eq)", 0d },
                { "PE Non Renewable End of Life (kWh oil-eq)", 0d },
                { "Green House Gases Embodied (kg CO\x2082-eq/m\xB2)", 0d },
                { "Green House Gases End of Life (kg CO\x2082-eq/m\xB2)", 0d },
                { "U value (W/m2*K)", 0 }
            };

            foreach (var item in resultsAvg)
            {
                outputAvgText["UBP13 Embodied (P/m\xB2)"] = Math.Round(outputAvgText["UBP13 Embodied (P/m\xB2)"] + item.UBP13Embodied, 2);
                outputAvgText["UBP13 End of Life (P/m\xB2)"] = Math.Round(outputAvgText["UBP13 End of Life (P/m\xB2)"] + item.UBP13EoL, 2);
                outputAvgText["PE Total Embodied (kWh oil-eq)"] = Math.Round(outputAvgText["PE Total Embodied (kWh oil-eq)"] + item.TotalEmbodied, 2);
                outputAvgText["PE Total End of Life (kWh oil-eq)"] = Math.Round(outputAvgText["PE Total End of Life (kWh oil-eq)"] + item.TotalEoL, 2);
                outputAvgText["PE Renewable Embodied (kWh oil-eq)"] = Math.Round(outputAvgText["PE Renewable Embodied (kWh oil-eq)"] + item.RenewableEmbodied, 2);
                outputAvgText["PE Renewable End of Life (kWh oil-eq)"] = Math.Round(outputAvgText["PE Renewable End of Life (kWh oil-eq)"] + item.RenewableEoL, 2);
                outputAvgText["PE Non Renewable Embodied (kWh oil-eq)"] = Math.Round(outputAvgText["PE Non Renewable Embodied (kWh oil-eq)"] + item.NonRenewableEmbodied, 2);
                outputAvgText["PE Non Renewable End of Life (kWh oil-eq)"] = Math.Round(outputAvgText["PE Non Renewable End of Life (kWh oil-eq)"] + item.NonRenewableEoL, 2);
                outputAvgText["Green House Gases Embodied (kg CO\x2082-eq/m\xB2)"] = Math.Round(outputAvgText["Green House Gases Embodied (kg CO\x2082-eq/m\xB2)"] + item.GHGEmbodied, 2);
                outputAvgText["Green House Gases End of Life (kg CO\x2082-eq/m\xB2)"] = Math.Round(outputAvgText["Green House Gases End of Life (kg CO\x2082-eq/m\xB2)"] + item.GHGEoL, 2);
                outputAvgText["U value (W/m2*K)"] = Math.Round(outputAvgText["U value (W/m2*K)"] + item.Uvalue, 4);
            }

            output.Add(outputAll);
            output.Add(outputMin);
            output.Add(outputMax);
            output.Add(outputAvg);

            DA.SetDataList(0, output);
            DA.SetDataList(1, resultsAll);
            DA.SetDataList(2, inputs);
            DA.SetDataList(4, outputMinText);
            DA.SetDataList(5, outputMaxText);
            DA.SetDataList(6, outputAvgText);

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

        protected override Bitmap Icon
        {
            get
            {
                return Icons.elementInput;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("cfda6d8b-9331-4dda-b0d0-d210dfdcc28b"); }
        }
    }
}