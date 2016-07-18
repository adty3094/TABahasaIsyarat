using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    public class Features
    {
        public static Vector3 VectorFeatureNorm(Vector3 u)
        {
            Vector3 newVector = new Vector3();
            newVector.X = (u.X + 2) / 4;
            newVector.Y = (u.Y + 2) / 4;
            newVector.Z = (u.Z + 2) / 4;
            return newVector;
        }

        public static double AngleFeatureNorm(double n)
        {
            return n / Math.PI;
        }

        public static double DistanceFeatureNorm(double n)
        {
            return n / 3.4641f;
        }
    }
}
