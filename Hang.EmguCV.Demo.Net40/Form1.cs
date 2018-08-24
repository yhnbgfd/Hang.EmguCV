using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Hang.EmguCV.Demo.Net40
{
    public partial class Form1 : Form
    {
        private bool _saveNext = false;

        private HaarCascade haar;

        public Form1()
        {
            InitializeComponent();
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceImages");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            haar = new HaarCascade(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml"));
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            string haarXmlPath = @"haarcascade_frontalface_alt_tree.xml";
            HaarCascade haar = new HaarCascade(haarXmlPath);
            int[] hist_size = new int[1] { 256 };//建一个数组来存放直方图数据
                                                 //IntPtr img1 = CvInvoke.cvLoadImage("", Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_ANYCOLOR); //根据路径导入图像

            //准备轮廓  
            Image<Bgr, Byte> image1 = new Image<Bgr, byte>("D:\\code\\picture\\frunck.jpg");
            Image<Bgr, Byte> image2 = new Image<Bgr, byte>("D:\\code\\picture\\lena.jpg");
            var aaa = image1.Convert<Gray, byte>();
            MCvAvgComp[] faces = image1.Convert<Gray, byte>().DetectHaarCascade(haar)[0];
            MCvAvgComp[] faces2 = image2.Convert<Gray, byte>().DetectHaarCascade(haar)[0];
            //MCvAvgComp[] faces = haar.Detect(image1.Convert<Gray, byte>(), 1.4, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20), Size.Empty);
            //MCvAvgComp[] faces2 = haar.Detect(image2.Convert<Gray, byte>(), 1.4, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20), Size.Empty);

            int l1 = faces.Length;
            int l2 = faces2.Length;
            image1 = image1.Copy(faces[0].rect);
            image2 = image2.Copy(faces2[0].rect);
            Image<Gray, Byte> imageGray1 = image1.Convert<Gray, Byte>();
            Image<Gray, Byte> imageGray2 = image2.Convert<Gray, Byte>();
            Image<Gray, Byte> imageThreshold1 = imageGray1.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
            Image<Gray, Byte> imageThreshold2 = imageGray2.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
            //Contour<Point> contour1 = imageThreshold1.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL);
            Contour<Point> contour1 = imageThreshold1.FindContours();
            Contour<Point> contour2 = imageThreshold2.FindContours();
            IntPtr HistImg1 = CvInvoke.cvCreateHist(1, hist_size, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1); //创建一个空的直方图
            IntPtr HistImg2 = CvInvoke.cvCreateHist(1, hist_size, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1);

            //CvInvoke.cvHaarDetectObjects();

            IntPtr[] inPtr1 = new IntPtr[1] { imageThreshold1 };
            IntPtr[] inPtr2 = new IntPtr[1] { imageThreshold2 };
            CvInvoke.cvCalcHist(inPtr1, HistImg1, false, IntPtr.Zero); //计算inPtr1指向图像的数据，并传入HistImg1中
            CvInvoke.cvCalcHist(inPtr2, HistImg2, false, IntPtr.Zero);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double compareResult;
            Emgu.CV.CvEnum.HISTOGRAM_COMP_METHOD compareMethod = Emgu.CV.CvEnum.HISTOGRAM_COMP_METHOD.CV_COMP_BHATTACHARYYA;
            CvInvoke.cvNormalizeHist(HistImg1, 1d); //直方图对比方式 
            CvInvoke.cvNormalizeHist(HistImg2, 1d);
            compareResult = CvInvoke.cvCompareHist(HistImg1, HistImg2, compareMethod);
            //compareResult = CvInvoke.cvMatchShapes(HistImg1, HistImg2, Emgu.CV.CvEnum.CONTOURS_MATCH_TYPE.CV_CONTOURS_MATCH_I3, 1d); 
            sw.Stop();
            double time = sw.Elapsed.TotalMilliseconds;
            //return string.Format("成对几何直方图匹配（匹配方式：{0}），结果：{1:F05}，用时：{2:F05}毫秒\r\n", compareMethod.ToString("G"), compareResult, time);

            //using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
            //{
            //    if (imageFrame != null)
            //    {
            //        var grayframe = imageFrame.Convert<Gray, byte>();
            //        var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here

            //        foreach (var face in faces)
            //        {
            //            var result = _faceRecognizer?.Predict(grayframe);
            //            imageFrame.Draw(result?.Label.ToString(), new Point(face.X, face.Y), FontFace.HersheySimplex, 1.1, new Bgr(Color.Green));
            //            imageFrame.Draw(face, new Bgr(Color.BurlyWood), 1); //the detected face(s) is highlighted here using a box that is drawn around it/them
            //        }
            //        imgCamUser.Image = imageFrame;

            //        if (_saveNext == true)
            //        {
            //            _saveNext = false;
            //            var faceToSave = new Image<Gray, byte>(imageFrame.Bitmap);
            //            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceImages", $"{Guid.NewGuid().ToString()}.bmp");
            //            faceToSave.ToBitmap().Save(filePath);
            //        }
            //    }
            //}
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _saveNext = true;
        }
    }
}
