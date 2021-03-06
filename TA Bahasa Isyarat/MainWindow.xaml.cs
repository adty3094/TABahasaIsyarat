﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace TA_Bahasa_Isyarat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Kinect
        private const int KinectDefaultAngle = 0;
        private KinectSensor mainSensor;
        private Skeleton first;        
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        //Method
        ClassificationClass cc = new ClassificationClass();
        ClassificationClass cc1 = new ClassificationClass();
        ClassificationClass cc2 = new ClassificationClass();
        ClassificationClass ccBP = new ClassificationClass();

        //etc
        const string path = "..\\..\\..\\DataSet\\";
        const string pathBP = "..\\..\\..\\DataSet\\BP\\";
        const string imagePath = "..\\..\\..\\IsyaratImage\\";
        private string filename;
        bool result;
        bool isFirstNull;
        bool isTestingMode = false;
        bool isHaveNaN;
        bool isHaveBadValue;
        bool isNeedReboot = false;
        int phase;
        string algorithm = "BP";
        string algoTest = "BP";
        NeuralNetwork loadNet = new NeuralNetwork();
        int[] seleksi1;
        int[] seleksi2;

        StreamWriter file;
        
        //Joints
        //Right Body 
        Vector3 SR = new Vector3();
        Vector3 ER = new Vector3();
        Vector3 WR = new Vector3();
        Vector3 HR = new Vector3();
        //Left Body
        Vector3 SL = new Vector3();
        Vector3 EL = new Vector3();
        Vector3 WL = new Vector3();
        Vector3 HL = new Vector3();
        //Center Body;
        Vector3 SC = new Vector3();
        Vector3 Head = new Vector3();

        //Feature
        Vector3 SRER = new Vector3();
        Vector3 ERWR = new Vector3();
        Vector3 WRHR = new Vector3();
        Vector3 SLEL = new Vector3();
        Vector3 ELWL = new Vector3();
        Vector3 WLHL = new Vector3();
        Vector3 HRHL = new Vector3();
        double SCSRER = new double();
        double SRERWR = new double();
        double ERWRHR = new double();
        double SCSLEL = new double();
        double SLELWL = new double();
        double ELWLHL = new double();
        double DisHRHL = new double();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitFeatures()
        {
            Vector3 SRER = new Vector3();
            Vector3 ERWR = new Vector3();
            Vector3 WRHR = new Vector3();
            Vector3 SLEL = new Vector3();
            Vector3 ELWL = new Vector3();
            Vector3 WLHL = new Vector3();
            Vector3 HRHL = new Vector3();
            double SCSRER = 0f;
            double SRERWR = 0f;
            double ERWRHR = 0f;
            double SCSLEL = 0f;
            double SLELWL = 0f;
            double ELWLHL = 0f;
            double DisHRHL = 0f;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusDetail.Content = "Program Started";

            if (KinectSensor.KinectSensors.Count > 0)
            {
                StatusDetail.Content = "Kinect Found";
                mainSensor = KinectSensor.KinectSensors[0];


                if (mainSensor.IsRunning)
                {
                    StatusDetail.Content = "Reset Kinect";
                    StopKinect(mainSensor);
                }

                if (mainSensor.Status == KinectStatus.Connected)
                {
                    StatusDetail.Content = "Setup Kinect"; 
                    Microsoft.Samples.Kinect.WpfViewers.KinectSensorManager kinectManager = new Microsoft.Samples.Kinect.WpfViewers.KinectSensorManager();
                    kinectManager.KinectSensor = mainSensor;
                    kinectManager.ColorStreamEnabled = true;
                    kinectManager.SkeletonStreamEnabled = true;
                    kinectManager.DepthStreamEnabled = true;
                    kinectManager.ElevationAngle = KinectDefaultAngle;
                    ColorView.KinectSensorManager =
                        SkeletonView.KinectSensorManager = kinectManager;         
                    StatusDetail.Content = "Idle";
                    mainSensor.AllFramesReady += MainSensor_AllFramesReady;
                    InitClassificationClass();
                }
            }
            else StatusDetail.Content = "Kinect Not Found";
        }

        private void InitClassificationClass()
        {
            string group1path = path + "gol1-normalize\\";
            string group2path = path + "gol2-normalize\\";
            DirectoryInfo dirInfo1 = new DirectoryInfo(group1path);
            FileInfo[] files1 = dirInfo1.GetFiles("*.txt");
            DirectoryInfo dirInfo2 = new DirectoryInfo(group2path);
            FileInfo[] files2 = dirInfo2.GetFiles("*.txt");
            DirectoryInfo dirInfoBP = new DirectoryInfo(pathBP);
            FileInfo[] filesBP = dirInfoBP.GetFiles("*.txt");
            cc1 = new ClassificationClass();
            cc2 = new ClassificationClass();
            ccBP = new ClassificationClass();

            group1Gesture.Items.Clear();
            foreach(FileInfo f in files1)
            {
                string className = f.Name.Split('.')[1];
                if (cc1.GetIndex(className) == -1)
                {
                    cc1.Add(className);
                    group1Gesture.Items.Add(className);
                }
            }

            group2Gesture.Items.Clear();
            foreach (FileInfo f in files2)
            {
                string className = f.Name.Split('.')[1];
                if (cc2.GetIndex(className) == -1)
                {
                    cc2.Add(className);
                    group2Gesture.Items.Add(className);
                }
            }

            foreach(FileInfo f in filesBP)
            {
                string className = f.Name.Split('.')[1];
                if (ccBP.GetIndex(className) == -1)
                    ccBP.Add(className);
            }
        }

        private void MainSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //Get a skeleton
            first = GetFirstSkeleton(e);

            if (first == null)
            {
                isFirstNull = true;
                return;
            }
            isFirstNull = false;
        }

        private Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                    return null;

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;
            }
        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.AudioSource.Stop();
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (phase == 1)
                StatusDetail.Content = "Searching Skeleton";
            else if (phase == 2)
                StatusDetail.Content = "Skeleton Found";
            else if (phase == 3)
            {
                StatusDetail.Content = "Creating File";
                progressBar.Value = e.ProgressPercentage;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isNeedReboot)
            {
                DisableAll();
                return;
            }
            else if (isHaveBadValue)
                StatusDetail.Content = "Features Have value out of range ( 0.0 - 1.0 )";
            else if (result)
            {
                StatusDetail.Content = "File Created";
            }
            else
                StatusDetail.Content = "Skeleton Lost / Not Found";
            createButton.IsEnabled = true;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            phase = 1;
            (sender as BackgroundWorker).ReportProgress(0);
            Thread.Sleep(5000);
            if (first == null)
            {
                result = false;
                return;
            }
            phase++;
            (sender as BackgroundWorker).ReportProgress(0);
            Thread.Sleep(1000);
            phase++;

            StringBuilder sb = new StringBuilder();

            int progress = 0;
            InitFeatures();
            (sender as BackgroundWorker).ReportProgress(progress);
            
            for (int i = 0; i < 100; i++)
            {
                if (first == null)
                {
                    result = false;
                    return;
                }

                progress ++;
                //Right Body
                SR.SetVector(first.Joints[JointType.ShoulderRight].Position);
                ER.SetVector(first.Joints[JointType.ElbowRight].Position);
                WR.SetVector(first.Joints[JointType.WristRight].Position);
                HR.SetVector(first.Joints[JointType.HandRight].Position);
                //Left Body
                SL.SetVector(first.Joints[JointType.ShoulderLeft].Position);
                EL.SetVector(first.Joints[JointType.ElbowLeft].Position);
                WL.SetVector(first.Joints[JointType.WristLeft].Position);
                HL.SetVector(first.Joints[JointType.HandLeft].Position);
                //Center Body
                SC.SetVector(first.Joints[JointType.ShoulderCenter].Position);
                Head.SetVector(first.Joints[JointType.Head].Position);
                
                SRER += ER - SR;
                ERWR += WR - ER;
                WRHR += HR - WR;
                SLEL += EL - SL;
                ELWL += WL - EL;
                WLHL += HL - WL;
                HRHL += HL - HR;

                Vector3 v1, v2;
                double res;

                //SC-SR-ER
                v1 = SC - SR;
                v1 = v1.Normalize();
                v2 = ER - SR;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SCSRER += (double)Math.Acos(res);

                //SR-ER-WR
                v1 = SR - ER;
                v1 = v1.Normalize();
                v2 = WR - ER;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SRERWR += (double)Math.Acos(res);

                //ER-WR-HR
                v1 = ER - WR;
                v1 = v1.Normalize();
                v2 = HR - WR;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                ERWRHR += (double)Math.Acos(res);

                //SC-SL-EL
                v1 = SC - SL;
                v1 = v1.Normalize();
                v2 = EL - SL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SCSLEL += (double)Math.Acos(res);

                //SL-EL-WL
                v1 = SL - EL;
                v1 = v1.Normalize();
                v2 = WL - EL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SLELWL += (double)Math.Acos(res);

                //EL-WL-HL
                v1 = EL - WL;
                v1 = v1.Normalize();
                v2 = HL - WL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                ELWLHL += (double)Math.Acos(res);

                //Distance HR - HL
                DisHRHL += Vector3.Distance(HR, HL);

                (sender as BackgroundWorker).ReportProgress(progress);
                Thread.Sleep(10);
            }
            SRER /= 100;
            ERWR /= 100;
            WRHR /= 100;
            SLEL /= 100;
            ELWL /= 100;
            WLHL /= 100;
            HRHL /= 100;
            SCSRER /= 100;
            SRERWR /= 100;
            ERWRHR /= 100;
            SCSLEL /= 100;
            SLELWL /= 100;
            ELWLHL /= 100;
            DisHRHL /= 100;
            AllFeatureNormalize();
            if(!CheckFeatureOK())
            {
                if(isHaveNaN)
                    isNeedReboot = true;
                return;
            }
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", SRER.X.ToString("0.00000"), System.Environment.NewLine, SRER.Y.ToString("0.00000"), System.Environment.NewLine, SRER.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", ERWR.X.ToString("0.00000"), System.Environment.NewLine, ERWR.Y.ToString("0.00000"), System.Environment.NewLine, ERWR.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", WRHR.X.ToString("0.00000"), System.Environment.NewLine, WRHR.Y.ToString("0.00000"), System.Environment.NewLine, WRHR.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", SLEL.X.ToString("0.00000"), System.Environment.NewLine, SLEL.Y.ToString("0.00000"), System.Environment.NewLine, SLEL.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", ELWL.X.ToString("0.00000"), System.Environment.NewLine, ELWL.Y.ToString("0.00000"), System.Environment.NewLine, ELWL.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", WLHL.X.ToString("0.00000"), System.Environment.NewLine, WLHL.Y.ToString("0.00000"), System.Environment.NewLine, WLHL.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}{1}{2}{3}{4}", HRHL.X.ToString("0.00000"), System.Environment.NewLine, HRHL.Y.ToString("0.00000"), System.Environment.NewLine, HRHL.Z.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}", SCSRER.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}", SRERWR.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}", ERWRHR.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}", SCSLEL.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}", SLELWL.ToString("0.00000")));
            sb.AppendLine(String.Format("{0}", ELWLHL.ToString("0.00000")));
            sb.Append(String.Format("{0}", DisHRHL.ToString("0.00000")));

            int gol = 0;
            if (ERWR.Y < 0.53)
                gol = 1;
            else
                gol = 2;

            result = true;
            
            int count = 0;
            string fullPath;
            if (gol == 1)
            {
                filename = string.Format("gol1.{0}", filename);
                fullPath = @path + "gol1-normalize\\" + filename;
            }
            else
            {
                filename = string.Format("gol2.{0}", filename);
                fullPath = @path + "gol2-normalize\\" + filename;
            }
            while (File.Exists(fullPath)) 
            {
                string[] temp = filename.Split('.');
                count++;
                if(gol == 1)
                    filename = String.Format("gol1.{0}.{1}.txt", temp[1], count.ToString());
                else
                    filename = String.Format("gol2.{0}.{1}.txt", temp[1], count.ToString());

                if (gol == 1)
                    fullPath = @path + "gol1-normalize\\" + filename;
                else
                    fullPath = @path + "gol2-normalize\\" + filename;
            }
            File.AppendAllText(fullPath, sb.ToString());

            //Creating Weka Testing File
            sb.Replace(",", ".");
            sb.Replace(System.Environment.NewLine, ",");
            sb.AppendLine("," + filename.Split('.')[1]);
            string wekafile = @path + filename.Split('.')[1] + ".arff";
            File.AppendAllText(wekafile, sb.ToString());

            return;
        }

        private void createButton_click(object sender, RoutedEventArgs e)
        {
            createButton.IsEnabled = false;
            if(fileName.Text.Length < 3)
            {
                StatusDetail.Content = "Gesture name must have 3 or more character";
                return;
            }
            filename = fileName.Text + ".0.txt";
            StatusDetail.Content = "Searching for Skeleton";
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            StopKinect(mainSensor);
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            TrainButton.IsEnabled = false;
            if (algorithm.Equals("BP"))
            {
                float avgError = 0;
                NeuralNetwork nn = new NeuralNetwork();
                DirectoryInfo info = new DirectoryInfo(pathBP);
                DataSetList dsl = new DataSetList();
                FileReader fr = new FileReader();
                dsl = fr.ReadFile(info.FullName, ccBP);
                StatusDetail.Content = "Training Dataset";
                nn.InitNetwork(dsl[0].AttributeCount, dsl[0].AttributeCount / 2, ccBP.TargetCount);
                nn.Seed = 0;
                nn.InitWeight();
                BackPropagation bp = new BackPropagation();
                bp.Init(nn, dsl, ccBP);
                avgError = bp.Run(Int32.Parse(iteration_text.Text));
                NNtoXMLWriter nnw = new NNtoXMLWriter(nn, avgError);
                nnw.Write("bpnet.xml", algorithm, null);

            }
            else
            {
                float avgError1 = 0;
                float avgError2 = 0;
                NeuralNetwork nn1 = new NeuralNetwork();
                NeuralNetwork nn2 = new NeuralNetwork();
                DirectoryInfo info1 = new DirectoryInfo(string.Format("{0}gol1-normalize\\", path));
                DirectoryInfo info2 = new DirectoryInfo(string.Format("{0}gol2-normalize\\", path));
                DataSetList dsl1 = new DataSetList();
                DataSetList dsl2 = new DataSetList();
                FileReader fr1 = new FileReader();
                FileReader fr2 = new FileReader();
                dsl1 = fr1.ReadFile(info1.FullName, cc1);
                dsl2 = fr2.ReadFile(info2.FullName, cc2);
                StatusDetail.Content = "Training Dataset";

                GeneticAlgorithm ga1 = new GeneticAlgorithm();
                GeneticAlgorithm ga2 = new GeneticAlgorithm();
                ga1.Init(dsl1, cc1);
                ga2.Init(dsl2, cc2);

                //Chromosom fitChrom1 = ga1.Run();
                //Chromosom fitChrom2 = ga2.Run();

                loadNet = new NeuralNetwork();
                NNtoXMLReader nnr = new NNtoXMLReader(loadNet);
                algoTest = nnr.read("gol1ga.xml");
                seleksi1 = nnr.chromosom;

                loadNet = new NeuralNetwork();
                nnr = new NNtoXMLReader(loadNet);
                algoTest = nnr.read("gol2ga.xml");
                seleksi2 = nnr.chromosom;

                for (int i = 0; i < dsl1.Count; i++)
                {
                    int popCount = 0;
                    for (int j = 0; j < seleksi1.Count(); j++)
                    {
                        if (seleksi1[j] == 0)
                        {
                            dsl1[i].RemoveBit(j - popCount);
                            popCount++;
                        }
                    }
                }

                for (int i = 0; i < dsl2.Count; i++)
                {
                    int popCount = 0;
                    for (int j = 0; j < seleksi2.Count(); j++)
                    {
                        if (seleksi2[j] == 0)
                        {
                            dsl2[i].RemoveBit(j - popCount);
                            popCount++;
                        }
                    }
                }

                nn1.InitNetwork(dsl1[0].AttributeCount, dsl1[0].AttributeCount / 2, cc1.TargetCount);
                nn2.InitNetwork(dsl2[0].AttributeCount, dsl2[0].AttributeCount / 2, cc2.TargetCount);
                nn1.Seed = 0;
                nn2.Seed = 0;
                nn1.InitWeight();
                nn2.InitWeight();
                BackPropagation bp1 = new BackPropagation();
                BackPropagation bp2 = new BackPropagation();
                bp1.Init(nn1, dsl1, cc1);
                bp2.Init(nn2, dsl2, cc2);
                avgError1 = bp1.Run(Int32.Parse(iteration_text.Text));
                avgError2 = bp2.Run(Int32.Parse(iteration_text.Text));

                NNtoXMLWriter nnw1 = new NNtoXMLWriter(nn1, avgError1);
                NNtoXMLWriter nnw2 = new NNtoXMLWriter(nn2, avgError2);
                nnw1.Write("gol1ga.xml", algorithm, seleksi1);
                nnw2.Write("gol2ga.xml", algorithm, seleksi2);
                
                group1Gesture.Items.Clear();
                group2Gesture.Items.Clear();

                for (int i = 0; i < cc1.GetClassList().Count; i++)
                {
                    group1Gesture.Items.Add(cc1[i]);
                }
                for (int i = 0; i < cc2.GetClassList().Count; i++)
                {
                    group2Gesture.Items.Add(cc2[i]);
                }
            }
            StatusDetail.Content = "Training Finished";
            TrainButton.IsEnabled = true;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if(!isTestingMode)
            {
                isTestingMode = true;
                TestButton.Content = "Stop Testing";
                StatusDetail.Content = "Testing Mode";
                BackgroundWorker testWorker = new BackgroundWorker();
                testWorker.WorkerReportsProgress = true;
                testWorker.DoWork += TestWorker_DoWork;
                testWorker.ProgressChanged += TestWorker_ProgressChanged;
                testWorker.RunWorkerCompleted += TestWorker_RunWorkerCompleted;
                testWorker.RunWorkerAsync();

            }
            else if(isTestingMode)
            {
                isTestingMode = false;
                TestButton.Content = "Start Testing";
            }
        }

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isNeedReboot)
                DisableAll();
            else if (isHaveBadValue)
            {
                isTestingMode = false;
                TestButton.Content = "Start Testing";
                StatusDetail.Content = "Features Have value out of range ( 0.0 - 1.0 )";
            }
            else
                StatusDetail.Content = "Testing Finished";
        }

        private void DisableAll()
        {
            StatusDetail.Content = "Feature Damaged, Program need to reboot";
            TestButton.Content = "Need Reboot";
            TrainButton.Content = "Need Reboot";
            outputText.Content = "Need Reboot";
            createButton.Content = "Need Reboot";
            comboBox.IsEnabled = false;
            TestButton.IsEnabled = false;
            TrainButton.IsEnabled = false;
            createButton.IsEnabled = false;
        }

        private void TestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int gol;
                DataSetList dsl = new DataSetList();
                List<float> fitur = new List<float>();
                string xmlName = "";
                fitur.Add((float)SRER.X);
                fitur.Add((float)SRER.Y);
                fitur.Add((float)SRER.Z);
                fitur.Add((float)ERWR.X);
                fitur.Add((float)ERWR.Y);
                fitur.Add((float)ERWR.Z);
                fitur.Add((float)WRHR.X);
                fitur.Add((float)WRHR.Y);
                fitur.Add((float)WRHR.Z);
                fitur.Add((float)SLEL.X);
                fitur.Add((float)SLEL.Y);
                fitur.Add((float)SLEL.Z);
                fitur.Add((float)ELWL.X);
                fitur.Add((float)ELWL.Y);
                fitur.Add((float)ELWL.Z);
                fitur.Add((float)WLHL.X);
                fitur.Add((float)WLHL.Y);
                fitur.Add((float)WLHL.Z);
                fitur.Add((float)HRHL.X);
                fitur.Add((float)HRHL.Y);
                fitur.Add((float)HRHL.Z);
                fitur.Add((float)SCSRER);
                fitur.Add((float)SRERWR);
                fitur.Add((float)ERWRHR);
                fitur.Add((float)SCSLEL);
                fitur.Add((float)SLELWL);
                fitur.Add((float)ELWLHL);
                fitur.Add((float)DisHRHL);
                if (algorithm.Equals("BP"))
                {
                    xmlName = "bpnet.xml";
                    gol = 0;
                }
                else if (ERWR.Y < 0.53)
                {
                    xmlName = "gol1ga.xml";
                    gol = 1;
                }
                else
                {
                    xmlName = "gol2ga.xml";
                    gol = 2;
                }
                DataSet ds = new DataSet(fitur.Count);
                for (int i = 0; i < fitur.Count; i++)
                {
                    ds[i] = fitur[i];
                }

                dsl.Add(ds);

                FeedForward ff = new FeedForward();
                loadNet = new NeuralNetwork();
                NNtoXMLReader nnr = new NNtoXMLReader(loadNet);
                Translate t = new Translate();

                algoTest = nnr.read(xmlName);
                for (int i = 0; i < dsl.Count; i++)
                {
                    if (algoTest.Equals("BPGA"))
                    {
                        int popCount = 0;
                        for (int j = 0; j < nnr.chromosom.Length; j++)
                        {
                            if (nnr.chromosom[j] == 0)
                            {
                                dsl[i].RemoveBit(j - popCount);
                                popCount++;
                            }
                        }
                    }
                }


                loadNet = new NeuralNetwork();
                nnr = new NNtoXMLReader(loadNet);
                nnr.read(xmlName);

                ff = new FeedForward();
                ff.Init(loadNet, dsl);
                for (int i = 0; i < dsl.Count; i++)
                    ff.Run(i);

                t = new Translate(ff.GetActualClass());
                if (gol == 0)
                    outputText.Content = t.Result(ccBP);
                else if (gol == 1)
                    outputText.Content = t.Result(cc1);
                else if (gol == 2)
                    outputText.Content = t.Result(cc2);
                string imageFullPath = @imagePath + outputText.Content + ".bmp";
                if (File.Exists(imageFullPath))
                    outputImage.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(imageFullPath);
                Log_ERWRY.Content = ERWR.Y;
                string actualClass = "";
                for (int i = 0; i < t.GetActualClassLength(); i++)
                    actualClass += t[i];
                Log_ActualClass.Content = actualClass;
                outputList.Items.Clear();
                for (int i = 0; i < loadNet.OutputLayer.Count; i++)
                    outputList.Items.Add(loadNet.OutputLayer[i].Input);
                StatusDetail.Content = "Testing Mode";
            }
            catch
            {
                StatusDetail.Content = "Testing Error";
            }
        }

        private void TestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isTestingMode)
            {
                InitFeatures();
                if (first != null)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (first == null) break;
                        //Right Body
                        SR.SetVector(first.Joints[JointType.ShoulderRight].Position);
                        ER.SetVector(first.Joints[JointType.ElbowRight].Position);
                        WR.SetVector(first.Joints[JointType.WristRight].Position);
                        HR.SetVector(first.Joints[JointType.HandRight].Position);
                        //Left Body
                        SL.SetVector(first.Joints[JointType.ShoulderLeft].Position);
                        EL.SetVector(first.Joints[JointType.ElbowLeft].Position);
                        WL.SetVector(first.Joints[JointType.WristLeft].Position);
                        HL.SetVector(first.Joints[JointType.HandLeft].Position);
                        //Center Body
                        SC.SetVector(first.Joints[JointType.ShoulderCenter].Position);

                        SRER += ER - SR;
                        ERWR += WR - ER;
                        WRHR += HR - WR;
                        SLEL += EL - SL;
                        ELWL += WL - EL;
                        WLHL += HL - WL;
                        HRHL += HL - HR;

                        Vector3 v1, v2;
                        double res;

                        //SC-SR-ER
                        v1 = SC - SR;
                        v1 = v1.Normalize();
                        v2 = ER - SR;
                        v2 = v2.Normalize();
                        res = Vector3.DotProduct(v1, v2);
                        SCSRER += (double)Math.Acos(res);

                        //SR-ER-WR
                        v1 = SR - ER;
                        v1 = v1.Normalize();
                        v2 = WR - ER;
                        v2 = v2.Normalize();
                        res = Vector3.DotProduct(v1, v2);
                        SRERWR += (double)Math.Acos(res);

                        //ER-WR-HR
                        v1 = ER - WR;
                        v1 = v1.Normalize();
                        v2 = HR - WR;
                        v2 = v2.Normalize();
                        res = Vector3.DotProduct(v1, v2);
                        ERWRHR += (double)Math.Acos(res);

                        //SC-SL-EL
                        v1 = SC - SL;
                        v1 = v1.Normalize();
                        v2 = EL - SL;
                        v2 = v2.Normalize();
                        res = Vector3.DotProduct(v1, v2);
                        SCSLEL += (double)Math.Acos(res);

                        //SL-EL-WL
                        v1 = SL - EL;
                        v1 = v1.Normalize();
                        v2 = WL - EL;
                        v2 = v2.Normalize();
                        res = Vector3.DotProduct(v1, v2);
                        SLELWL += (double)Math.Acos(res);

                        //EL-WL-HL
                        v1 = EL - WL;
                        v1 = v1.Normalize();
                        v2 = HL - WL;
                        v2 = v2.Normalize();
                        res = Vector3.DotProduct(v1, v2);
                        ELWLHL += (double)Math.Acos(res);

                        //Distance HR - HL
                        DisHRHL += Vector3.Distance(HR, HL);
                    }
                    SRER /= 100;
                    ERWR /= 100;
                    WRHR /= 100;
                    SLEL /= 100;
                    ELWL /= 100;
                    WLHL /= 100;
                    HRHL /= 100;
                    SCSRER /= 100;
                    SRERWR /= 100;
                    ERWRHR /= 100;
                    SCSLEL /= 100;
                    SLELWL /= 100;
                    ELWLHL /= 100;
                    DisHRHL /= 100;
                    AllFeatureNormalize();
                    if (!CheckFeatureOK())
                    {
                        if (isHaveNaN)
                            isNeedReboot = true;
                        return;
                    }
                    (sender as BackgroundWorker).ReportProgress(1);
                    Thread.Sleep(3000);
                }
            }
        }

        private void AllFeatureNormalize()
        {
            SRER = Features.VectorFeatureNorm(SRER);
            ERWR = Features.VectorFeatureNorm(ERWR);
            WRHR = Features.VectorFeatureNorm(WRHR);
            SLEL = Features.VectorFeatureNorm(SLEL);
            ELWL = Features.VectorFeatureNorm(ELWL);
            WLHL = Features.VectorFeatureNorm(WLHL);
            HRHL = Features.VectorFeatureNorm(HRHL);
            SCSRER = Features.AngleFeatureNorm(SCSRER);
            SRERWR = Features.AngleFeatureNorm(SRERWR);
            ERWRHR = Features.AngleFeatureNorm(ERWRHR);
            SCSLEL = Features.AngleFeatureNorm(SCSLEL);
            SLELWL = Features.AngleFeatureNorm(SLELWL);
            ELWLHL = Features.AngleFeatureNorm(ELWLHL);
            DisHRHL = Features.DistanceFeatureNorm(DisHRHL);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusDetail.Content = "Algorithm Changed";
            if (comboBox.SelectedIndex == 0)
                algorithm = "BP";
            else if (comboBox.SelectedIndex == 1)
                algorithm = "BPGA";
        }

        private bool CheckFeatureOK()
        {
            List<float> fitur = new List<float>();
            fitur.Add((float)SRER.X);
            fitur.Add((float)SRER.Y);
            fitur.Add((float)SRER.Z);
            fitur.Add((float)ERWR.X);
            fitur.Add((float)ERWR.Y);
            fitur.Add((float)ERWR.Z);
            fitur.Add((float)WRHR.X);
            fitur.Add((float)WRHR.Y);
            fitur.Add((float)WRHR.Z);
            fitur.Add((float)SLEL.X);
            fitur.Add((float)SLEL.Y);
            fitur.Add((float)SLEL.Z);
            fitur.Add((float)ELWL.X);
            fitur.Add((float)ELWL.Y);
            fitur.Add((float)ELWL.Z);
            fitur.Add((float)WLHL.X);
            fitur.Add((float)WLHL.Y);
            fitur.Add((float)WLHL.Z);
            fitur.Add((float)HRHL.X);
            fitur.Add((float)HRHL.Y);
            fitur.Add((float)HRHL.Z);
            fitur.Add((float)SCSRER);
            fitur.Add((float)SRERWR);
            fitur.Add((float)ERWRHR);
            fitur.Add((float)SCSLEL);
            fitur.Add((float)SLELWL);
            fitur.Add((float)ELWLHL);
            fitur.Add((float)DisHRHL);
            for(int i = 0; i < fitur.Count; i++)
            {
                if (float.IsNaN(fitur[i]))
                {
                    isHaveNaN = true;
                    isHaveBadValue = false;
                    return false;
                }
                else if(fitur[i] > 1 || fitur[i] < 0)
                {
                    isHaveBadValue = true;
                    isHaveNaN = false;
                    return false;
                }
            }
            isHaveBadValue = false;
            isHaveNaN = false;
            return true;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int angle = (int)slider.Value;
            try
            {
                mainSensor.ElevationAngle = angle;
            }
            catch
            {
                StatusDetail.Content = "Failed to chanage Kinect Angle";
            }
        }

        private void OneTestButton_Click(object sender, RoutedEventArgs e)
        {
            OneTestButton.IsEnabled = false;
            BackgroundWorker oneWorker = new BackgroundWorker();
            oneWorker.WorkerReportsProgress = true;
            oneWorker.DoWork += OneWorker_DoWork;
            oneWorker.ProgressChanged += OneWorker_ProgressChanged;
            oneWorker.RunWorkerCompleted += OneWorker_RunWorkerCompleted;
            oneWorker.RunWorkerAsync();
        }

        private void OneWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isNeedReboot)
            {
                DisableAll();
            }
            else if(isHaveBadValue)
            {
                StatusDetail.Content = "Features Have value out of range ( 0.0 - 1.0 )";
            }
            else if (result)
            {
                try
                {
                    int gol;
                    DataSetList dsl = new DataSetList();
                    List<float> fitur = new List<float>();
                    string xmlName = "";
                    fitur.Add((float)SRER.X);
                    fitur.Add((float)SRER.Y);
                    fitur.Add((float)SRER.Z);
                    fitur.Add((float)ERWR.X);
                    fitur.Add((float)ERWR.Y);
                    fitur.Add((float)ERWR.Z);
                    fitur.Add((float)WRHR.X);
                    fitur.Add((float)WRHR.Y);
                    fitur.Add((float)WRHR.Z);
                    fitur.Add((float)SLEL.X);
                    fitur.Add((float)SLEL.Y);
                    fitur.Add((float)SLEL.Z);
                    fitur.Add((float)ELWL.X);
                    fitur.Add((float)ELWL.Y);
                    fitur.Add((float)ELWL.Z);
                    fitur.Add((float)WLHL.X);
                    fitur.Add((float)WLHL.Y);
                    fitur.Add((float)WLHL.Z);
                    fitur.Add((float)HRHL.X);
                    fitur.Add((float)HRHL.Y);
                    fitur.Add((float)HRHL.Z);
                    fitur.Add((float)SCSRER);
                    fitur.Add((float)SRERWR);
                    fitur.Add((float)ERWRHR);
                    fitur.Add((float)SCSLEL);
                    fitur.Add((float)SLELWL);
                    fitur.Add((float)ELWLHL);
                    fitur.Add((float)DisHRHL);
                    if (algorithm.Equals("BP"))
                    {
                        xmlName = "bpnet.xml";
                        gol = 0;
                    }
                    else if (ERWR.Y < 0.53)
                    {
                        xmlName = "gol1ga.xml";
                        gol = 1;
                    }
                    else
                    {
                        xmlName = "gol2ga.xml";
                        gol = 2;
                    }
                    DataSet ds = new DataSet(fitur.Count);
                    for (int i = 0; i < fitur.Count; i++)
                    {
                        ds[i] = fitur[i];
                    }

                    dsl.Add(ds);

                    FeedForward ff = new FeedForward();
                    loadNet = new NeuralNetwork();
                    NNtoXMLReader nnr = new NNtoXMLReader(loadNet);
                    Translate t = new Translate();

                    algoTest = nnr.read(xmlName);
                    for (int i = 0; i < dsl.Count; i++)
                    {
                        if (algoTest.Equals("BPGA"))
                        {
                            int popCount = 0;
                            for (int j = 0; j < nnr.chromosom.Length; j++)
                            {
                                if (nnr.chromosom[j] == 0)
                                {
                                    dsl[i].RemoveBit(j - popCount);
                                    popCount++;
                                }
                            }
                        }
                    }

                    loadNet = new NeuralNetwork();
                    nnr = new NNtoXMLReader(loadNet);
                    nnr.read(xmlName);

                    ff = new FeedForward();
                    ff.Init(loadNet, dsl);
                    for (int i = 0; i < dsl.Count; i++)
                        ff.Run(i);

                    int[] actualClass = new int[loadNet.OutputLayer.Count];
                    actualClass = ff.GetActualClass();

                    t = new Translate(actualClass);
                    string actualClassLog = "";
                    for(int i = 0; i < actualClass.Length; i++)
                        actualClassLog += actualClass[i];
                    if (gol == 0)
                        outputText.Content = t.Result(ccBP);
                    else if (gol == 1)
                        outputText.Content = t.Result(cc1);
                    else if (gol == 2)
                        outputText.Content = t.Result(cc2);
                    string imageFullPath = @imagePath + outputText.Content + ".bmp";
                    if (File.Exists(imageFullPath)) 
                        outputImage.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(imageFullPath);
                    Log_ERWRY.Content = ERWR.Y.ToString();
                    outputList.Items.Clear();
                    for (int i = 0; i < loadNet.OutputLayer.Count(); i++)
                    {
                        outputList.Items.Add(loadNet.OutputLayer[i].Input);
                    }
                    StatusDetail.Content = "Test Completed";
                }
                catch
                {
                    StatusDetail.Content = "Testing Failed";
                }
            }
            else
                StatusDetail.Content = "Skeleton Lost / Not Found";
            OneTestButton.IsEnabled = true;
        }

        private void OneWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (phase == 1)
                StatusDetail.Content = "Searching Skeleton";
            else if (phase == 2)
                StatusDetail.Content = "Testing Ready";
        }

        private void OneWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            phase = 1;
            (sender as BackgroundWorker).ReportProgress(0);
            Thread.Sleep(5000);
            if (first == null)
            {
                result = false;
                return;
            }
            phase++;
            (sender as BackgroundWorker).ReportProgress(0);
            Thread.Sleep(5000);

            InitFeatures();

            for (int i = 0; i < 100; i++)
            {
                if (first == null)
                {
                    result = false;
                    return;
                }
                
                //Right Body
                SR.SetVector(first.Joints[JointType.ShoulderRight].Position);
                ER.SetVector(first.Joints[JointType.ElbowRight].Position);
                WR.SetVector(first.Joints[JointType.WristRight].Position);
                HR.SetVector(first.Joints[JointType.HandRight].Position);
                //Left Body
                SL.SetVector(first.Joints[JointType.ShoulderLeft].Position);
                EL.SetVector(first.Joints[JointType.ElbowLeft].Position);
                WL.SetVector(first.Joints[JointType.WristLeft].Position);
                HL.SetVector(first.Joints[JointType.HandLeft].Position);
                //Center Body
                SC.SetVector(first.Joints[JointType.ShoulderCenter].Position);
                Head.SetVector(first.Joints[JointType.Head].Position);

                SRER += ER - SR;
                ERWR += WR - ER;
                WRHR += HR - WR;
                SLEL += EL - SL;
                ELWL += WL - EL;
                WLHL += HL - WL;
                HRHL += HL - HR;

                Vector3 v1, v2;
                double res;

                //SC-SR-ER
                v1 = SC - SR;
                v1 = v1.Normalize();
                v2 = ER - SR;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SCSRER += (double)Math.Acos(res);

                //SR-ER-WR
                v1 = SR - ER;
                v1 = v1.Normalize();
                v2 = WR - ER;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SRERWR += (double)Math.Acos(res);

                //ER-WR-HR
                v1 = ER - WR;
                v1 = v1.Normalize();
                v2 = HR - WR;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                ERWRHR += (double)Math.Acos(res);

                //SC-SL-EL
                v1 = SC - SL;
                v1 = v1.Normalize();
                v2 = EL - SL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SCSLEL += (double)Math.Acos(res);

                //SL-EL-WL
                v1 = SL - EL;
                v1 = v1.Normalize();
                v2 = WL - EL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SLELWL += (double)Math.Acos(res);

                //EL-WL-HL
                v1 = EL - WL;
                v1 = v1.Normalize();
                v2 = HL - WL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                ELWLHL += (double)Math.Acos(res);

                //Distance HR - HL
                DisHRHL += Vector3.Distance(HR, HL);
            }
            SRER /= 100;
            ERWR /= 100;
            WRHR /= 100;
            SLEL /= 100;
            ELWL /= 100;
            WLHL /= 100;
            HRHL /= 100;
            SCSRER /= 100;
            SRERWR /= 100;
            ERWRHR /= 100;
            SCSLEL /= 100;
            SLELWL /= 100;
            ELWLHL /= 100;
            DisHRHL /= 100;
            AllFeatureNormalize();
            if (!CheckFeatureOK())
            {
                if(isHaveNaN)
                    isNeedReboot = true;
                return;
            }
            result = true;
            return;
        }
    }
}
