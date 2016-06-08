using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class FileReader
    {
        public DataSetList ReadFile(string path, ClassificationClass cc)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfo = dirInfo.GetFiles("*.txt");
            cc.Clear();
            DataSetList dsl = new DataSetList();
            foreach(FileInfo file in fileInfo)
            {
                string className = file.Name.Split('.')[1];
                cc.Add(className);
                StreamReader reader = new StreamReader(path + file.Name);
                List<float> attr = new List<float>();

                while (!reader.EndOfStream)
                {
                    attr.Add(float.Parse(reader.ReadLine()));
                }

                DataSet ds = new DataSet(attr.Count);
                ds.ClassName = className;
                for(int i = 0; i < attr.Count; i++)
                {
                    ds[i] = attr[i];
                }
                dsl.Add(ds);
            }
            return dsl;
        }
    }
}
