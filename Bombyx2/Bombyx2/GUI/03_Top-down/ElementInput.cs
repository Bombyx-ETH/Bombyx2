using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private List<string> Roof = new List<string> {
            "AND eBKP = 'C 4.4' ",
            "AND eBKP = 'F 1.1' ",
            "AND eBKP = 'F 1.2' ",
            "AND eBKP = 'F 1.3' ",
            "AND eBKP = 'G 4.1' ",
            "AND eBKP = 'G 4.2' " };

        private List<string> InteriorWalls = new List<string> {
            "AND eBKP = 'C 2.2' ",
            "AND eBKP = 'G 3.1' " };

        private List<string> PartitionWalls = new List<string> {
            "AND eBKP = 'G 1.1' ",
            "AND eBKP = 'G 1.3' ",
            "AND eBKP = 'G 1.4' ",
            "AND eBKP = 'G 3.1' ",
            "AND eBKP = 'G 3.2' " };

        private List<string> Windows = new List<string> { "AND eBKP = 'E 3.1' " };

        private List<string> Balcony = new List<string> { "AND eBKP = 'C 4.3' " };

        private List<string> TechnicalEquipment = new List<string> {
            "AND eBKP = 'D 1.1' ",
            "AND eBKP = 'D 1.2' ",
            "AND eBKP = 'D 1.5' ",
            "AND eBKP = 'D 1.6' ",
            "AND eBKP = 'D 5.2' ",
            "AND eBKP = 'D 5.3' ",
            "AND eBKP = 'D 5.4' ",
            "AND eBKP = 'D 7.1' ",
            "AND eBKP = 'D 7.2' ",
            "AND eBKP = 'D 7.3' ",
            "AND eBKP = 'D 7.4' ",
            "AND eBKP = 'D 7.5' ",
            "AND eBKP = 'D 8.1' ",
            "AND eBKP = 'D 8.3' ",
            "AND eBKP = 'D 8.4' ",
            "AND eBKP = 'D 8.5' " };

        private List<string> Ceilings = new List<string> {
            "AND eBKP = 'C 4.1' ",
            "AND eBKP = 'G 4.1' ",
            "AND eBKP = 'G 4.2' ",
            "AND eBKP = 'G 2.1' ",
            "AND eBKP = 'G 2.2' " };

        private List<string> Columns = new List<string> { "AND eBKP = 'C 3.2' " };

        private List<string> ExtWallsAboveGround = new List<string> {
            "AND eBKP = 'C 2.1' ",
            "AND eBKP = 'E 2.1' ",
            "AND eBKP = 'E 2.2' ",
            "AND eBKP = 'E 2.3' ",
            "AND eBKP = 'E 2.4' ",
            "AND eBKP = 'E 2.5' ",
            "AND eBKP = 'E 2.6' ",
            "AND eBKP = 'G 3.1' ",
            "AND eBKP = 'G 3.2' " };

        private List<string> ExtWallsUnderGround = new List<string> {
            "AND eBKP = 'C 2.1' ",
            "AND eBKP = 'E 1.1' ",
            "AND eBKP = 'E 1.2' ",
            "AND eBKP = 'E 1.3' " };

        private List<string> Foundation = new List<string> {
            "AND eBKP = 'C 1.1' ",
            "AND eBKP = 'C 1.2' ",
            "AND eBKP = 'C 1.3' ",
            "AND eBKP = 'C 1.4' ",
            "AND eBKP = 'C 1.5' ",
            "AND eBKP = 'G 2.1' ",
            "AND eBKP = 'G 2.2' " };

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
          : base("3.2: Element Input",
                 "Element input",
                 "Select element for building level",
                 "Bombyx 2",
                 "3: Top-down")
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

            switch (element)
            {             
                case "Roof":               
                    foreach (var comp in Roof)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("Roof", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("Roof", inputs, area);
                    break;

                case "Interior walls":
                    foreach (var comp in InteriorWalls)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("InteriorWalls", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("InteriorWalls", inputs, area);
                    break;

                case "Partition walls":
                    foreach (var comp in PartitionWalls)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("PartitionWalls", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("PartitionWalls", inputs, area);
                    break;

                case "Windows":
                    foreach (var comp in Windows)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("Windows", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("Windows", inputs, area);
                    break;

                case "Balcony":
                    foreach (var comp in Balcony)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("Balcony", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("Balcony", inputs, area);
                    break;

                case "Technical equipment":
                    foreach (var comp in TechnicalEquipment)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("TechnicalEquipment", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("TechnicalEquipment", inputs, area);
                    break;

                case "Ceilings":
                    foreach (var comp in Ceilings)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("Ceilings", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("Ceilings", inputs, area);
                    break;

                case "Columns":
                    foreach (var comp in Columns)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("Columns", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("Columns", inputs, area);
                    break;

                case "Ext walls above ground":
                    foreach (var comp in ExtWallsAboveGround)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsAboveGround", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("ExtWallsAboveGround", inputs, area);
                    break;

                case "Ext walls under ground":
                    foreach (var comp in ExtWallsUnderGround)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsUnderGround", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("ExtWallsUnderGround", inputs, area);
                    break;

                case "Foundation":
                    foreach (var comp in Foundation)
                    {
                        resultsMin.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "min"));
                        resultsMax.Add(BuildingLevelDataAccess.GetComponentsForBuilding(null, comp, inputs, "max"));
                        resultsAvg.Add(BuildingLevelDataAccess.GetComponentsForBuilding("Foundation", null, inputs, "avg"));
                    }

                    resultsAll = BuildingLevelDataAccess.GetAllComponentsForBuilding("Foundation", inputs, area);
                    break;
            }

            var outputMin = new BuildingLevelModel();
            var outputMax = new BuildingLevelModel();
            var outputAvg = new BuildingLevelModel();
            var uValMin = new List<double>();
            var uValMax = new List<double>();
            var uValAvg = new List<double>();

            foreach (var item in resultsMin)
            {
                outputMin.UBP13Embodied += (item.UBP13Embodied * area);
                outputMin.UBP13EoL += (item.UBP13EoL * area);
                outputMin.TotalEmbodied += (item.TotalEmbodied * area);
                outputMin.TotalEoL += (item.TotalEoL * area);
                outputMin.RenewableEmbodied += (item.RenewableEmbodied * area);
                outputMin.RenewableEoL += (item.RenewableEoL * area);
                outputMin.NonRenewableEmbodied += (item.NonRenewableEmbodied * area);
                outputMin.NonRenewableEoL += (item.NonRenewableEoL * area);
                outputMin.GHGEmbodied += (item.GHGEmbodied * area);
                outputMin.GHGEoL += (item.GHGEoL * area);
                outputMin.Uvalue = 0;
                uValMin.Add(item.Uvalue);
            }

            foreach (var item in resultsMax)
            {
                outputMax.UBP13Embodied += (item.UBP13Embodied * area);
                outputMax.UBP13EoL += (item.UBP13EoL * area);
                outputMax.TotalEmbodied += (item.TotalEmbodied * area);
                outputMax.TotalEoL += (item.TotalEoL * area);
                outputMax.RenewableEmbodied += (item.RenewableEmbodied * area);
                outputMax.RenewableEoL += (item.RenewableEoL * area);
                outputMax.NonRenewableEmbodied += (item.NonRenewableEmbodied * area);
                outputMax.NonRenewableEoL += (item.NonRenewableEoL * area);
                outputMax.GHGEmbodied += (item.GHGEmbodied * area);
                outputMax.GHGEoL += (item.GHGEoL * area);
                outputMax.Uvalue = 0;
                uValMax.Add(item.Uvalue);
            }        

            foreach (var item in resultsAvg)
            {
                outputAvg.UBP13Embodied += (item.UBP13Embodied * area);
                outputAvg.UBP13EoL += (item.UBP13EoL * area);
                outputAvg.TotalEmbodied += (item.TotalEmbodied * area);
                outputAvg.TotalEoL += (item.TotalEoL * area);
                outputAvg.RenewableEmbodied += (item.RenewableEmbodied * area);
                outputAvg.RenewableEoL += (item.RenewableEoL * area);
                outputAvg.NonRenewableEmbodied += (item.NonRenewableEmbodied * area);
                outputAvg.NonRenewableEoL += (item.NonRenewableEoL * area);
                outputAvg.GHGEmbodied += (item.GHGEmbodied * area);
                outputAvg.GHGEoL += (item.GHGEoL * area);
                outputAvg.Uvalue = 0;
                uValAvg.Add(item.Uvalue);
            }

            var avgMinU = uValMin.Count > 0 ? uValMin.Average() : 0.0;
            var avgMaxU = uValMax.Count > 0 ? uValMax.Average() : 0.0;
            var avgAvgU = uValAvg.Count > 0 ? uValAvg.Average() : 0.0;

            var outputMinText = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", Math.Round(outputMin.UBP13Embodied, 2) },
                { "UBP13 End of Life (P/m\xB2)", Math.Round(outputMin.UBP13EoL, 2) },
                { "PE Total Embodied (kWh oil-eq)", Math.Round(outputMin.TotalEmbodied, 2) },
                { "PE Total End of Life (kWh oil-eq)", Math.Round(outputMin.TotalEoL, 2) },
                { "PE Renewable Embodied (kWh oil-eq)", Math.Round(outputMin.RenewableEmbodied, 2) },
                { "PE Renewable End of Life (kWh oil-eq)", Math.Round(outputMin.RenewableEoL, 2) },
                { "PE Non Renewable Embodied (kWh oil-eq)", Math.Round(outputMin.NonRenewableEmbodied, 2) },
                { "PE Non Renewable End of Life (kWh oil-eq)", Math.Round(outputMin.NonRenewableEoL, 2) },
                { "Green House Gases Embodied (kg CO\x2082-eq)", Math.Round(outputMin.GHGEmbodied, 2) },
                { "Green House Gases End of Life (kg CO\x2082-eq)", Math.Round(outputMin.GHGEoL, 2) },
                { "U value (W/m2*K)", Math.Round(avgMinU, 4) }
            };

            var outputMaxText = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", Math.Round(outputMax.UBP13Embodied, 2) },
                { "UBP13 End of Life (P/m\xB2)", Math.Round(outputMax.UBP13EoL, 2) },
                { "PE Total Embodied (kWh oil-eq)", Math.Round(outputMax.TotalEmbodied, 2) },
                { "PE Total End of Life (kWh oil-eq)", Math.Round(outputMax.TotalEoL, 2) },
                { "PE Renewable Embodied (kWh oil-eq)", Math.Round(outputMax.RenewableEmbodied, 2) },
                { "PE Renewable End of Life (kWh oil-eq)", Math.Round(outputMax.RenewableEoL, 2) },
                { "PE Non Renewable Embodied (kWh oil-eq)", Math.Round(outputMax.NonRenewableEmbodied, 2) },
                { "PE Non Renewable End of Life (kWh oil-eq)", Math.Round(outputMax.NonRenewableEoL, 2) },
                { "Green House Gases Embodied (kg CO\x2082-eq)", Math.Round(outputMax.GHGEmbodied, 2) },
                { "Green House Gases End of Life (kg CO\x2082-eq)", Math.Round(outputMax.GHGEoL, 2) },
                { "U value (W/m2*K)", Math.Round(avgMaxU, 4) }
            };

            var outputAvgText = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", Math.Round(outputAvg.UBP13Embodied, 2) },
                { "UBP13 End of Life (P/m\xB2)", Math.Round(outputAvg.UBP13EoL, 2) },
                { "PE Total Embodied (kWh oil-eq)", Math.Round(outputAvg.TotalEmbodied, 2) },
                { "PE Total End of Life (kWh oil-eq)", Math.Round(outputAvg.TotalEoL, 2) },
                { "PE Renewable Embodied (kWh oil-eq)", Math.Round(outputAvg.RenewableEmbodied, 2) },
                { "PE Renewable End of Life (kWh oil-eq)", Math.Round(outputAvg.RenewableEoL, 2) },
                { "PE Non Renewable Embodied (kWh oil-eq)", Math.Round(outputAvg.NonRenewableEmbodied, 2) },
                { "PE Non Renewable End of Life (kWh oil-eq)", Math.Round(outputAvg.NonRenewableEoL, 2) },
                { "Green House Gases Embodied (kg CO\x2082-eq)", Math.Round(outputAvg.GHGEmbodied, 2) },
                { "Green House Gases End of Life (kg CO\x2082-eq)", Math.Round(outputAvg.GHGEoL, 2) },
                { "U value (W/m2*K)", Math.Round(avgAvgU, 4) }
            };

            var output = new List<BuildingLevelModel>();
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