using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._02_ImpactLevel.BrasilImpact
{
    public class BrasilBuilding : GH_Component
    {
        public BrasilBuilding()
          : base("Brasil Edifício",
                 "BR Edifício",
                 "Calcula o impacto do edifício.",
                 "Bombyx 2",
                 "Brasil")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Elemento", "Elemento", "Propriedades do elemento", GH_ParamAccess.list);
            pManager.AddNumberParameter("PER (anos)", "Período de estudo\nde referência (anos)", "Período de estudo de referência (anos)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Área líquida", "Área líquida", "Área líquida", GH_ParamAccess.item);
        }
        
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Consumo de recursos", "Consumo de recursos", "Consumo de recursos (kg/m2 a) (min/max)", GH_ParamAccess.item);
            pManager.AddTextParameter("Consumo de energia", "Consumo de energia", "Consumo de energia (MJ/m2 a) (min/max)", GH_ParamAccess.item);
            pManager.AddTextParameter("Consumo de água", "Consumo de água", "Consumo de água (kg/m2 a) (min/max)", GH_ParamAccess.item);
            pManager.AddTextParameter("Emissão de CO2", "Emissão de CO2", "Emissão de CO2 (kg CO2-eq/m2 a) (min/max)", GH_ParamAccess.item);
            pManager.AddTextParameter("Geração de resíduos", "Geração de resíduos", "Geração de resíduos (kg/m2 a) (min/max)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Todos os resultados (valores)", "Todos os resultados (valores)", "Todos os resultados (valores)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var element = new List<double>();
            if (!DA.GetDataList(0, element)) { return; }

            var RSP = 0d;
            if (!DA.GetData(1, ref RSP)) { return; }
            var NFA = 0d;
            if (!DA.GetData(2, ref NFA)) { return; }

            var valueSets = element.Select((x, i) => new { Index = i, Value = x })
                                     .GroupBy(x => x.Index / 10)
                                     .Select(x => x.Select(v => v.Value).ToList())
                                     .ToList();

            var resultsResources = new Dictionary<string, double>
            {
                {"Consumo de recursos (kg/m2 a) (min)", 0},
                {"Consumo de recursos (kg/m2 a) (max)", 0}
            };

            foreach (var item in valueSets)
            {
                resultsResources["Consumo de recursos (kg/m2 a) (min)"] += Math.Round(item[0] / (RSP * NFA), 2);
                resultsResources["Consumo de recursos (kg/m2 a) (max)"] += Math.Round(item[1] / (RSP * NFA), 2);
            }

            var resultsEnergy = new Dictionary<string, double>
            {
                {"Consumo de energia (MJ/m2 a) (min)", 0},
                {"Consumo de energia (MJ/m2 a) (max)", 0}
            };

            foreach (var item in valueSets)
            {
                resultsEnergy["Consumo de energia (MJ/m2 a) (min)"] += Math.Round(item[2] / (RSP * NFA), 2);
                resultsEnergy["Consumo de energia (MJ/m2 a) (max)"] += Math.Round(item[3] / (RSP * NFA), 2);
            }
            var resultsWater = new Dictionary<string, double>
            {
                {"Consumo de água (kg/m2 a) (min)", 0},
                {"Consumo de água (kg/m2 a) (max)", 0}
            };

            foreach (var item in valueSets)
            {
                resultsWater["Consumo de água (kg/m2 a) (min)"] += Math.Round(item[4] / (RSP * NFA), 2);
                resultsWater["Consumo de água (kg/m2 a) (max)"] += Math.Round(item[5] / (RSP * NFA), 2);
            }
            var resultsCO2 = new Dictionary<string, double>
            {
                {"Emissão de CO2 (kg CO2-eq/m2 a) (min)", 0},
                {"Emissão de CO2 (kg CO2-eq/m2 a) (max)", 0}
            };

            foreach (var item in valueSets)
            {
                resultsCO2["Emissão de CO2 (kg CO2-eq/m2 a) (min)"] += Math.Round(item[6] / (RSP * NFA), 2);
                resultsCO2["Emissão de CO2 (kg CO2-eq/m2 a) (max)"] += Math.Round(item[7] / (RSP * NFA), 2);
            }
            var resultsWaste = new Dictionary<string, double>
            {
                {"Geração de resíduos (kg/m2 a) (min)", 0},
                {"Geração de resíduos (kg/m2 a) (max)", 0}
            };

            foreach (var item in valueSets)
            {
                resultsWaste["Geração de resíduos (kg/m2 a) (min)"] += Math.Round(item[8] / (RSP * NFA), 2);
                resultsWaste["Geração de resíduos (kg/m2 a) (max)"] += Math.Round(item[9] / (RSP * NFA), 2);
            }

            var resultValues = resultsResources.Values.ToList();
            resultValues.AddRange(resultsEnergy.Values.ToList());
            resultValues.AddRange(resultsWater.Values.ToList());
            resultValues.AddRange(resultsCO2.Values.ToList());
            resultValues.AddRange(resultsWaste.Values.ToList());

            DA.SetDataList(0, resultsResources);
            DA.SetDataList(1, resultsEnergy);
            DA.SetDataList(2, resultsWater);
            DA.SetDataList(3, resultsCO2);
            DA.SetDataList(4, resultsWaste);
            DA.SetDataList(5, resultValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.brasilBuilding;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fd0315b3-4064-4cf2-afc1-0d5cd1d65573"); }
        }
    }
}