using System;
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
        private KinectSensor mainSensor;
        private Skeleton first;
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        const string path = "..\\..\\..\\DataSet\\";
        private string filename;
        bool result;
        int phase;

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

        public MainWindow()
        {
            InitializeComponent();
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
                    kinectManager.ElevationAngle = 0;
                    ColorView.KinectSensorManager =
                        DepthView.KinectSensorManager =
                        SkeletonView.KinectSensorManager =
                        KinectSetting.KinectSensorManager = kinectManager;
                    StatusDetail.Content = "Idle";
                    mainSensor.AllFramesReady += MainSensor_AllFramesReady;
                }
            }
            else StatusDetail.Content = "Kinect Not Found";
            PopulateFiles();
        }

        private void MainSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
                return;

            //Get a skeleton
            first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }
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

        private void PopulateFiles()
        {
            fileList.Items.Clear();
            DirectoryInfo dinfo = new DirectoryInfo(path);
            FileInfo[] Files = dinfo.GetFiles();
            foreach (FileInfo file in Files)
            {
                fileList.Items.Add(file.Name.Substring(0, file.Name.LastIndexOf(".")));
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
            progressBar.Value = 0;
            if (result)
            {
                StatusDetail.Content = "File Created";
                PopulateFiles();
            }
            else
                StatusDetail.Content = "Skeleton Not Found";
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
            sb.AppendLine("@relation gesture");
            sb.AppendLine("@attribute ERSRX real");
            sb.AppendLine("@attribute ERSRY real");
            sb.AppendLine("@attribute ERSRZ real");
            sb.AppendLine("@attribute WRERX real");
            sb.AppendLine("@attribute WRERY real");
            sb.AppendLine("@attribute WRERZ real");
            sb.AppendLine("@attribute HRWRX real");
            sb.AppendLine("@attribute HRWRY real");
            sb.AppendLine("@attribute HRWRZ real");
            sb.AppendLine("@attribute ELSLX real");
            sb.AppendLine("@attribute ELSLY real");
            sb.AppendLine("@attribute ELSLZ real");
            sb.AppendLine("@attribute WLELX real");
            sb.AppendLine("@attribute WLELY real");
            sb.AppendLine("@attribute WLELZ real");
            sb.AppendLine("@attribute HLWLX real");
            sb.AppendLine("@attribute HLWLY real");
            sb.AppendLine("@attribute HLWLZ real");
            sb.AppendLine("@attribute HLWRX real");
            sb.AppendLine("@attribute HLWRY real");
            sb.AppendLine("@attribute HLWRZ real");
            sb.AppendLine("@attribute SCSRER real");
            sb.AppendLine("@attribute SRERWR real");
            sb.AppendLine("@attribute ERWRHR real");
            sb.AppendLine("@attribute SCSLEL real");
            sb.AppendLine("@attribute SLELWL real");
            sb.AppendLine("@attribute ELWLHL real");
            sb.AppendLine("@attribute DisHRHL real\n");
            sb.AppendLine("@data");

            int progress = 0;
            (sender as BackgroundWorker).ReportProgress(progress);
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
            for (int i = 0; i < 10; i++)
            {
                progress += 10;
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
                SCSRER = Math.Acos(res);

                //SR-ER-WR
                v1 = SR - ER;
                v1 = v1.Normalize();
                v2 = WR - ER;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SRERWR = Math.Acos(res);

                //ER-WR-HR
                v1 = ER - WR;
                v1 = v1.Normalize();
                v2 = HR - WR;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                ERWRHR = Math.Acos(res);

                //SC-SL-EL
                v1 = SC - SL;
                v1 = v1.Normalize();
                v2 = EL - SL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SCSLEL = Math.Acos(res);

                //SL-EL-WL
                v1 = SL - EL;
                v1 = v1.Normalize();
                v2 = WL - EL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                SLELWL = Math.Acos(res);

                //EL-WL-HL
                v1 = EL - WL;
                v1 = v1.Normalize();
                v2 = HL - WL;
                v2 = v2.Normalize();
                res = Vector3.DotProduct(v1, v2);
                ELWLHL = Math.Acos(res);

                //Distance HR - HL
                DisHRHL = Vector3.Distance(HR, HL);

                (sender as BackgroundWorker).ReportProgress(progress);
                Thread.Sleep(1000);
            }
            SRER /= 10;
            ERWR /= 10;
            WRHR /= 10;
            SLEL /= 10;
            ELWL /= 10;
            WLHL /= 10;
            HRHL /= 10;
            SCSRER /= 10;
            SRERWR /= 10;
            ERWRHR /= 10;
            SCSLEL /= 10;
            SLELWL /= 10;
            ELWLHL /= 10;
            DisHRHL /= 10;
            sb.Append(String.Format("{0};{1};{2};", SRER.X.ToString("0.00000"), SRER.Y.ToString("0.00000"), SRER.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};{1};{2};", ERWR.X.ToString("0.00000"), ERWR.Y.ToString("0.00000"), ERWR.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};{1};{2};", WRHR.X.ToString("0.00000"), WRHR.Y.ToString("0.00000"), WRHR.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};{1};{2};", SLEL.X.ToString("0.00000"), SLEL.Y.ToString("0.00000"), SLEL.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};{1};{2};", ELWL.X.ToString("0.00000"), ELWL.Y.ToString("0.00000"), ELWL.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};{1};{2};", WLHL.X.ToString("0.00000"), WLHL.Y.ToString("0.00000"), WLHL.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};{1};{2};", HRHL.X.ToString("0.00000"), HRHL.Y.ToString("0.00000"), HRHL.Z.ToString("0.00000")));
            sb.Append(String.Format("{0};", SCSRER.ToString("0.00000")));
            sb.Append(String.Format("{0};", SRERWR.ToString("0.00000")));
            sb.Append(String.Format("{0};", ERWRHR.ToString("0.00000")));
            sb.Append(String.Format("{0};", SCSLEL.ToString("0.00000")));
            sb.Append(String.Format("{0};", SLELWL.ToString("0.00000")));
            sb.Append(String.Format("{0};", ELWLHL.ToString("0.00000")));
            sb.Append(String.Format("{0}", DisHRHL.ToString("0.00000")));

            result = true;
            sb.Replace(",", ".");
            sb.Replace(";", ",");
            int count = 1;
            string fullPath = @path + filename;
            while (File.Exists(fullPath))
            {
                string[] temp = filename.Split('.');
                count++;
                filename = String.Format("{0}.{1}.arff", temp[0], count.ToString());
                fullPath = @path + filename; 
            }
            File.AppendAllText(fullPath, sb.ToString());

            return;
        }

        private void createButton_click(object sender, RoutedEventArgs e)
        {
            if(fileName.Text.Length < 3)
            {
                StatusDetail.Content = "Gesture name must have 3 or more character";
                return;
            }
            filename = fileName.Text + ".1.arff";
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
    }
}
