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
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Image<Gray, byte> image2 = new Image<Gray, byte>(@"D:\Temp\431224198711172917.bmp");
            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>())
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here

                    foreach (var face in faces)
                    {
                        //var result = CvInvoke.CompareHist(grayframe, image2, HistogramCompMethod.Bhattacharyya);
                        var result = CvInvoke.MatchShapes(grayframe, image2, ContoursMatchType.I3);

                        imageFrame.Draw(result.ToString("0.0000"), new Point(face.X, face.Y), FontFace.HersheySimplex, 1.1, new Bgr(Color.Red));
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
