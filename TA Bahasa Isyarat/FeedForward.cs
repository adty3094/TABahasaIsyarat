using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class FeedForward
    {
        private NeuralNetwork neuralNetwork;

        public NeuralNetwork Network
        {
            get { return neuralNetwork; }
            set { neuralNetwork = value; }
        }

        private DataSetList _dsl;

        public DataSetList dsl
        {
            get { return _dsl; }
        }

        public void Init(NeuralNetwork nn , DataSetList dsl)
        {
            neuralNetwork = nn;
            _dsl = dsl;
        }

        public void Run()
        {
            for(int i = 0; i < _dsl.Count; i++)
            {
                neuralNetwork.InitInput();
                DoInputLayer(i);                
            }
        }

        public void Run(int index)
        {
            neuralNetwork.InitInput();
            DoInputLayer(index);
        }

        private void DoInputLayer(int index)
        {
            for (int i = 0; i < _dsl[index].AttributeCount; i++)
                neuralNetwork.InputLayer[i].Input = _dsl[index][i];
            DoHiddenLayer();
        }
        private void DoHiddenLayer()
        {
            for (int i = 0; i < neuralNetwork.InputLayer.Count; i++)
                for (int j = 0; j < neuralNetwork.HiddenLayer.Count; j++)
                    neuralNetwork.HiddenLayer[j].Input += neuralNetwork.InputLayer[i].GetOutput(j);

            for (int j = 0; j < neuralNetwork.HiddenLayer.Count; j++)
                neuralNetwork.HiddenLayer[j].Input = Sigmoid(neuralNetwork.HiddenLayer[j].Input);
            DoOutputLayer();
        }

        private void DoOutputLayer()
        {
            for (int i = 0; i < neuralNetwork.HiddenLayer.Count; i++)
                for (int j = 0; j < neuralNetwork.OutputLayer.Count; j++)
                    neuralNetwork.OutputLayer[j].Input += neuralNetwork.HiddenLayer[i].Input * neuralNetwork.HiddenLayer[i].GetWeight(j);
        }

        public int[] GetActualClass()
        {
            int[] act = new int[neuralNetwork.OutputLayer.Count];
            for(int j = 0; j < neuralNetwork.OutputLayer.Count; j++)
            {
                if (Math.Abs(neuralNetwork.OutputLayer[j].Input) < neuralNetwork.threshold)
                    act[j] = 0;
                else
                    act[j] = 1;
            }
            return act;
        }

        private float Sigmoid(float val)
        {
            return 1 / (1.0f + (float)Math.Exp(-val));
        }
    }
}
