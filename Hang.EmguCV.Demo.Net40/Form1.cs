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
        private bool _saveNext = false;

        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;
        private FaceRecognizer _faceRecognizer = null;

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

            //
            var files = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceImages")).GetFiles();
            if (files.Length > 0)
            {
                _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);

                var faceImages = new Image<Gray, byte>[files.Length];
                var faceLabels = new int[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    var faceImage = new Image<Gray, byte>(new Bitmap(files[i].FullName));
                    faceImages[i] = faceImage;
                    faceLabels[i] = i;
                }

                _faceRecognizer.Train(faceImages, faceLabels);
                _faceRecognizer.Write(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "faceRecognizer.dat"));
                //_faceRecognizer.Read(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "faceRecognizer.dat"));
            }
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
                        var result = _faceRecognizer?.Predict(grayframe);
                        imageFrame.Draw(result?.Label.ToString(), new Point(face.X, face.Y), FontFace.HersheySimplex, 1.1, new Bgr(Color.Green));
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
