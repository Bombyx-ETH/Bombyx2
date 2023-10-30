using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._10_BuildingLevel
{
    public class BuildingInput : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private string[] SIZE_VALUES = new string[] { "Low rise", "Mid Rise", "High Rise" };
        private string[] USAGE_VALUES = new string[] { "Resi SFH", "Resi MFH", "Office" };
        private string[] ENERGY_VALUES = new string[] { "Minimum", "Above Standard", "PassivHaus" };
        private string[] STRUCTUAL_VALUES = new string[] { "Concrete", "Masonry", "Timber", "Steel" };

        public BuildingInput()
          : base("3.1: Building Input",
                 "Building input",
                 "Select inputs for building level",
                 "Bombyx 2",
                 "3: Top-down")
        {
            Message = "Connect a Button to the first \ninput parameter(Activate) and \nclick it to show inputs.";          
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate (Button)", "Activate (Button)", "Activate (Button)", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Building size", "Building size", "Building size", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Building usage", "Building usage", "Building usage", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddTextParameter("Energy performance", "Energy performance", "Energy performance", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddTextParameter("Structural material", "Structural material", "Structural material", GH_ParamAccess.item);
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Building parameters", "Building\nparameters", "Building parameters", GH_ParamAccess.list);
            //pManager.AddTextParameter("Roof (C4.4., F1., G4.)", "Roof", "Roof (C4.4., F1., G4.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Interior Walls (C2.2., G3.)", "Interior Walls", "Interior Walls (C2.2., G3.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Partition Walls (G1., G3.)", "Partition Walls", "Partition Walls (G1., G3.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Windows (E3.)", "Windows", "Windows (E3.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Balcony (C4.3.)", "Balcony", "Balcony (C4.3.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Technical Equipment (D1., D5.2, D5.3/D5.4., D7., D8.)", "Technical Equipment", "Technical Equipment (D1., D5.2, D5.3/D5.4., D7., D8.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Ceilings (C4.1., G4., G2.)", "Ceilings", "Ceilings (C4.1., G4., G2.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Columns (C3.)", "Columns", "Columns (C3.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Ext Walls above ground (C2.1B., E2., G3.)", "Ext Walls above ground", "Ext Walls above ground (C2.1B., E2., G3.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Ext Walls under ground (C2.1A., E1.)", "Ext Walls under ground", "Ext Walls under ground (C2.1A., E1.)", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Foundation (C1., G2.)", "Foundation", "Foundation (C1., G2.)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            string size = "";
            string usage = "";
            string energy = "";
            string structural = "";

            bool input = false;
            if (!DA.GetData(0, ref input)) { return; }

            if (input &&
                Params.Input[1].SourceCount == 0 &&
                Params.Input[2].SourceCount == 0 &&
                Params.Input[3].SourceCount == 0 &&
                Params.Input[4].SourceCount == 0)
            {
                CreateDropDownList(SIZE_VALUES, "Building size", 1, 200, -20);
                CreateDropDownList(USAGE_VALUES, "Building usage", 2, 200, -10);
                CreateDropDownList(ENERGY_VALUES, "Energy preference", 3, 230, 0);
                CreateDropDownList(STRUCTUAL_VALUES, "Structural material", 4, 195, 10);
                Message = "Component activated.";
            }

            if (!DA.GetData(1, ref size)) { return; }
            if (!DA.GetData(2, ref usage)) { return; }
            if (!DA.GetData(3, ref energy)) { return; }
            if (!DA.GetData(4, ref structural)) { return; }

            var inputs = new List<string>();
            inputs.Add(size);
            inputs.Add(usage);
            inputs.Add(energy); 
            inputs.Add(structural);

            DA.SetDataList(0, inputs);
            //DA.SetDataList(0, BuildingLevelDataAccess.GetComponentsForBuilding("Roof", inputs));
            //DA.SetDataList(1, BuildingLevelDataAccess.GetComponentsForBuilding("InteriorWalls", inputs));
            //DA.SetDataList(2, BuildingLevelDataAccess.GetComponentsForBuilding("PartitionWalls", inputs));
            //DA.SetDataList(3, BuildingLevelDataAccess.GetComponentsForBuilding("Windows", inputs));
            //DA.SetDataList(4, BuildingLevelDataAccess.GetComponentsForBuilding("Balcony", inputs));
            //DA.SetDataList(5, BuildingLevelDataAccess.GetComponentsForBuilding("TechnicalEquipment", inputs));
            //DA.SetDataList(6, BuildingLevelDataAccess.GetComponentsForBuilding("Ceilings", inputs));
            //DA.SetDataList(7, BuildingLevelDataAccess.GetComponentsForBuilding("Columns", inputs));
            //DA.SetDataList(8, BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsAboveGround", inputs));
            //DA.SetDataList(9, BuildingLevelDataAccess.GetComponentsForBuilding("ExtWallsUnderGround", inputs));
            //DA.SetDataList(10, BuildingLevelDataAccess.GetComponentsForBuilding("Foundation", inputs));
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
                return Icons.buildingInput;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d52bdab9-93fe-47d8-adf9-742182576ad5"); }
        }
    }
}