using System;
using System.Collections.Generic;
using System.Drawing;
using Bombyx2.Data.Access;
using Bombyx2.GUI.Common;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace Bombyx2.GUI._00_MaterialLevel.KBOB
{
    public class MaterialStart : GH_Component, IGH_VariableParameterComponent
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        private readonly string[] _languagesStrings = {
            "EN: English",
            "DE: German",
            "FR: French",
            "IT: Italian",
            "RU: Russian"
        };

        public MaterialStart()
          : base("Start",
                 "Start",
                 "Set up material preferences",
                 "Bombyx 2",
                 "Materials")
        {
            Hidden = true;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Activate (Button)", "Activate (Button)", "Connect a Button to the first \ninput parameter(Activate) and \nclick it to show inputs.", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Language groups", "Language groups", "Language groups", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("English", "English", "English", GH_ParamAccess.list);
            //pManager.AddTextParameter("2", "2", "Materials", GH_ParamAccess.list);
            //pManager.AddTextParameter("3", "3", "Materials", GH_ParamAccess.list);
            //pManager.AddTextParameter("4", "4", "Materials", GH_ParamAccess.list);
            //pManager.AddTextParameter("5", "5", "Materials", GH_ParamAccess.list);
            //pManager.AddTextParameter("6", "6", "Materials", GH_ParamAccess.list);
        }

        protected override void BeforeSolveInstance()
        {
            DestroyParameter(GH_ParameterSide.Output, 0);
            CreateParameter(GH_ParameterSide.Output, 0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = OnPingDocument();

            bool input = false;
            var group = "";
            if (!DA.GetData(0, ref input)) { return; }

            if (input && Params.Input[1].SourceCount == 0)
            {
                CreateSelectionList(_languagesStrings, "Languages", 1, 330, 75);
            }

            if (!DA.GetData(1, ref group)) { return; }

            var lang = "";

            if (group.Contains("EN"))
            {
                lang = "english";
            }
            else
            {
                lang = "german";
            }

            var newParam = group.Split(':');
            var output = KbobMaterialsDataAccess.GetKbobMaterialsList(newParam[0] + "%");

            DA.SetDataList(0, output);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {

            var param = new GenericParameter();
            ////var param = new Param_GenericObject();
            ////var param = new Param_ScriptVariable();

            param.Name = GH_ComponentParamServer.InventUniqueNickname("xxx", Params.Input);
            param.NickName = param.Name;
            param.Description = "Property Name";
            param.Optional = true;
            param.Access = GH_ParamAccess.item;

            return param;
            //return null;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
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

        protected override Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("02b5647a-4f42-44f4-93dd-4afd4e8b1f81");
    }
}