using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bombyx2.GUI._04_HeatingDemand
{
    public class CatDataModel
    {
        public string Name { get; set; }
        public double theta_i { get; set; }
        public double area_pers { get; set; }
        public double Heatinput_pers { get; set; }
        public double presence_time { get; set; }
        public double demand_el_annual { get; set; }
        public double el_reduction { get; set; }
        public double q_thO { get; set; }
        public double Q_H_liO { get; set; }
        public double DeltaQ_H__li { get; set; }
        public double Heat_input_standard_useage { get; set; }
        public double Ph_li { get; set; }

        public CatDataModel(string name, double theta_i, double area_pers, double heatinput_pers, 
            double presence_time, double demand_el_annual, double el_reduction, double q_thO, 
            double q_H_liO, double deltaQ_H__li, double heat_input_standard_useage, double ph_li)
        {
            Name = name;
            this.theta_i = theta_i;
            this.area_pers = area_pers;
            Heatinput_pers = heatinput_pers;
            this.presence_time = presence_time;
            this.demand_el_annual = demand_el_annual;
            this.el_reduction = el_reduction;
            this.q_thO = q_thO;
            Q_H_liO = q_H_liO;
            DeltaQ_H__li = deltaQ_H__li;
            Heat_input_standard_useage = heat_input_standard_useage;
            Ph_li = ph_li;
        }
    }

    public class HeatingDemand : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        double util_param = 1.0;
        double ref_time_const = 15.0;

        List<CatDataModel> cat_data = new List<CatDataModel>();

        private string[] BuildingCategory = new string[] { "Multifamily house", 
                                                           "Singlefamily house", 
                                                           "Administration", 
                                                           "School", 
                                                           "Commersial", 
                                                           "Restaurant", 
                                                           "Meeting venue", 
                                                           "Hospital", 
                                                           "Industrial", 
                                                           "Warehouse", 
                                                           "Sport facility", 
                                                           "Indoor pool" };

        private string[] BuildingCase = new string[] { "New building", "Renovation" };

        private string[] ConstructionMethod = new string[] { "Light", "Medium", "Heavy" };

        private string[] RegulationType = new string[] { " One-room regulation", "Refernece room regulation", "All Cases" };

        private string[] VentilationType = new string[] { "Natural", "Mechanical" };

        private string[] HeatRecovery = new string[] { "On", "Off" };

        public HeatingDemand()
          : base("Heating demand",
                 "Heating demand",
                 "Calculates heating demand.",
                 "Bombyx 2",
                 "4: Heating demand")
        {
            Message = "Bombyx v" + Config.Version;

            cat_data.AddRange(new List<CatDataModel>
            {
                new CatDataModel("Wohnen MFH I", 20, 40, 70, 12, 28, 0.7, 0.7, 13, 15, 3.1, 20),
                new CatDataModel("Wohnen EFH II", 20, 60, 70, 12, 22, 0.7, 0.7, 16, 15, 2.4, 25),
                new CatDataModel("Verwaltung III", 20, 20, 80, 6, 22, 0.9, 0.7, 13, 15, 3.3, 25),
                new CatDataModel("Schulen IV", 20, 10, 70, 4, 11, 0.9, 0.7, 14, 15, 2.3, 20),
                new CatDataModel("Verkauf V", 20, 10, 90, 4, 33, 0.8, 0.7, 7, 14, 0, 0),
                new CatDataModel("Restaurants VI", 20, 5, 100, 3, 33, 0.7, 1.2, 16, 15, 0, 0),
                new CatDataModel("Versammlungslokale VII", 20, 5, 80, 3, 17, 0.8, 1.0, 18, 15, 0, 0),
                new CatDataModel("Spitaler VIII", 22, 30, 80, 16, 28, 0.7, 1.0, 18, 17, 0, 0),
                new CatDataModel("Industrie IX", 18, 20, 100, 6,17, 0.9, 0.7, 10, 14, 0, 0),
                new CatDataModel("Lager X", 18, 100, 100, 6,6, 0.9, 0.3, 14, 14, 0, 0),
                new CatDataModel("Sportbauten XI", 18, 20, 100, 6,6, 0.9, 0.7, 16, 14, 0, 0),
                new CatDataModel("Hallenbader XII", 28, 20, 60, 4, 56, 0.7, 0.7, 15, 18, 0, 0)
            });
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {         
            pManager.AddNumberParameter("UA", "UA", "UA", GH_ParamAccess.item); //UA: A sum of UA of all the components           
            pManager.AddNumberParameter("Opaque elements", "Opaque elements", "Opaque elements", GH_ParamAccess.item); //Opaque_elements_A: Insert the areas of the components           
            pManager.AddGenericParameter("Glazed elements", "Glazed elements", "Glazed elements", GH_ParamAccess.item); //Glazed_elements: fgdl           
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Building category", "Building category", "Building category", GH_ParamAccess.item); //Building category: (default=1)           
            pManager.AddNumberParameter("City climate", "City climate", "City climate", GH_ParamAccess.item); //Select city for climate data           
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Building case", "Building case", "Building case", GH_ParamAccess.item, 1); //building_case: (default = 1) New building = 1 Renovation = 2          
            pManager[6].Optional = true;
            pManager.AddNumberParameter("Construction method", "Construction method", "Construction method", GH_ParamAccess.item); //construction_method: Very light=1, Light=2, Medium=3, Heavy=4           
            pManager.AddNumberParameter("Thermal bridges", "Thermal bridges", "Thermal bridges", GH_ParamAccess.item); //Insert percentage of thermal bridges in decimals (example:5%=0.05)
            pManager[7].Optional = true;           
            pManager.AddNumberParameter("Regulation type", "Regulation type", "Regulation type", GH_ParamAccess.item); //regulation_type:(default=1) One-room temperature regulation = 1 Refernece room temperature regulation = 2 All Cases = 3            
            pManager[8].Optional = true;
            pManager.AddNumberParameter("Ventilation type", "Ventilation type", "Ventilation type", GH_ParamAccess.item); //ventilation_type: (default=1) Natural=1 Mechanical=2         
            pManager.AddNumberParameter("Air flow mech", "Air flow mech", "Air flow mech", GH_ParamAccess.item); //air_flow_mech:Number of air flow (mechanical ventilation only) in m3/m2h        
            pManager[10].Optional = true;
            pManager.AddNumberParameter("Heat recovery", "Heat recovery", "Heat recovery", GH_ParamAccess.item); //heat_recovery: optional(if mech is on) on=1, off=2 (default)         
            pManager.AddNumberParameter("Heat exch efficiency", "Heat exch efficiency", "Heat exch efficiency", GH_ParamAccess.item); //heat_exch_efficiency: efficiency of heat exchanger - default = 0.8
            pManager.AddNumberParameter("Air to earth", "Air to earth", "Air to earth", GH_ParamAccess.item);           
            pManager.AddNumberParameter("Room number", "Room number", "Room number", GH_ParamAccess.item, 1); //room_num: Optional.Number of living and sleeping rooms (default=1)   
            pManager.AddNumberParameter("B wall", "B wall", "B wall", GH_ParamAccess.item); //b_wall: The b value for underground walls           
            pManager.AddNumberParameter("B floor", "B floor", "B floor", GH_ParamAccess.item); //b_floor:The b value for underground floors            
            pManager.AddNumberParameter("A_e", "A_e", "A_e", GH_ParamAccess.item); //A_e: Insert energy reference area (m2)
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Transmission losses", "Transmission losses", "Transmission losses", GH_ParamAccess.item);
            pManager.AddNumberParameter("Ventilation losses", "Ventilation losses", "Ventilation losses", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heating demand", "Heating demand", "Heating demand", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupancy gains", "Occupancy gains", "Occupancy gains", GH_ParamAccess.item);
            pManager.AddNumberParameter("Electricity gains", "Electricity gains", "Electricity gains", GH_ParamAccess.item);
            pManager.AddNumberParameter("Solar gains", "Solar gains", "Solar gains", GH_ParamAccess.item);
            pManager.AddNumberParameter("Utilized gains", "Utilized gains", "Utilized gains", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            var ua = 0d;
            if (!DA.GetData(0, ref ua)) { return; }

            if (Params.Input[0].SourceCount == 1 && 
                Params.Input[3].SourceCount == 0 &&
                Params.Input[5].SourceCount == 0 &&
                Params.Input[6].SourceCount == 0 &&
                Params.Input[7].SourceCount == 0 &&
                Params.Input[8].SourceCount == 0 &&
                Params.Input[10].SourceCount == 0)
            {
                CreateSelectionList(BuildingCategory, "Building category", 3, 200, 50);
                CreateSelectionList(BuildingCase, "Building case", 5, 200, 50);
                CreateSelectionList(ConstructionMethod, "Construction method", 6, 200, 50);
                CreateSelectionList(RegulationType, "Regulation type", 7, 200, 50);
                CreateSelectionList(VentilationType, "Ventilation type", 8, 200, 50);
                CreateSelectionList(HeatRecovery, "Heat recovery", 10, 200, 50);
                ExpireSolution(true);
            }

            var opaque = 0d;
            if (!DA.GetData(1, ref opaque)) { return; }
            var glazed = new GlazedElements();
            if (!DA.GetData(2, ref glazed)) { return; }
            var category = 0d;
            if (!DA.GetData(3, ref category)) { return; }           
            var city = 0d;
            if (!DA.GetData(4, ref city)) { return; }
            var buildCase = 0d;
            if (!DA.GetData(5, ref buildCase)) { return; }
            var construction = 0d;
            if (!DA.GetData(6, ref construction)) { return; }
            var bridges = 0d;
            if (!DA.GetData(7, ref bridges)) { return; }
            var regulation = "";
            if (!DA.GetData(8, ref regulation)) { return; }
            var ventilation = 0d;
            if (!DA.GetData(9, ref ventilation)) { return; }
            var airFlow = 0d;
            if (!DA.GetData(10, ref airFlow)) { return; }
            var heat = 0d;
            if (!DA.GetData(11, ref heat)) { return; }
            var efficiency = 0d;
            if (!DA.GetData(12, ref efficiency)) { return; }
            var earth = 0d;
            if (!DA.GetData(13, ref earth)) { return; }
            var roomNum = 0d;
            if (!DA.GetData(14, ref roomNum)) { return; }
            var bWall = 0d;
            if (!DA.GetData(15, ref bWall)) { return; }
            var bFloor = 0d;
            if (!DA.GetData(16, ref bFloor)) { return; }
            var ae = 0d;
            if (!DA.GetData(17, ref ae)) { return; }
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

        protected override Bitmap Icon => Icons.heatingDemand;

        public override Guid ComponentGuid => new Guid("c3527bf7-7a5c-43cb-9bdf-da319d40bf09"); 
    }
}