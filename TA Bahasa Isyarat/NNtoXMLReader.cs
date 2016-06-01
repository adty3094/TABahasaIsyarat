using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TA_Bahasa_Isyarat
{
    class NNtoXMLReader
    {
        private NeuralNetwork nn;
        int inputLayerCount, hiddenLayerCount, outputLayerCount;
        public int[] chromosom = new int[99];
        public NNtoXMLReader()
        {
        }

        public NNtoXMLReader(NeuralNetwork nn)
        {
            // TODO: Complete member initialization
            this.nn = nn;
        }

        public string read(string filename)
        {
            string algorithms = "";
            using (XmlReader readers = XmlReader.Create(filename))
            {
                while (readers.Read())
                {
                    if (readers.IsStartElement())
                    {
                        if (readers.Name.Equals("Algorithm"))
                        {
                            if (readers.Read())
                            {
                                algorithms = readers.Value;
                            }
                        }
                        if (readers.Name.Equals("OutputNodeCount"))
                        {
                            if (readers.Read())
                            {
                                outputLayerCount = int.Parse(readers.Value);
                            }
                        }
                        if (readers.Name.Equals("HiddenNodeCount"))
                        {
                            if (readers.Read())
                            {
                                hiddenLayerCount = int.Parse(readers.Value);
                            }
                        }
                        if (readers.Name.Equals("InputNodeCount"))
                        {
                            if (readers.Read())
                            {
                                inputLayerCount = int.Parse(readers.Value);
                            }
                        }
                        if (readers.Name.Equals("Chromosom"))
                        {
                            if (readers.Read())
                            {
                                string chromo = readers.Value;
                                chromosom = new int[chromo.Length];
                                for (int i = 0; i < chromo.Length; i++)
                                {
                                    char car = chromo[i];
                                    chromosom[i] = int.Parse(car.ToString());
                                }
                            }
                        }
                    }
                }
            }
            nn.InitNetwork(inputLayerCount, hiddenLayerCount, outputLayerCount);

            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name.Contains("I-"))
                        {
                            string fromStr = reader.Name.Split('-')[1];
                            string toStr = reader.Name.Split('-')[2];
                            int from = Int32.Parse(fromStr) - 1;
                            int to = Int32.Parse(toStr) - 1;
                            float val = new float();
                            //Console.WriteLine(from+"->"+to);
                            if (reader.Read())
                            {
                                val = float.Parse(reader.Value);
                                nn.InputLayer[from].SetWeight(to, val);
                            }
                        }
                        if (reader.Name.Contains("H-"))
                        {
                            string fromStr = reader.Name.Split('-')[1];
                            string toStr = reader.Name.Split('-')[2];
                            int from = Int32.Parse(fromStr) - 1;
                            int to = Int32.Parse(toStr) - 1;
                            float val = new float();
                            //Console.WriteLine(from + "->" + to);
                            if (reader.Read())
                            {
                                val = float.Parse(reader.Value);
                                nn.HiddenLayer[from].SetWeight(to, val);
                            }
                        }
                      
                    }
                }
            }
            return algorithms;
        }

    }
}
