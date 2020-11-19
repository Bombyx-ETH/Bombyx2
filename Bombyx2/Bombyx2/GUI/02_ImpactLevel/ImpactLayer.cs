using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._02_ImpactLevel
{
    public class ImpactLayer : GH_Component
    {
        public ImpactLayer()
          : base("Layer impact",
                 "Layer impact",
                 "Calculates impacts of PE, GWP, UBP",
                 "Bombyx 2",
                 "Impacts")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Material", "Material\nproperties", "Material properties", GH_ParamAccess.list);
            pManager.AddNumberParameter("Thickness (meters)", "Thickness (m)", "Number value", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Layer properties (text)", "Layer\nproperties (text)", "Layer property (text)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Layer properties (values)", "Layer properties (values)", "Layer properties (values)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var material = new List<double>();
            if (!DA.GetDataList(0, material)) { return; }
            var thickness = 0d;
            if (!DA.GetData(1, ref thickness)) { return; }

            double AreaDensity;
            if (material[1] == 0)
            {
                AreaDensity = thickness;
            }
            else
            {
                AreaDensity = material[0] * thickness;
            }

            var resistance = 0d;
            if (material[11] != 0 && thickness != 0)
            {
                resistance = thickness / material[11];
            }

            var output = new Dictionary<string, double>
            {
                { "Density (kg/m\xB3)", (double?)material[0] ?? -1 },
                { "UBP13 Embodied * (density * thickness) (P/m\xB2)", Math.Round(material[1] * AreaDensity, 2) },
                { "UBP13 End of Life * (density * thickness) (P/m\xB2)", Math.Round(material[2] * AreaDensity, 2) },
                { "PE Total Embodied * (density * thickness) (kWh oil-eq)", Math.Round(material[3] * AreaDensity, 2) },
                { "PE Total End of Life * (density * thickness) (kWh oil-eq)", Math.Round(material[4] * AreaDensity, 2) },
                { "PE Renewable Embodied * (density * thickness) (kWh oil-eq)", Math.Round(material[5] * AreaDensity, 2) },
                { "PE Renewable End of Life * (density * thickness) (kWh oil-eq)", Math.Round(material[6] * AreaDensity, 2) },
                { "PE Non Renewable Embodied * (density * thickness) (kWh oil-eq)", Math.Round(material[7] * AreaDensity, 2) },
                { "PE Non Renewable End of Life * (density * thickness) (kWh oil-eq)", Math.Round(material[8] * AreaDensity, 2) },
                { "Green House Gases Embodied * (density * thickness) (kg CO\x2082-eq)", Math.Round(material[9] * AreaDensity, 2) },
                { "Green House Gases End of Life * (density * thickness) (kg CO\x2082-eq)", Math.Round(material[10] * AreaDensity, 2) },
                { "R value = (thickness / thermal conductivity)", (double?)resistance ?? -1 }
            };

            var outputValues = output.Values.ToList();

            DA.SetDataList(0, output);
            DA.SetDataList(1, outputValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.impactLayer;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("4e99ae35-68f0-41c4-a72b-549b15f33f09"); }
        }
    }
}