using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Bombyx2
{
    public class Bombyx2Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Bombyx 2";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                return Icons.bombyxLogo;
            }
        }
        public override string Description
        {
            get
            {
                return "Real-time Life Cycle Assessment – Bombyx; version 2";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("5af956a8-f831-4691-9d71-621053263843");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "ETH Zurich";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "bombyx@ibi.baug.ethz.ch";
            }
        }
    }
}
