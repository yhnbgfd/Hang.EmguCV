using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Hang.EmguCV.Demo.Net40
{
    public partial class Form1 : Form
    {
        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;
        private FaceRecognizer _faceRecognizer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture();
            _cascadeClassifier = new CascadeClassifier(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml"));
            _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);


            //
            var faceImages = new Image<Gray, byte>[1];
            var faceLabels = new int[1];

            var faceImage = new Image<Gray, byte>(new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "999a707d-87e6-4f0f-8cbb-0fa1f44ce6f1.bmp")));

            faceImages[0] = faceImage;//.Resize(100, 100, Inter.Cubic);
            faceLabels[0] = 123;
            _faceRecognizer.Train(faceImages, faceLabels);
            _faceRecognizer.Write(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "faceRecognizer.dat"));
            _faceRecognizer.Read(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "faceRecognizer.dat"));
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (imageFrame != null)
                {
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here

                    foreach (var face in faces)
                    {
                        var result = _faceRecognizer.Predict(grayframe);
                        imageFrame.Draw(face, new Bgr(Color.BurlyWood), 2); //the detected face(s) is highlighted here using a box that is drawn around it/them
                    }
                    imgCamUser.Image = imageFrame;
                }

                //var faceToSave = new Image<Gray, byte>(imageFrame.Bitmap);
                //var filePath = Application.StartupPath + String.Format("/{0}.bmp", Guid.NewGuid().ToString());
                //faceToSave.ToBitmap().Save(filePath);
            }
        }

    }
}
