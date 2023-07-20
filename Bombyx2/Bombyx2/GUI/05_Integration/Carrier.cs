using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Bombyx2.Data.Access;
using Bombyx2.GUI._05_Integration;
using Grasshopper.Kernel.Special;
using System.ComponentModel;
using System.Drawing;

namespace Bombyx2.GUI._05_Integration._02_Carrier
{
    public class Carrier : GH_Component
    {
        IGH_Component Component;

        private string[] EnergyList;

        public string[] EnergyArrayHeat, EnergyArrayElec, EnergyArrayPV;

        public Carrier()
          : base("3: Carrier", "Carrier",
              "Collects the energy carriers and factors for the environmental impact calculations.",
              "Bombyx 2", "4: Integration")
        {
            Message = "Bombyx v" + Config.Version;

            EnergyList = KbobMaterialsDataAccess.GetKbobEnergyList().ToArray();

            List<string> EnergyListHeat = new List<string>();
            List<string> EnergyListElec = new List<string>();
            List<string> EnergyListPV = new List<string>();

            List<int> KBOB_codes_heat = new List<int>() { 41, 42, 43, 44 };
            List<int> KBOB_codes_elec = new List<int>() { 45 };
            List<int> KBOB_codes_pv = new List<int>() { 46 };

            foreach (string entry in EnergyList)
            {
                int KBOB_code = Convert.ToInt32(entry.Substring(0, 2));

                if (KBOB_codes_heat.Contains(KBOB_code)) { EnergyListHeat.Add(entry); }
                else if (KBOB_codes_elec.Contains(KBOB_code)) { EnergyListElec.Add(entry); }
                else if (KBOB_codes_pv.Contains(KBOB_code)) { EnergyListPV.Add(entry); }

            }

            EnergyListHeat.Sort();
            EnergyListElec.Sort();
            EnergyListPV.Sort();

            EnergyArrayHeat = EnergyListHeat.ToArray();
            EnergyArrayElec = EnergyListElec.ToArray();
            EnergyArrayPV = EnergyListPV.ToArray();
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate", "Activate", "Connect a button to generate input lists.", GH_ParamAccess.item);
            pManager[0].Optional = true;

            pManager.AddNumberParameter("Heat Loss %", "Heat Loss %", "Percentage of lost thermal energy in the building for KBOB 41.xxx and 42.xxx. Default value is 0.", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;

            pManager.AddTextParameter("Heat Source", "Heat", "Source for the heat supplied to the building.", GH_ParamAccess.item);
            pManager[2].Optional = true;

            pManager.AddTextParameter("Grid Source", "Grid Elec", "Source for the electricity supplied to the building by the grid.", GH_ParamAccess.item);
            pManager[3].Optional = true;

            pManager.AddTextParameter("On-site Source", "On-site Elec", "KBOB Electricity source for the energy generated on-site (PV or other renewables).", GH_ParamAccess.item);
            pManager[4].Optional = true;

            ExpireSolution(true);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Carrier Object", "Carrier", "Object containing carrier values and factors.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool activate = false;
            if (!DA.GetData(0, ref activate)) { return; };
            if (activate
                && Params.Input[2].SourceCount == 0
                && Params.Input[3].SourceCount == 0
                && Params.Input[4].SourceCount == 0)
            {
                CreateSelectionList(EnergyArrayHeat, "Source: Heat | " + EnergyArrayHeat.Length + " Entries", 2, 580, 50);
                CreateSelectionList(EnergyArrayElec, "Source: Grid Electricity | " + EnergyArrayElec.Length + " Entries", 3, 322, 75);
                CreateSelectionList(EnergyArrayPV, "Source: On-site Electricity | " + EnergyArrayPV.Length + " Entries", 4, 360, 100);
            }

            var Carrier = new BombyxCarrier();

            double Carrier_HeatLossFactor = 0d;
            DA.GetData(1, ref Carrier_HeatLossFactor);

            string Carrier_Heat = null;
            DA.GetData(2, ref Carrier_Heat);

            string Carrier_Electricity = null;
            DA.GetData(3, ref Carrier_Electricity);

            string Carrier_PV = null;
            DA.GetData(4, ref Carrier_PV);

            Carrier.Carrier_Heat = Carrier_Heat;
            Carrier.Carrier_HeatLossFactor = Carrier_HeatLossFactor;
            Carrier.Carrier_Electricity = Carrier_Electricity;
            Carrier.Carrier_PV = Carrier_PV;

            DA.SetData(0, Carrier);
        }

        private void CreateSelectionList(string[] values, string nick, int inputParam, int offsetX, int offsetY)
        {

            Component = this;
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

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C806971C-B6B9-415F-A4B2-5459F51283C8"); }
        }
    }
}