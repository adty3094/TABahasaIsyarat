using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class Translate
    {
        private int[] actualClass;
        private int dec = 0;
        private List<string> kata = new List<string>();
        //private int[] p;
        private int sum;

        public Translate()
        {

        }

        public Translate(int[] p)
        {
            this.actualClass = p;
            
        }

        public Translate(int[] p, int sum)
        {
            actualClass = p;
            this.sum = sum;
        }

        public void Init(ClassificationClass cc)
        {
            int iter = (int)Math.Pow(2, cc.TargetCount);
            List<string> classList = cc.GetClassList();
            for (int i = 0; i < iter; i++)
            {
                if (i < classList.Count)
                    kata.Add(classList[i]);
                else
                    kata.Add("-");
            }
        }

        public string Result(ClassificationClass cc)
        {
            Init(cc);
            int iter = cc.TargetCount;
            int index = 0;
            for (int i = 0, faktor = 1; i < iter; i++, faktor *= 2)
                index += actualClass[i] * faktor;
            return kata[index];
        }
    }
}
