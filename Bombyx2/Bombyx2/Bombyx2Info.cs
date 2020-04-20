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
                return "Bombyx2";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
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
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
