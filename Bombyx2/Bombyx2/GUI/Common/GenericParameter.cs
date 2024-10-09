using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

namespace Bombyx2.GUI.Common
{

    [Obsolete] // 2.0.9 Pedram: Set to obsolete so prev users
               // know their <Data> component is the duplicate
               // and it doesn't show in the GH Primitives tab.
    public class GenericParameter : Param_GenericObject, IDisposable
    {
        private const string ParamAccessKey = "ParamAccess";

        public override bool Read(GH_IReader reader)
        {
            var result = base.Read(reader);
            if (reader.ItemExists(ParamAccessKey))
            {
                try
                {
                    //In case casting produces invalid Access enum
                    Access = (GH_ParamAccess)reader.GetInt32(ParamAccessKey);
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        public override bool Write(GH_IWriter writer)
        {
            var result = base.Write(writer);
            writer.SetInt32(ParamAccessKey, (int)Access);
            return result;
        }

        public void Dispose()
        {
            ClearData();
        }

        public override string Name { get => "Data (from Bombyx)"; set => base.Name = value; }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        public override Guid ComponentGuid => new Guid("ff9faf9b-fda6-4bd0-9e6e-04c850c7d962");
    }
}
