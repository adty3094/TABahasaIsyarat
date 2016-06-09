using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class GeneticAlgorithm
    {
        const int individualCount = 4;
        private Random random = new Random();
        private ClassificationClass classificationClass;
        private BackPropagation backPropagation;
        private NeuralNetwork network;
        private DataSetList dataSetList;
        private List<Chromosom> chromosoms;

        public void Init(DataSetList dsl, ClassificationClass cc)
        {
            dataSetList = dsl;
            classificationClass = cc;
        }

        public Chromosom Run()
        {
            ChromosomInit();

            int iteration = 2;
            for(int i = 0; i < iteration; i++)
            {
                DoBackPropagation();
                DoSelection();
                DoCrossOver();

                int chanceMutation = GetRandom();
                if (chanceMutation < GetRandom())
                    DoMutation();
            }

            int index = 0;
            for(int i = 1; i < chromosoms.Count; i++)
            {
                if (chromosoms[i].FitnessValue > chromosoms[index].FitnessValue)
                    index = i;
            }

            Chromosom fittestChromosom = chromosoms[index];
            return fittestChromosom;
        }

        private void ChromosomInit()
        {
            chromosoms = new List<Chromosom>(individualCount);
            for(int i = 0; i < individualCount; i++)
            {
                chromosoms.Add(new Chromosom(GetRandomBinary()));
            }
        }

        private void DoBackPropagation()
        {
            for(int i = 0; i < chromosoms.Count; i++)
            {
                DataSetList dsl = new DataSetList(this.dataSetList);

                for(int j = 0; j < dsl.Count; j++)
                {
                    int popCount = 0;
                    for(int k = 0; k < chromosoms[i].Length; k++)
                    {
                        if(chromosoms[i][k] == 0)
                        {
                            dsl[j].RemoveBit(k - popCount);
                            popCount++;
                        }
                    }
                }

                NeuralNetwork nn = new NeuralNetwork();
                nn.InitNetwork(dsl[0].AttributeCount, dsl[0].AttributeCount / 2, classificationClass.TargetCount);
                nn.InitWeight();

                BackPropagation bp = new BackPropagation();
                bp.Init(nn, dsl, classificationClass);
                bp.Run(5000);

                FeedForward ff = new FeedForward();
                ff.Init(nn, dsl);

                int totalCorrect = 0;
                for(int j = 0; j < dsl.Count; j++)
                {
                    ff.Run(j);
                    bool correct = true;
                    int[] targetClass = classificationClass.GetTarget(dsl[i].ClassName);
                    for (int k = 0; k < ff.GetActualClass().Length; k++)
                    {
                        if (targetClass[k] != ff.GetActualClass()[k])
                            correct = false;
                    }
                    if (correct)
                        totalCorrect++;                    
                }
                chromosoms[i].FitnessValue = totalCorrect / (float)dsl.Count;
            }
        }

        private void DoSelection()
        {
            chromosoms.Sort((val1, val2) => val1.FitnessValue.CompareTo(val2.FitnessValue));
        }

        private void DoCrossOver()
        {
            for(int i = 0; i < chromosoms.Count; i+= 2)
            {
                int nextindex = i + 1;
                if (nextindex >= chromosoms.Count)
                    nextindex = i - 1;
                int[] bit1 = chromosoms[i].Bit;
                int[] bit2 = chromosoms[nextindex].Bit;

                for(int j = bit1.Length / 2; j < bit1.Length; j++)
                {
                    int temp = bit1[j];
                    bit1[j] = bit2[j];
                    bit2[j] = temp;
                }

                chromosoms[i].Bit = bit1;
                chromosoms[nextindex].Bit = bit2;
            }
        }

        private void DoMutation()
        {
            for(int i = 0; i < chromosoms.Count; i++)
            {
                int chanceChromosomMutation = GetRandom();
                if(chanceChromosomMutation <= GetRandom())
                {
                    int bitIndex = GetRandom(chromosoms[i].Length);
                    if (chromosoms[i][bitIndex] == 0)
                        chromosoms[i][bitIndex] = 1;
                    else
                        chromosoms[i][bitIndex] = 0;
                }
            }
        }

        private int[] GetRandomBinary()
        {
            int[] chromosomTemp = new int[dataSetList[0].AttributeCount];
            for (int i = 0; i < dataSetList[0].AttributeCount; i++)
            {
                int ran = random.Next(0, 2);
                chromosomTemp[i] = ran;
            }
            return chromosomTemp;
        }

        private int GetRandom(int max)
        {
            return random.Next(0, max);
        }

        private int GetRandom()
        {
            return random.Next(0, 100);
        }
    }
}
