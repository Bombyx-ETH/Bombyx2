using Bombyx2.Data.Access;
using Bombyx2.Data.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Bombyx2.GUI._01_Bottom_up
{
    internal class WindowModel
    {
        public double UBP13Emb { get; set; }
        public double UBP13Rep { get; set; }
        public double UBP13Eol { get; set; }
        public double TotalEmb { get; set; }
        public double TotalRep { get; set; }
        public double TotalEol { get; set; }
        public double RenewableEmb { get; set; }
        public double RenewableRep { get; set; }
        public double RenewableEol { get; set; }
        public double NonRenewableEmb { get; set; }
        public double NonRenewableRep { get; set; }
        public double NonRenewableEol { get; set; }
        public double GHGEmb { get; set; }
        public double GHGRep { get; set; }
        public double GHGEol { get; set; }
        public double UValue { get; set; }
        public double BiogenicCarbon { get; set; }
    }

    public class ImpactWindow : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private List<KbobWindowGlazingModel> windowGlazingList = new List<KbobWindowGlazingModel>();
        private List<KbobMaterialModel> windowFramesList = new List<KbobMaterialModel>();
        private List<string> frameNames = new List<string>();
        private List<string> glazingNames = new List<string>();

        public ImpactWindow()
          : base("4: Window impact",
                 "Window impact",
                 "Calculates impacts of PE, GWP, UBP",
                 "Bombyx 2",
                 "1: Bottom-up")
        {
            Message = "Bombyx v" + Config.Version;
            windowGlazingList = KbobMaterialsDataAccess.GetKbobWindowGlazingList();
            windowFramesList = KbobMaterialsDataAccess.GetKbobWindowFramesList();

            foreach (var item in windowGlazingList)
            {
                glazingNames.Add(item.IdKbob + ":" + item.NameEnglish);
            }

            foreach (var item in windowFramesList)
            {
                frameNames.Add(item.IdKbob + ":" + item.NameEnglish);
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {           
            pManager.AddBooleanParameter("Activate (Button)", "Activate (Button)", "Activate (Button)", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Frame material", "Frame material", "Frame material", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Glazing material", "Glazing material", "Glazing material", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Frame percentage", "Frame percentage", "The percentage of the total area covered by the window frame (e.g. 30). Default is 0.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("Reference study period", "RSP (years)", "Manual input", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddIntegerParameter("Reference service life", "RSL (years)", "Manual input", GH_ParamAccess.item);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Surface area (m\xB2)", "Surface area (m\xB2)", "Manual input", GH_ParamAccess.item);
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LCA factors (text)", "LCA factors (text)", "Element Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("LCA factors (values)", "LCA factors (values)", "Element Properties (values)", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value", "U value (W/m2*K)", "U Value", GH_ParamAccess.item);
            pManager.AddTextParameter("LCA frame (text)", "LCA frame (text)", "Frame (text)", GH_ParamAccess.item);
            pManager.AddTextParameter("LCA filling (text)", "LCA filling (text)", "Filling (text)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            var fillingSelect = "";
            var frameSelect = "";       
            var frameInput = 0d;
            var RSP = 0;
            var RSL = 0;
            var area = 0d;
            var repNum = 0d;

            bool input = false;
            if (!DA.GetData(0, ref input)) { return; }

            if (input && Params.Input[1].SourceCount == 0 && Params.Input[2].SourceCount == 0)
            {            
                CreateDropDownList(frameNames.ToArray(), "Window frame", 1, 400, -30);
                CreateDropDownList(glazingNames.ToArray(), "Window glazing", 2, 520, -15);
                ExpireSolution(true);
            }

            if (!DA.GetData(1, ref frameSelect)) { return; }
            if (!DA.GetData(2, ref fillingSelect)) { return; }       
            if (!DA.GetData(3, ref frameInput)) { return; }

            var glazingParam = fillingSelect.Split(':');
            var frameParam = frameSelect.Split(':');
            KbobWindowGlazingModel filling = KbobMaterialsDataAccess.GetKbobWindowGlazing(glazingParam[0]);
            KbobMaterialModel frame = KbobMaterialsDataAccess.GetKbobWindowFrames(frameParam[0]);

            var framePercent = frameInput / 100;
            var fillingPercent = 1 - framePercent;

            if (!DA.GetData(4, ref RSP)) { return; }
            if (!DA.GetData(5, ref RSL)) { return; }
            if (!DA.GetData(6, ref area)) { return; }

            var frameArea = framePercent * area;
            var fillingArea = fillingPercent * area;

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

            var windowFrame = new WindowModel
            {
                UBP13Emb = frame.UBP13Embodied * frameArea,
                UBP13Rep = (frame.UBP13Embodied + frame.UBP13EoL) * repNum * frameArea,
                UBP13Eol = frame.UBP13EoL * frameArea,
                TotalEmb = frame.TotalEmbodied * frameArea,
                TotalRep = (frame.TotalEmbodied + frame.TotalEoL) * repNum * frameArea,
                TotalEol = frame.TotalEoL * frameArea,
                RenewableEmb = frame.RenewableEmbodied * frameArea,
                RenewableRep = (frame.RenewableEmbodied + frame.RenewableEoL) * repNum * frameArea,
                RenewableEol = frame.RenewableEoL * frameArea,
                NonRenewableEmb = frame.NonRenewableEmbodied * frameArea,
                NonRenewableRep = (frame.NonRenewableEmbodied + frame.NonRenewableEoL) * repNum * frameArea,
                NonRenewableEol = frame.NonRenewableEoL * frameArea,
                GHGEmb = frame.GHGEmbodied * frameArea,
                GHGRep = (frame.GHGEmbodied + frame.GHGEoL) * repNum * frameArea,
                GHGEol = frame.GHGEoL * frameArea,
                UValue = 1 / (frame.ThermalCond ?? 0.8) * frameArea,
                BiogenicCarbon = (frame.BiogenicCarbon ?? 0) * frameArea
            };

            var windowGlazing = new WindowModel
            {
                UBP13Emb = filling.UBP13Embodied * fillingArea,
                UBP13Rep = (filling.UBP13Embodied + filling.UBP13EoL) * repNum * fillingArea,
                UBP13Eol = filling.UBP13EoL * fillingArea,
                TotalEmb = filling.TotalEmbodied * fillingArea,
                TotalRep = (filling.TotalEmbodied + filling.TotalEoL) * repNum * fillingArea,
                TotalEol = filling.TotalEoL * fillingArea,
                RenewableEmb = filling.RenewableEmbodied * fillingArea,
                RenewableRep = (filling.RenewableEmbodied + filling.RenewableEoL) * repNum * fillingArea,
                RenewableEol = filling.RenewableEoL * fillingArea,
                NonRenewableEmb = filling.NonRenewableEmbodied * fillingArea,
                NonRenewableRep = (filling.NonRenewableEmbodied + filling.NonRenewableEoL) * repNum * fillingArea,
                NonRenewableEol = filling.NonRenewableEoL * fillingArea,
                GHGEmb = filling.GHGEmbodied * fillingArea,
                GHGRep = (filling.GHGEmbodied + filling.GHGEoL) * repNum * fillingArea,
                GHGEol = filling.GHGEoL * fillingArea,
                UValue = (filling.Uvalue ?? 0.8) * fillingArea,
                BiogenicCarbon = (filling.BiogenicCarbon ?? 0) * fillingArea
            };
            
            var frameDict = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P)", Math.Round(frame.UBP13Embodied * frameArea, 2) },
                { "UBP13 Replacements (P)", Math.Round(((frame.UBP13Embodied + frame.UBP13EoL) * repNum) * frameArea, 2) },
                { "UBP13 End of Life (P)", Math.Round(frame.UBP13EoL * frameArea, 2) },
                { "Total Embodied (kWh oil-eq)", Math.Round(frame.TotalEmbodied * frameArea, 2) },
                { "Total Replacements (kWh oil-eq)", Math.Round(((frame.TotalEmbodied + frame.TotalEoL) * repNum) * frameArea, 2) },
                { "Total End of Life (kWh oil-eq)", Math.Round(frame.TotalEoL * frameArea, 2) },
                { "Renewable Embodied (kWh oil-eq)", Math.Round(frame.RenewableEmbodied * frameArea, 2) },
                { "Renewable Replacements (kWh oil-eq)", Math.Round(((frame.RenewableEmbodied + frame.RenewableEoL) * repNum) * frameArea, 2) },
                { "Renewable End of Life (kWh oil-eq)", Math.Round(frame.RenewableEoL * frameArea, 2) },
                { "Non Renewable Embodied (kWh oil-eq)", Math.Round(frame.NonRenewableEmbodied * frameArea, 2) },
                { "Non Renewable Replacements (kWh oil-eq)", Math.Round(((frame.NonRenewableEmbodied + frame.NonRenewableEoL) * repNum) * frameArea, 2) },
                { "Non Renewable End of Life (kWh oil-eq)", Math.Round(frame.NonRenewableEoL * frameArea, 2) },
                { "Green House Gasses Embodied (kg CO\x2082-eq)", Math.Round(frame.GHGEmbodied * frameArea, 2) },
                { "Green House Gasses Replacements (kg CO\x2082-eq)", Math.Round(((frame.GHGEmbodied + frame.GHGEoL) * repNum) * frameArea, 2) },
                { "Green House Gasses End of Life (kg CO\x2082-eq)", Math.Round(frame.GHGEoL * frameArea, 2) },
                { "U value: (1/Rf)*area(filling)", Math.Round(1 / (frame.ThermalCond ?? 0.8), 4)  * frameArea },
                { "Biogenic Carbon Storage (kg CO₂-eq)", (frame.BiogenicCarbon ?? 0) * frameArea }
            };

            var fillingDict = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P)", filling.UBP13Embodied * fillingArea },
                { "UBP13 Replacements (P)", ((filling.UBP13Embodied + filling.UBP13EoL) * repNum) * fillingArea },
                { "UBP13 End of Life (P)", filling.UBP13EoL * fillingArea },
                { "Total Embodied (kWh oil-eq)", filling.TotalEmbodied * fillingArea },
                { "Total Replacements (kWh oil-eq)", ((filling.TotalEmbodied + filling.TotalEoL) * repNum) * fillingArea },
                { "Total End of Life (kWh oil-eq)", filling.TotalEoL * fillingArea },
                { "Renewable Embodied (kWh oil-eq)", filling.RenewableEmbodied * fillingArea },
                { "Renewable Replacements (kWh oil-eq)", ((filling.RenewableEmbodied + filling.RenewableEoL) * repNum) * fillingArea },
                { "Renewable End of Life (kWh oil-eq)", filling.RenewableEoL * fillingArea },
                { "Non Renewable Embodied (kWh oil-eq)", filling.NonRenewableEmbodied * fillingArea },
                { "Non Renewable Replacements (kWh oil-eq)", ((filling.NonRenewableEmbodied + filling.NonRenewableEoL) * repNum) * fillingArea },
                { "Non Renewable End of Life (kWh oil-eq)", filling.NonRenewableEoL * fillingArea },
                { "Green House Gasses Embodied (kg CO\x2082-eq)", filling.GHGEmbodied * fillingArea },
                { "Green House Gasses Replacements (kg CO\x2082-eq)", ((filling.GHGEmbodied + filling.GHGEoL) * repNum) * fillingArea },
                { "Green House Gasses End of Life (kg CO\x2082-eq)", filling.GHGEoL * fillingArea },
                { "U value: (1/Rg)*area(glasing)", (filling.Uvalue ?? 0.8) * fillingArea },
                { "Biogenic Carbon Storage (kg CO₂-eq)", (filling.BiogenicCarbon ?? 0) * fillingArea }
            };
            
            var window = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P)", Math.Round(windowFrame.UBP13Emb + windowGlazing.UBP13Emb, 2) },
                { "UBP13 Replacements (P)", Math.Round(windowFrame.UBP13Rep + windowGlazing.UBP13Rep, 2) },
                { "UBP13 End of Life (P)", Math.Round(windowFrame.UBP13Eol + windowGlazing.UBP13Eol, 2) },
                { "Total Embodied (kWh oil-eq)", Math.Round(windowFrame.TotalEmb + windowGlazing.TotalEmb, 2) },
                { "Total Replacements (kWh oil-eq)", Math.Round(windowFrame.TotalRep + windowGlazing.TotalRep, 2) },
                { "Total End of Life (kWh oil-eq)", Math.Round(windowFrame.TotalEol + windowGlazing.TotalEol, 2) },
                { "Renewable Embodied (kWh oil-eq)", Math.Round(windowFrame.RenewableEmb + windowGlazing.RenewableEmb, 2) },
                { "Renewable Replacements (kWh oil-eq)", Math.Round(windowFrame.RenewableRep + windowGlazing.RenewableRep, 2) },
                { "Renewable End of Life (kWh oil-eq)", Math.Round(windowFrame.RenewableEol + windowGlazing.RenewableEol, 2) },
                { "Non Renewable Embodied (kWh oil-eq)", Math.Round(windowFrame.NonRenewableEmb + windowGlazing.NonRenewableEmb, 2) },
                { "Non Renewable Replacements (kWh oil-eq)", Math.Round(windowFrame.NonRenewableRep + windowGlazing.NonRenewableRep, 2) },
                { "Non Renewable End of Life (kWh oil-eq)", Math.Round(windowFrame.NonRenewableEol + windowGlazing.NonRenewableEol, 2) },
                { "Green House Gasses Embodied (kg CO\x2082-eq)", Math.Round(windowFrame.GHGEmb + windowGlazing.GHGEmb, 2) },
                { "Green House Gasses Replacements (kg CO\x2082-eq)", Math.Round(windowFrame.GHGRep + windowGlazing.GHGRep, 2) },
                { "Green House Gasses End of Life (kg CO\x2082-eq)", Math.Round(windowFrame.GHGEol + windowGlazing.GHGEol, 2) },
                { "U value(W/m2,K): (Uf+Ug)/area", Math.Round((windowFrame.UValue + windowGlazing.UValue) / area, 4) },
                { "Biogenic Carbon Storage (kg CO₂-eq)", Math.Round(windowFrame.BiogenicCarbon + windowGlazing.BiogenicCarbon, 2) }
            };

            var resultValues = window.Values.ToList();

            DA.SetDataList(0, window);
            DA.SetDataList(1, resultValues);
            DA.SetData(2, window["U value(W/m2,K): (Uf+Ug)/area"]);
            DA.SetDataList(3, frameDict);
            DA.SetDataList(4, fillingDict);
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

        protected override Bitmap Icon => Icons.impactWindow;

        public override Guid ComponentGuid => new Guid("06fc4624-d1ad-4d47-a5e4-b94999331343");
    }
}