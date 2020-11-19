using System;
using System.Collections.Generic;
using System.Linq;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;

namespace Bombyx2.GUI._01_ComponentLevel.BrasilComponents
{
    public class BrComponent : GH_Component
    {
        public BrComponent()
          : base("Brasil Componente",
                 "BR Componente",
                 "Retorna os valores de todos os materiais para o componente selecionado.",
                 "Bombyx 2",
                 "Brasil")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Componente", "Componente", "Componente", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Conteúdo de materiais", "Conteúdo de materiais", "Conteúdo de materiais", GH_ParamAccess.list);
            pManager.AddTextParameter("Componente (texto)", "Componente (texto)", "Propriedades do componente (texto)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Componente (valores)", "Componente (valores)", "Propriedades do componente (valores)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var input = "";
            if (!DA.GetData(0, ref input)) { return; }

            var splitted = input.Split('-');
            var mats = BrasilComponentsDataAccess.GetBrasilComponents(splitted[1].Trim());

            var materialList = new List<string>();
            var component = new Dictionary<string, double>
            {
                {"Consumo de recursos (kg/m2) (min)", 0},
                {"Consumo de recursos (kg/m2) (max)", 0},
                {"Consumo de energia (MJ/m2) (min)", 0},
                {"Consumo de energia (MJ/m2) (max)", 0},
                {"Consumo de água (kg/m2) (min)", 0},
                {"Consumo de água (kg/m2) (max)", 0},
                {"Emissão de CO2 (kg CO2-eq/m2) (min)", 0},
                {"Emissão de CO2 (kg CO2-eq/m2) (max)", 0},
                {"Geração de resíduos (kg/m2) (min)", 0},
                {"Geração de resíduos (kg/m2) (max)", 0}
            };

            materialList.Add("O componente selecionado contém:");
            foreach (var item in mats)
            {
                materialList.Add(item.NameBrasil + ": " + item.Amount.ToString() + " " + item.Unit);

                component["Consumo de recursos (kg/m2) (min)"] += item.ResourcesConsumMin * item.Amount;
                component["Consumo de recursos (kg/m2) (max)"] += item.ResourcesConsumMax * item.Amount;
                component["Consumo de energia (MJ/m2) (min)"] += item.EnergyConsumMin * item.Amount;
                component["Consumo de energia (MJ/m2) (max)"] += item.EnergyConsumMax * item.Amount;
                component["Consumo de água (kg/m2) (min)"] += item.WaterConsumMin * item.Amount;
                component["Consumo de água (kg/m2) (max)"] += item.WaterConsumMax * item.Amount;
                component["Emissão de CO2 (kg CO2-eq/m2) (min)"] += item.CO2Min * item.Amount;
                component["Emissão de CO2 (kg CO2-eq/m2) (max)"] += item.CO2Max * item.Amount;
                component["Geração de resíduos (kg/m2) (min)"] += item.WasteGenMin * item.Amount;
                component["Geração de resíduos (kg/m2) (max)"] += item.WasteGenMax * item.Amount;
            }

            DA.SetDataList(0, materialList);

            var componentValues = component.Values.ToList();

            DA.SetDataList(1, component);
            DA.SetDataList(2, componentValues);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Icons.brasilComponent;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("bc929b89-7479-4b02-a533-8583e9ce61c6"); }
        }
    }
}