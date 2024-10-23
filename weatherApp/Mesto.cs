using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherApp
{
    public class Mesto
    {
        public string name;
        public string countryCode;
        public Tuple<float, float> coordinates;

        public Mesto(string pName, string PcountryCode, Tuple<float, float> pCoordinates)
        {
            name = pName;
            countryCode = PcountryCode;
            coordinates = pCoordinates;
        }

        public override string ToString()
        {
            return name.ToString() + " " + countryCode.ToString();
        }
    }
}
