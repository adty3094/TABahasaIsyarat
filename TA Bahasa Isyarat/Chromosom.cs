using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class Chromosom
    {
        private int[] bit;

        public int[] Bit
        {
            get { return bit; }
            set { bit = value; }
        }

        public int this[int index]
        {
            get { return bit[index]; }
            set { bit[index] = value; }
        }

        public int Length
        {
            get { return bit.Length; }
        }

        private float fitnessValue = 0;

        public float FitnessValue
        {
            get { return fitnessValue; }
            set { fitnessValue = value; }
        }

        public Chromosom(int[] newBit)
        {
            bit = newBit;
        }
    }
}
