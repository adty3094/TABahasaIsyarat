using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    public class Features
    {
        public static double Normalize(double x)
        {
            return (x + 2) / 4;
        }

        public static Vector3 FeatureNorm(Vector3 u)
        {
            return new Vector3(Normalize(u.X), Normalize(u.Y), Normalize(u.Z));
        }
    }
}
