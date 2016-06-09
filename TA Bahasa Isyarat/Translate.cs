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
        private string binary = "";
        private int sum;

        public int this[int index]
        {
            get { return actualClass[index]; }
        }
        
        public int GetActualClassLength()
        {
            return this.actualClass.Length;
        }
        public Translate()
        {

        }

        public Translate(int[] p)
        {
            this.actualClass = p;
            for(int i = 0; i < p.Length; i++)
            {
                binary += Convert.ToString(p[i]);
            }

            
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
            /*int index = 0;
            for (int i = 0; i < cc.TargetCount; i++)
            {
                int faktor = (int)Math.Pow(2, i);
                index += actualClass[i] * faktor;
            }*/
            string hasil = kata[Convert.ToInt32(binary, 2)];
            return hasil;
        }
    }
}
