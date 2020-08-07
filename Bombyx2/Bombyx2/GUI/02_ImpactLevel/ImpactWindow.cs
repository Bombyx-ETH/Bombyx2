using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._02_ImpactLevel
{
    public class ImpactWindow : GH_Component
    {
        public ImpactWindow()
          : base("Window impact",
                 "Window impact",
                 "Calculates impacts of PE, GWP, UBP",
                 "Bombyx 2",
                 "Impacts")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Frame material", "Frame material", "Frame material", GH_ParamAccess.list);
            pManager.AddNumberParameter("Filling material", "Filling material", "Filling material", GH_ParamAccess.list);
            pManager.AddNumberParameter("Frame percentage", "Frame percentage", "Value", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference study period", "RSP (years)", "Manual input", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Reference service life", "RSL (years)", "Manual input", GH_ParamAccess.item);
            pManager.AddNumberParameter("Surface area (m\xB2)", "Surface area (m\xB2)", "Manual input", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("LCA factors (text)", "LCA factors (text)", "Element Properties (text)", GH_ParamAccess.item);
            pManager.AddNumberParameter("LCA factors (values)", "LCA factors (values)", "Element Properties (values)", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value", "U value (W/K)", "U Value", GH_ParamAccess.item);
            pManager.AddTextParameter("LCA frame (text)", "LCA frame (text)", "Frame (text)", GH_ParamAccess.item);
            pManager.AddTextParameter("LCA filling (text)", "LCA filling (text)", "Filling (text)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var frame = new List<double>();
            var filling = new List<double>();
            var frameInput = 0d;
            var RSP = 0;
            var RSL = 0;
            var area = 0d;
            var repNum = 0d;

            if (!DA.GetDataList(0, frame)) { return; }
            if (!DA.GetDataList(1, filling)) { return; }
            if (!DA.GetData(2, ref frameInput)) { return; }

            var framePercent = frameInput / 100;
            var fillingPercent = 1 - framePercent;

            if (!DA.GetData(3, ref RSP)) { return; }
            if (!DA.GetData(4, ref RSL)) { return; }
            if (!DA.GetData(5, ref area)) { return; }

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
            //fix units
            var frameDict = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", frame[1] * frameArea },
                { "UBP13 Replacements (P/m\xB2)", ((frame[1] + frame[2]) * repNum) * frameArea },
                { "UBP13 End of Life (P/m\xB2)", frame[2] * frameArea },
                { "Total Embodied (kWh oil-eq)", frame[3] * frameArea },
                { "Total Replacements (kWh oil-eq)", ((frame[3] + frame[4]) * repNum) * frameArea },
                { "Total End of Life (kWh oil-eq)", frame[4] * frameArea },
                { "Renewable Embodied (kWh oil-eq)", frame[5] * frameArea },
                { "Renewable Replacements (kWh oil-eq)", ((frame[5] + frame[6]) * repNum) * frameArea },
                { "Renewable End of Life (kWh oil-eq)", frame[6] * frameArea },
                { "Non Renewable Embodied (kWh oil-eq)", frame[7] * frameArea },
                { "Non Renewable Replacements (kWh oil-eq)", ((frame[7] + frame[8]) * repNum) * frameArea },
                { "Non Renewable End of Life (kWh oil-eq)", frame[8] * frameArea },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)", frame[9] * frameArea },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", ((frame[9] + frame[10]) * repNum) * frameArea },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)", frame[10] * frameArea },
                { "U value: (1/Rf)*area(filling)", (1 / frame[11]) * frameArea }
            };

            var fillingDict = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", filling[1] * fillingArea },
                { "UBP13 Replacements (P/m\xB2)", ((filling[1] + filling[2]) * repNum) * fillingArea },
                { "UBP13 End of Life (P/m\xB2)", filling[2] * fillingArea },
                { "Total Embodied (kWh oil-eq)", filling[3] * fillingArea },
                { "Total Replacements (kWh oil-eq)", ((filling[3] + filling[4]) * repNum) * fillingArea },
                { "Total End of Life (kWh oil-eq)", filling[4] * fillingArea },
                { "Renewable Embodied (kWh oil-eq)", filling[5] * fillingArea },
                { "Renewable Replacements (kWh oil-eq)", ((filling[5] + filling[6]) * repNum) * fillingArea },
                { "Renewable End of Life (kWh oil-eq)", filling[6] * fillingArea },
                { "Non Renewable Embodied (kWh oil-eq)", filling[7] * fillingArea },
                { "Non Renewable Replacements (kWh oil-eq)", ((filling[7] + filling[8]) * repNum) * fillingArea },
                { "Non Renewable End of Life (kWh oil-eq)", filling[8] * fillingArea },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)", filling[9] * fillingArea },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", ((filling[9] + filling[10]) * repNum) * fillingArea },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)", filling[10] * fillingArea },
                { "U value: (1/Rg)*area(glasing)", (1 / filling[11]) * fillingArea }
            };

            var window = new Dictionary<string, double>
            {
                { "UBP13 Embodied (P/m\xB2)", frameDict["UBP13 Embodied (P/m\xB2)"] + fillingDict["UBP13 Embodied (P/m\xB2)"] },
                { "UBP13 Replacements (P/m\xB2)", frameDict["UBP13 Replacements (P/m\xB2)"] + fillingDict["UBP13 Replacements (P/m\xB2)"] },
                { "UBP13 End of Life (P/m\xB2)", frameDict["UBP13 End of Life (P/m\xB2)"] + fillingDict["UBP13 End of Life (P/m\xB2)"] },
                { "Total Embodied (kWh oil-eq)", frameDict["Total Embodied (kWh oil-eq)"] + fillingDict["Total Embodied (kWh oil-eq)"] },
                { "Total Replacements (kWh oil-eq)", frameDict["Total Replacements (kWh oil-eq)"] + fillingDict["Total Replacements (kWh oil-eq)"] },
                { "Total End of Life (kWh oil-eq)", frameDict["Total End of Life (kWh oil-eq)"] + fillingDict["Total End of Life (kWh oil-eq)"] },
                { "Renewable Embodied (kWh oil-eq)", frameDict["Renewable Embodied (kWh oil-eq)"] + fillingDict["Renewable Embodied (kWh oil-eq)"] },
                { "Renewable Replacements (kWh oil-eq)", frameDict["Renewable Replacements (kWh oil-eq)"] + fillingDict["Renewable Replacements (kWh oil-eq)"] },
                { "Renewable End of Life (kWh oil-eq)", frameDict["Renewable End of Life (kWh oil-eq)"] + fillingDict["Renewable End of Life (kWh oil-eq)"] },
                { "Non Renewable Embodied (kWh oil-eq)", frameDict["Non Renewable Embodied (kWh oil-eq)"] + fillingDict["Non Renewable Embodied (kWh oil-eq)"] },
                { "Non Renewable Replacements (kWh oil-eq)", frameDict["Non Renewable Replacements (kWh oil-eq)"] + fillingDict["Non Renewable Replacements (kWh oil-eq)"] },
                { "Non Renewable End of Life (kWh oil-eq)", frameDict["Non Renewable End of Life (kWh oil-eq)"] + fillingDict["Non Renewable End of Life (kWh oil-eq)"] },
                { "Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)", frameDict["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)"] + fillingDict["Green House Gasses Embodied (kg CO\x2082-eq/m\xB2)"] },
                { "Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)", frameDict["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)"] + fillingDict["Green House Gasses Replacements (kg CO\x2082-eq/m\xB2)"] },
                { "Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)", frameDict["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)"] + fillingDict["Green House Gasses End of Life (kg CO\x2082-eq/m\xB2)"] },
                { "U value: (Uf+Ug)/area", (frameDict["U value: (1/Rf)*area(filling)"] + fillingDict["U value: (1/Rg)*area(glasing)"]) / area }
            };

            var resultValues = window.Values.ToList();

            DA.SetDataList(0, window);
            DA.SetDataList(1, resultValues);
            DA.SetData(2, window["U value: (Uf+Ug)/area"]);
            DA.SetDataList(3, frameDict);
            DA.SetDataList(4, fillingDict);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.impactWindow;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b8c26af5-1240-48b7-a019-32db17513517"); }
        }
    }
}