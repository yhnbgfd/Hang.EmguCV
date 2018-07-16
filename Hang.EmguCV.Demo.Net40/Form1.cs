using Emgu.CV;
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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture();
            _cascadeClassifier = new CascadeClassifier(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml"));
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
                        imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them
                    }
                }
                imgCamUser.Image = imageFrame;
            }
        }

    }
}
