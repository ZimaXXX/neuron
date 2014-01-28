using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse.Samples {

    class SignalInspectorSample {
        static void Main(string[] args) {
            TrainingData[] xor=new TrainingData[] {
                new TrainingData(new double[] {1,0}, new double[] {1}),
                new TrainingData(new double[] {0,1}, new double[] {1}),
                new TrainingData(new double[] {1,1}, new double[] {0}),
                new TrainingData(new double[] {0,0}, new double[] {0}),
            };

            int[] layers = new int[] { 2,2,1 };
            MultilayerPerceptron mlp=new MLPGenerator().Create(layers,1,new Sigmoid());
            mlp.Inspector=new SignalInspector();

            //shows the neurons processing in VS Output window
            mlp.Inspector.Debug=true;
            mlp.Inspector.TraceLevel=3;
            mlp.LearnFactor=0.9;
            var response=mlp.BP(new BPRequest(xor,1000));

            Console.ReadKey();
        }
    }
}
