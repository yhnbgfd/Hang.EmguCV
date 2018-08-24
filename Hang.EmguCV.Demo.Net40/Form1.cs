using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Hang.EmguCV.Demo.Net40
{
    public partial class Form1 : Form
    {
        private bool _saveNext = false;

        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;
        private Image<Gray, byte> image2;

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
            _capture = new VideoCapture();
            _cascadeClassifier = new CascadeClassifier(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml"));

            image2 = new Image<Gray, byte>(@"D:\Temp\TIM截图20180815174747.bmp");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            int[] hist_size = new int[1] { 256 };//建一个数组来存放直方图数据
            Image<Bgr, Byte> image1 = new Image<Bgr, byte>("D:\\code\\picture\\frunck.jpg");
            Image<Bgr, Byte> image2 = new Image<Bgr, byte>("D:\\code\\picture\\lena.jpg");
            var faces1 = _cascadeClassifier.DetectMultiScale(image1.Convert<Gray, byte>(), 1.4, 1, new Size(20, 20), Size.Empty);
            var faces2 = _cascadeClassifier.DetectMultiScale(image2.Convert<Gray, byte>(), 1.4, 1, new Size(20, 20), Size.Empty);

            int l1 = faces1.Length;
            int l2 = faces2.Length;
            image1 = image1.Copy(faces1[0]);
            image2 = image2.Copy(faces2[0]);
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
            CvInvoke.CalcHist(inPtr1, HistImg1, false, IntPtr.Zero); //计算inPtr1指向图像的数据，并传入HistImg1中
            CvInvoke.cvCalcHist(inPtr2, HistImg2, false, IntPtr.Zero);




            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>())
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here

                    foreach (var face in faces)
                    {
                        var ret1 = CvInvoke.CompareHist(grayframe, image2, HistogramCompMethod.Bhattacharyya);
                        //var ret2 = CvInvoke.MatchShapes(grayframe, image2, ContoursMatchType.I3);

                        imageFrame.Draw(ret1.ToString("0.0000"), new Point(face.X, face.Y), FontFace.HersheySimplex, 1.1, new Bgr(Color.Red));
                        imageFrame.Draw(face, new Bgr(Color.BurlyWood), 1); //the detected face(s) is highlighted here using a box that is drawn around it/them
                    }
                    imgCamUser.Image = imageFrame;

                    if (_saveNext == true)
                    {
                        _saveNext = false;
                        var faceToSave = new Image<Gray, byte>(imageFrame.Bitmap);
                        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceImages", $"{Guid.NewGuid().ToString()}.bmp");
                        faceToSave.ToBitmap().Save(filePath);
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _saveNext = true;
        }

    }
}
