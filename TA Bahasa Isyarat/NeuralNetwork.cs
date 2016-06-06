using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class NeuralNetwork
    {
        private Random random = new Random();

        public int Seed
        {
            set { random = new Random(value); }
        }

        public float threshold = 0.3f;

        public List<Node> InputLayer { set; get; }
        public List<Node> HiddenLayer { set; get; }
        public List<Node> OutputLayer { set; get; }

        public NeuralNetwork()
        {

        }

        public NeuralNetwork(NeuralNetwork newNN)
        {
            newNN.InitInput();
            InitNetwork(newNN.InputLayer.Count, newNN.HiddenLayer.Count, newNN.OutputLayer.Count);
            for(int i = 0; i< newNN.InputLayer.Count; i++)
            {
                InputLayer[i] = newNN.InputLayer[i];
            }

            for(int i = 0; i < newNN.HiddenLayer.Count; i++)
            {
                HiddenLayer[i] = newNN.HiddenLayer[i];
            }
        }

        public void InitInput()
        {
            for (int i = 0; i < InputLayer.Count; i++)
                InputLayer[i].Input = 0;

            for (int i = 0; i < HiddenLayer.Count; i++)
                HiddenLayer[i].Input = 0;

            for (int i = 0; i < OutputLayer.Count; i++)
                OutputLayer[i].Input = 0;
        }

        public void InitNetwork(int inputLayercount, int hiddenLayerCount, int outputLayerCount)
        {
            InputLayer = new List<Node>();
            HiddenLayer = new List<Node>();
            OutputLayer = new List<Node>();

            for (int i = 0; i < inputLayercount; i++)
                InputLayer.Add(new Node(hiddenLayerCount));

            for (int i = 0; i < hiddenLayerCount; i++)
                HiddenLayer.Add(new Node(outputLayerCount));

            for(int i = 0; i < outputLayerCount; i++)
            {
                OutputLayer.Add(new Node(1));
                OutputLayer[i].SetWeight(0, 1);
            }
        }

        public void InitWeight()
        {
            for (int i = 0; i < InputLayer.Count; i++)
                for(int j = 0; j < HiddenLayer.Count; j++)
                    InputLayer[i].SetWeight(j, GetRandom() / 100.0f);

            for (int i = 0; i < HiddenLayer.Count; i++)
                for (int j = 0; j < OutputLayer.Count; j++)
                    HiddenLayer[i].SetWeight(j, GetRandom() / 100.0f);
        }

        int GetRandom()
        {
            return random.Next(0, 30);
        }
    }
}
