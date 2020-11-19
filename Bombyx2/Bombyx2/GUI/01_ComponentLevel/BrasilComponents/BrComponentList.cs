using System;
using System.Collections.Generic;
using System.Drawing;
using Bombyx2.Data.Access;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._01_ComponentLevel.BrasilComponents
{
    public class BrComponentList : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;
        private string[] ComponentList = new string[] { "Fundação", "Parede exterior", "Parede interior", "Laje", "Cobertura" };

        public BrComponentList()
          : base("Brasil Lista de Componentes",
                 "BR Componentes",
                 "Retorna lista de componentes brasileiros do banco de dados.",
                 "Bombyx 2",
                 "Brasil")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Ativar (botão)", "Ativar (botão)", "Conecte um botão ao primeiro parâmetro \nde entrada (Ativar) e clique nele \npara mostrar as entradas.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Grupo de componentes", "Grupo de componentes", "Grupo de componentes", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Componentes", "Componentes", "Componentes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            bool input = false;
            var comps = "";
            if (!DA.GetData(0, ref input)) { return; }

            if (input && Params.Input[1].SourceCount == 0)
            {
                CreateSelectionList(ComponentList, "Grupo de componentes", 1, 200, 80);
            }

            if (!DA.GetData(1, ref comps)) { return; }

            string param;
            switch (comps)
            {
                case "Fundação":
                    param = "FO%";
                    break;
                case "Parede exterior":
                    param = "EW%";
                    break;
                case "Parede interior":
                    param = "IW%";
                    break;
                case "Laje":
                    param = "CE%";
                    break;
                case "Cobertura":
                    param = "RO%";
                    break;
                default:
                    param = "FO%";
                    break;
            }

            DA.SetDataList(0, BrasilComponentsDataAccess.GetBrasilComponentsList(param));
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

        protected override Bitmap Icon
        {
            get
            {
                return Icons.brasilComponentList;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("2124954c-88f9-43ab-8664-f0f6cee939cf"); }
        }
    }
}