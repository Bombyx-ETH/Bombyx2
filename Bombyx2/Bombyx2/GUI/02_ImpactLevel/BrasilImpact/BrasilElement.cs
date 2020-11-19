using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._02_ImpactLevel.BrasilImpact
{
    public class BrasilElement : GH_Component
    {
        public BrasilElement()
          : base("Brasil Elemento",
                 "BR Elemento",
                 "Calcula os impactos de todos os componentes.",
                 "Bombyx 2",
                 "Brasil")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Componente", "Componente", "Propriedades do componente", GH_ParamAccess.list);
            pManager.AddNumberParameter("Área de superfície", "Área de superfície\n(metros quadrados)", "Área de superfície (metros quadrados)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Elemento (texto)", "Elemento (texto)", "Propriedades do elemento (texto)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elemento (valores)", "Elemento (valores)", "Propriedades do elemento (valores)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var component = new List<double>();
            if (!DA.GetDataList(0, component)) { return; }

            var area = 0d;
            if (!DA.GetData(1, ref area)) { return; }

            var valueSets = component.Select((x, i) => new { Index = i, Value = x })
                                     .GroupBy(x => x.Index / 10)
                                     .Select(x => x.Select(v => v.Value).ToList())
                                     .ToList();

            var results = new Dictionary<string, double>
            {
                {"Consumo de recursos (kg) (min)", 0},
                {"Consumo de recursos (kg) (max)", 0},
                {"Consumo de energia (MJ) (min)", 0},
                {"Consumo de energia (MJ) (max)", 0},
                {"Consumo de água (kg) (min)", 0},
                {"Consumo de água (kg) (max)", 0},
                {"Emissão de CO2 (kg CO2-eq) (min)", 0},
                {"Emissão de CO2 (kg CO2-eq) (max)", 0},
                {"Geração de resíduos (kg) (min)", 0},
                {"Geração de resíduos (kg) (max)", 0}
            };

            foreach (var item in valueSets)
            {
                results["Consumo de recursos (kg) (min)"] += item[0] * area;
                results["Consumo de recursos (kg) (max)"] += item[1] * area;
                results["Consumo de energia (MJ) (min)"] += item[2] * area;
                results["Consumo de energia (MJ) (max)"] += item[3] * area;
                results["Consumo de água (kg) (min)"] += item[4] * area;
                results["Consumo de água (kg) (max)"] += item[5] * area;
                results["Emissão de CO2 (kg CO2-eq) (min)"] += item[6] * area;
                results["Emissão de CO2 (kg CO2-eq) (max)"] += item[7] * area;
                results["Geração de resíduos (kg) (min)"] += item[8] * area;
                results["Geração de resíduos (kg) (max)"] += item[9] * area;
            }

            var resultsValues = results.Values.ToList();

            DA.SetDataList(0, results);
            DA.SetDataList(1, resultsValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.brasilElement;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b5351058-1ced-46a0-b6c9-3972d817aa3e"); }
        }
    }
}