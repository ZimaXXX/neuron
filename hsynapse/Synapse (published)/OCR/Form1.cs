using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HAKGERSoft.Synapse;
using System.Xml.Linq;
using System.Threading;

namespace HAKGERSoft.Synapse.Samples {
    public partial class Form1:Form {
        ConvolutionalNetwork Network;
        Graphics UserGraph;
        Bitmap UserBitmap;

        bool Loaded=false;
        bool Stopped=true;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender,EventArgs e) {
            BuildNetwork();
            ClearUserInput();
        }

        void BuildNetwork(){
            int[] layers=new int[] {
                841,1014,1250,100,10
            };
            LayerConnector[] map=new LayerConnector[] {
                new ConvolutionAuto(5,6,1,3),
                new ConvolutionAuto(5,50,6,1),
                new FullLayerConnector(),
                new FullLayerConnector()
            };
            ConvolutionalTopology topology=new ConvolutionalTopology(layers,1,map,new HyperbolicTangent());
            ConvolutionalGenerator generator=new ConvolutionalGenerator();
            Network=generator.Create(topology);
            Network.LearnFactor=0.0005;
            Network.Reset(-0.1,0.1);
        }

        void ClearUserInput() {
            UserBitmap=new Bitmap(UserPictureBox.Width,UserPictureBox.Height);
            UserGraph=Graphics.FromImage(UserBitmap);
            UserGraph.Clear(Color.White);
            Pen pen=new Pen(Brushes.Red);
            pen.DashStyle=System.Drawing.Drawing2D.DashStyle.Dot;
            int margin=30;
            UserGraph.DrawRectangle(pen,margin,margin,UserPictureBox.Width-margin*2-3,UserPictureBox.Height-margin*2-3);
            UserPictureBox.Image=UserBitmap;
        }

        private void pictureBox6_MouseMove(object sender,MouseEventArgs e) {
            if(e.Button!=MouseButtons.Left) return;
            Rectangle rectEllipse=new Rectangle();
            rectEllipse.X=e.X-1;
            rectEllipse.Y=e.Y-1;
            rectEllipse.Width=2;
            rectEllipse.Height=2;
            UserGraph.FillEllipse(new SolidBrush(Color.Black),e.X,e.Y,13,13);
            UserPictureBox.Image=UserBitmap;
        }

        private void button1_Click(object sender,EventArgs e) {
            ClearUserInput();
        }

        private void button2_Click(object sender,EventArgs e) {
            //recon
            if(!Loaded)
                MessageBox.Show("Load the Network first!","Warning");
            double[] input=null;
            using(Bitmap cropped=Digitalizer.AutoCrop(UserBitmap)) {
                using(Bitmap sized=Digitalizer.Resize(cropped,17,20)) {
                    using(Bitmap bm=new Bitmap(29,29)) {
                        Graphics gr=Graphics.FromImage(bm);
                        gr.Clear(Color.White);
                        gr.DrawImage(sized,4,4);
                        input=Digitalizer.GetInput(bm,0,0,bm.Width,bm.Height, x => x.Name=="ff000000").ToArray();
                    }
                }
            }
            double[] output=Network.Pulse(input);
            DrawReconResults(output);
        }

        void DrawReconResults(double[] output){
            int idx=0;
            Tuple<int,double>[] sorted=output.Select(x => new Tuple<int,double>(idx++,x)).OrderByDescending(x => x.Item2).ToArray();
            label2.Text="Recognized as: "+sorted[0].Item1.ToString();

            foreach(var tuple in sorted) {
                int index=tuple.Item1;
                double value=tuple.Item2;
                //digits
                Bitmap bitmap=new Bitmap(OutputBox0.Width,OutputBox0.Height);
                Graphics graph=Graphics.FromImage(bitmap);
                graph.DrawString(index.ToString(),new Font("Arial",14),new SolidBrush(GetOutputDigitColor(value)),3,0);
                PictureBox pb= (PictureBox)this.Controls.Find("OutputBox"+index.ToString(),true)[0];
                pb.Image=bitmap;
                //graphs
                Bitmap bar=new Bitmap(OutputGraph0.Width,OutputGraph0.Height);
                Graphics barGraph=Graphics.FromImage(bar);
                int len=Convert.ToInt32((((value+1)*OutputGraph0.Width)/2));
                barGraph.DrawLine(new Pen(Brushes.Green,OutputGraph0.Height),0,0,len,0);
                PictureBox gb= (PictureBox)this.Controls.Find("OutputGraph"+index.ToString(),true)[0];
                gb.Image=bar;
            }
        }

        Color GetOutputDigitColor(double value){
            value+=1;
            var alpha=Convert.ToInt32((255*value/(2)));
            return Color.FromArgb(alpha,Color.Black);
        }

        private void button3_Click(object sender,EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists=true;
            dlg.CheckPathExists=true;
            dlg.Filter="xml files (*.xml)|*.xml";
            dlg.Multiselect=false;
             if (dlg.ShowDialog()==DialogResult.OK) {
                 ConvolutionalXMLSerializer serializer=new ConvolutionalXMLSerializer();
                 Network=serializer.Deserialize(XDocument.Load(dlg.FileName));
                 Network.LearnFactor=0.0005;
                 Loaded=true;
             }
        }

        private void button4_Click(object sender,EventArgs e) {
            ConvolutionalXMLSerializer serializer=new ConvolutionalXMLSerializer();
            XDocument doc=serializer.Serialize(Network);
            doc.Save("ocr.xml");
        }

        private void button5_Click(object sender,EventArgs e) {
            //start/stop
            if(Stopped)
                StartLearning();
            else
                StopLearning();
        }

        //util

        void StartLearning(){
            Stopped=false;
            button5.Text="Stop";
            Log.Clear();
            Thread.CurrentThread.Priority=ThreadPriority.Lowest;
            Image[] bigimgs=Utility.Generate(i => Image.FromFile(@"mnist_train"+i.ToString()+".jpg"),10).ToArray();

            int idx=0,total=0;
            List<TrainingData> trainingList=new List<TrainingData>();
            foreach(double[] input in GetAllImages(bigimgs)) {
                if(Stopped)
                    return;
                double[] output=Digitalizer.GetOutput(10,idx).ToArray();
                DrawDigitalized(input,LearnDigitBox);
                Application.DoEvents();
                trainingList.Add(new TrainingData(input,output));
                GC.Collect();
                GC.WaitForPendingFinalizers();
                idx= (idx+1)%10;
                if(idx==0) {
                    total++;
                    BPRequest request=new BPRequest(trainingList.ToArray(),1) { ShuffleTrainingSet=true };
                    BPResponse response= Network.BP(request);
                    double eps=response.Epochs[0].Epsilon;
                    double ms=response.BPTime.TotalMilliseconds;
                    Log.Text+=string.Format("Process cycle {0}, error={1}, time[ms]={2}\r\n",total.ToString(),eps.ToString("0.000"),ms.ToString("0.000"));
                    trainingList.Clear();
                }
            }
            StopLearning();
        }

        void StopLearning(){
            Stopped=true;
            button5.Text="Start";
        }

        static IEnumerable<double[]> GetAllImages(Image[] bigs) {
            int w=28;
            int h=28;
            int size=2072;
            for(int y=0;y<size-h-2;y+=h) {
                for(int x=0;x<size-w-2;x+=w) {
                    Rectangle rec=new Rectangle(x,y,w+1,h+1);
                    foreach(Image img in bigs)
                        yield return Digitalizer.GetInput((Bitmap)img,x,y,29,29,c=>c.B>100).ToArray();
                }
            }
        }

        static void DrawDigitalized(double[] input,PictureBox pb) {
            int w=pb.Width;
            int h=pb.Height;
            Bitmap bitmap=new Bitmap(w,h);
            Graphics graph=Graphics.FromImage(bitmap);

            for(int y=0;y<h;y++) {
                for(int x=0;x<w;x++) {
                    double value=input[y*h+x];
                    if(value>0)
                        graph.DrawRectangle(new Pen(new SolidBrush(Color.Black)),x,y,1,1);
                }
            }
            pb.Image=bitmap;
        }

        

    }
}
