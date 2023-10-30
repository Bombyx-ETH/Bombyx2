using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Bombyx2.GUI._05_Integration
{
    public class BombyxUValue
    {
        public double? UValue_Wall { get; set; }
        public double? UValue_Window { get; set; }
        public double? UValue_Floor { get; set; }
        public double? UValue_Roof { get; set; }

    }

    public class BombyxImpact
    {
        public List<double?> Impact_Construction { get; set; }
        public List<double?> Impact_Systems { get; set; }
        public double? RSP { get; set; }
        public double? NFA { get; set; }
    }

    public class BombyxCarrier
    {
        public string Carrier_Heat { get; set; }
        public double Carrier_HeatLossFactor { get; set; }
        public string Carrier_Electricity { get; set; }
        public string Carrier_PV { get; set; }
    }

    public class BombyxFilter
    {
        public List<int> Filter_Material { get; set; }
        public List<int> Filter_Energy { get; set; }
    }
}
