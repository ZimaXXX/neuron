using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HAKGERSoft.Synapse;

namespace HAKGERSoft.Synapse.Samples {
    public partial class Form1:Form {
        List<Tuple<int,int>> Map=new List<Tuple<int,int>>();
        Bitmap Bitmap;
        Graphics Graph;

        public Form1() {
            InitializeComponent();
            Bitmap=new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            Graph = Graphics.FromImage(Bitmap);
        }

        void DrawPoint(int x,int y){
            Graph.DrawRectangle(new Pen(new SolidBrush(Color.Black)),x,y,4,4);
        }

        private void pictureBox1_MouseClick(object sender,MouseEventArgs e) {
            Map.Add(new Tuple<int,int>(e.X,e.Y));
            DrawPoint(e.X,e.Y);
            pictureBox1.Image=Bitmap;
        }

        private void button1_Click(object sender,EventArgs e) {
            Map.Clear();
            Graph.Clear(Color.White);
            pictureBox1.Image=Bitmap;
        }

        private void button2_Click(object sender,EventArgs e) {
            int cycles=int.Parse(textBox1.Text);
            int hiddenNeurons=int.Parse(textBox2.Text);
            double learnRate=double.Parse(textBox3.Text);
            double momentum=double.Parse(textBox4.Text);
            int[] layers = new int[] { 1,hiddenNeurons,1 };
            MultilayerPerceptron ann=new MLPGenerator().Create(layers,1,new Sigmoid(2));
            ann.Reset(-1,1);
            ann.Momentum=momentum;
            ann.LearnFactor=learnRate;
            List<TrainingData> trainingList=new List<TrainingData>();

            for(int i=0;i<Map.Count;i++) {
                double[] input=new double[] { (double)Map[i].Item1/(double)pictureBox1.Width };
                double[] output=new double[] { (double)Map[i].Item2/(double)pictureBox1.Height };
                trainingList.Add(new TrainingData(input,output));
            }

            var res=ann.BP(new BPRequest(trainingList.ToArray(),cycles));
            for(int i=0;i<pictureBox1.Width;i++) {
                int x = i;
                double yd = ann.Pulse(new double[] { (double)i/(double)pictureBox1.Width })[0];
                int y=Convert.ToInt32(yd*pictureBox1.Height);
                Point p= new Point(x,y);
                Graph.DrawRectangle(new Pen(new SolidBrush(Color.Orange)),p.X,p.Y,1,1);
            }
            pictureBox1.Image=Bitmap;
        }

    }
}
