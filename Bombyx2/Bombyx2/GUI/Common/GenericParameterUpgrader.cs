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
    /// <summary>
    /// Enables users to replace copies of this obsolete component with the default "Data" component.
    /// They can do so by choosing "Solution" > "Upgrade Components..." from the topbar menu.
    /// </summary>
    public class GenericParameterUpgrader : IGH_UpgradeObject
    {
        public DateTime Version => new DateTime(2024, 9, 19);

        public Guid UpgradeFrom => new GenericParameter().ComponentGuid;

        public Guid UpgradeTo => new Param_GenericObject().ComponentGuid;

        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            var oldParam = target as GenericParameter;
            var newParam = new Param_GenericObject();
            var index = document.Objects.IndexOf(oldParam);
            GH_UpgradeUtil.MigrateSources(oldParam, newParam);
            GH_UpgradeUtil.MigrateRecipients(oldParam, newParam);
            newParam.CreateAttributes();
            newParam.Attributes.Pivot = oldParam.Attributes.Pivot;
            newParam.NickName = oldParam.NickName;
            newParam.Attributes.ExpireLayout();
            document.DestroyAttributeCache();
            document.DestroyObjectTable();
            document.RemoveObject(oldParam, false);
            document.AddObject(newParam, false, index);
            return newParam;
        }
    }
}
