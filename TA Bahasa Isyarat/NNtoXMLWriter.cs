using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TA_Bahasa_Isyarat
{
    class NNtoXMLWriter
    {
        private NeuralNetwork nn;
        private float avgError;

        public NNtoXMLWriter(NeuralNetwork nn)
        {
            // TODO: Complete member initialization
            this.nn = nn;
        }

        public NNtoXMLWriter(NeuralNetwork nn, float avgError)
        {
            // TODO: Complete member initialization
            this.nn = nn;
            this.avgError = avgError;
        }

        public void Write(string filename, string algo, int[] fitChrom)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
            using (StreamWriter stw = new StreamWriter(fileStream))
            using (XmlTextWriter writer = new XmlTextWriter(stw))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument();
                writer.WriteStartElement("NeuralNetwork");
                writer.WriteElementString("Algorithm", algo);
                writer.WriteStartElement("Layer");

                writer.WriteStartElement("Input");
                writer.WriteElementString("InputNodeCount", nn.InputLayer.Count.ToString());

                writer.WriteStartElement("Weight");
                for (int i = 0; i < nn.InputLayer.Count; i++)
                {
                    for (int j = 0; j < nn.InputLayer[i].NextNodeCount; j++)
                    {
                        writer.WriteElementString("I-" + (i + 1).ToString() + "-" + (j + 1).ToString(), nn.InputLayer[i].GetWeight(j).ToString());
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Hidden");
                writer.WriteElementString("HiddenNodeCount", nn.HiddenLayer.Count.ToString());

                writer.WriteStartElement("Weight");

                for (int i = 0; i < nn.HiddenLayer.Count; i++)
                {
                    for (int j = 0; j < nn.HiddenLayer[i].NextNodeCount; j++)
                    {
                        writer.WriteElementString("H-" + (i + 1).ToString() + "-" + (j + 1).ToString(), nn.HiddenLayer[i].GetWeight(j).ToString());
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Output");
                writer.WriteElementString("OutputNodeCount", nn.OutputLayer.Count.ToString());
                
                writer.WriteElementString("Error", avgError.ToString()); 
                writer.WriteEndElement();
                if (algo.Equals("GABP"))
                {
                    string chromo = "";
                    for (int i = 0; i < fitChrom.Length; i++)
                    {
                        chromo += fitChrom[i].ToString();
                    }
                    writer.WriteElementString("Chromosom", chromo);
                }


                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

    }
}
