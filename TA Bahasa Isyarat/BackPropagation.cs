using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class BackPropagation
    {
        private float learningRate = 0.08f;
        private FeedForward feedForward;
        private NeuralNetwork lastStableNetwork;

        public NeuralNetwork Network
        {
            set { feedForward.Network = value; }
            get { return feedForward.Network; }
        }

        public DataSetList dsl
        {
            get { return feedForward.dsl; }
        }

        private ClassificationClass classificationClass;

        public void Init(NeuralNetwork nn, DataSetList dsl, ClassificationClass cc)
        {
            feedForward = new FeedForward();
            feedForward.Init(nn, dsl);
            classificationClass = cc;
        }

        public float Run(int maxIter)
        {
            float lastAvgerror = float.MaxValue;
            float avgError = new float();
            for(int iter = 0; iter < maxIter; iter++)
            {
                avgError = 0;
                for(int k = 0; k < this.dsl.Count; k++)
                {
                    Network.InitInput();
                    feedForward.Run(k);

                    float[] errorOutput = GetErrorOutput(k);
                    float totalError = 0;
                    for (int i = 0; i < errorOutput.Length; i++)
                        totalError += Math.Abs(errorOutput[i]);
                    avgError += totalError;
                }
                avgError /= this.dsl.Count;
            }
            return avgError;
        }

        public void Run()
        {
            Run(100);
        }

        private float[] GetErrorOutput(int index)
        {
            float[] outputError = new float[classificationClass.TargetCount];

            for (int i = 0; i < classificationClass.TargetCount; i++)
                outputError[i] = classificationClass.GetTarget(this.dsl[index].ClassName)[i] - Network.OutputLayer[i].Input;

            UpdateWeightHidden(outputError, index);

            return outputError;
        }

        private void UpdateWeightHidden(float[] outputError, int index)
        {
            for(int i = 0; i < Network.HiddenLayer.Count; i++)
            {
                for(int j = 0; j < Network.OutputLayer.Count; j++)
                {
                    float newWeight = Network.HiddenLayer[i].GetWeight(j) +
                        (this.learningRate * outputError[j] * Network.HiddenLayer[i].Input);
                    Network.HiddenLayer[i].SetWeight(j, newWeight);
                }
            }

            GetErrorHidden(outputError, index);
        }

        private void GetErrorHidden(float[] outputError,int index)
        {
            float[] hiddenError = new float[Network.HiddenLayer.Count];

            for(int i = 0; i < Network.HiddenLayer.Count; i++)
            {
                float linear = 0;
                for (int j = 0; j < Network.OutputLayer.Count; j++)
                    linear += outputError[j] * Network.HiddenLayer[i].GetWeight(j);

                hiddenError[i] = Network.HiddenLayer[i].Input * (1 - Network.HiddenLayer[i].Input) * linear;
            }

            UpdateWeightInput(hiddenError, index);
        }

        private void UpdateWeightInput(float[] hiddenError, int index)
        {
            for(int i = 0; i < Network.InputLayer.Count; i++)
            {
                for(int j = 0; j < Network.HiddenLayer.Count; j++)
                {
                    float newWeight = Network.InputLayer[i].GetWeight(j) +
                        (this.learningRate * hiddenError[j] * Network.InputLayer[i].Input);
                    Network.InputLayer[i].SetWeight(j, newWeight);
                }
            }
        }
    }
}
