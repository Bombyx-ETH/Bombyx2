using Grasshopper.Kernel;

namespace Bombyx2
{
    public class Bombyx2Icon : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("Bombyx 2", Icons.bombyxLogo);
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("Bombyx 2", 'B');
            return GH_LoadingInstruction.Proceed;
        }
    }
}