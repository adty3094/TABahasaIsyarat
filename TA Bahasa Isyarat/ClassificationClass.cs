using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class ClassificationClass
    {
        private List<string> classList = new List<string>();
        private int[] actualClass;

        public List<string> GetClassList()
        {
            return classList;
        }

        public int TargetCount
        {
            get
            {
                int factor = 1;
                while (Math.Pow(2, factor) < classList.Count)
                    factor++;
                return factor;
            }
        }

        public void Add(string className)
        {
            int index = GetIndex(className);
            if (index == -1)
                classList.Add(className);
        }

        public void Clear()
        {
            classList.Clear();
        }

        public int GetIndex(string className)
        {
            return classList.IndexOf(className);
        }

        public int[] GetTarget(int index)
        {
            int[] target = new int[TargetCount];
            int bitCount = target.Length - 1;
            while(index > 0)
            {
                target[bitCount--] = (index % 2);
                index = index / 2;
            }
            return target;
        }

        public int[] GetTarget(string className)
        {
            return GetTarget(GetIndex(className));
        }
    }
}
