using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows.Forms;

namespace Hang.EmguCV.Demo.Net40
{
    public partial class Form1 : Form
    {
        private HaarCascade haar;

        private Capture _capture;
        private IntPtr HistImg2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string haarXmlPath = @"haarcascade_frontalface_alt_tree.xml";
            haar = new HaarCascade(haarXmlPath);
            _capture = new Capture(0);


            int[] hist_size = new int[1] { 256 };//建一个数组来存放直方图数据

            Image<Bgr, Byte> image2 = new Image<Bgr, byte>(@"D:\Temp\TIM截图20180824163809.bmp");
            MCvAvgComp[] faces2 = image2.Convert<Gray, byte>().DetectHaarCascade(haar)[0];
            if (faces2.Length == 0)
            {
                throw new Exception("图里没人！");
            }
            image2 = image2.Copy(faces2[0].rect);
            Image<Gray, Byte> imageGray2 = image2.Convert<Gray, Byte>();
            Image<Gray, Byte> imageThreshold2 = imageGray2.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
            //Contour<Point> contour2 = imageThreshold2.FindContours();
            HistImg2 = CvInvoke.cvCreateHist(1, hist_size, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1);
            IntPtr[] inPtr2 = new IntPtr[1] { imageThreshold2 };
            CvInvoke.cvCalcHist(inPtr2, HistImg2, false, IntPtr.Zero);
            CvInvoke.cvNormalizeHist(HistImg2, 1d);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            using (var imageFrame = _capture.QueryFrame())
            {
                if (imageFrame == null) return;

                imgCamUser.Image = imageFrame;

                int[] hist_size = new int[1] { 256 };//建一个数组来存放直方图数据

                //准备轮廓  
                Image<Bgr, Byte> image1 = imageFrame;// new Image<Bgr, byte>(@"D:\Temp\1.bmp");
                MCvAvgComp[] faces = image1.Convert<Gray, byte>().DetectHaarCascade(haar)[0];
                if (faces.Length == 0)
                {
                    return;
                }

                image1 = image1.Copy(faces[0].rect);
                Image<Gray, Byte> imageGray1 = image1.Convert<Gray, Byte>();
                Image<Gray, Byte> imageThreshold1 = imageGray1.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
                //Contour<Point> contour1 = imageThreshold1.FindContours();
                IntPtr HistImg1 = CvInvoke.cvCreateHist(1, hist_size, Emgu.CV.CvEnum.HIST_TYPE.CV_HIST_ARRAY, null, 1); //创建一个空的直方图

                IntPtr[] inPtr1 = new IntPtr[1] { imageThreshold1 };
                CvInvoke.cvCalcHist(inPtr1, HistImg1, false, IntPtr.Zero); //计算inPtr1指向图像的数据，并传入HistImg1中
                double compareResult;
                Emgu.CV.CvEnum.HISTOGRAM_COMP_METHOD compareMethod = Emgu.CV.CvEnum.HISTOGRAM_COMP_METHOD.CV_COMP_BHATTACHARYYA;
                CvInvoke.cvNormalizeHist(HistImg1, 1d); //直方图对比方式 
                compareResult = CvInvoke.cvCompareHist(HistImg1, HistImg2, compareMethod);
                //compareResult = CvInvoke.cvMatchShapes(HistImg1, HistImg2, Emgu.CV.CvEnum.CONTOURS_MATCH_TYPE.CV_CONTOURS_MATCH_I3, 1d); 
                var str = string.Format("成对几何直方图匹配（匹配方式：{0}），结果：{1:F05}", compareMethod.ToString("G"), compareResult);
                Console.WriteLine(str);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
        }
    }
}
