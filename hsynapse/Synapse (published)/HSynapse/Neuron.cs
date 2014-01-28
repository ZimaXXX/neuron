using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Neuron: NeuronBase {
        double PendingValue,PendingError;
        int SignalsReceived;

        public Neuron():this(new Sigmoid()) {
        }

        public Neuron(IContinuousActivator activator) {
            Utility.Verify(activator,x => x!=null,"activator");
            Func=activator;
        }

        public override void Pulse(Connection sender,double value){
            PendingValue+=sender.Calculate(value);
            if(++SignalsReceived<Previous.Length)
                return;
            SignalsReceived=0;
            double output=Func.Activation(PendingValue);
            LastInputSignal=PendingValue;
            LastOutputSignal=output;
            if(Inspector!=null && Inspector.TraceLevel>=1)
                Inspector.Trace(this,"Input sum={0}, output={1}",PendingValue,output);
            if(Next!=null)
                foreach(Connection i in Next)
                    i.Next.Pulse(i,output);
            PendingValue=0;
        }

        public override void BP(double error,double learnFactor){
            BP(error,learnFactor,0);
        }

        public void BP(double error,double learnFactor,double momentum) {
            PendingError+=error;
            if(Next!=null && ++SignalsReceived<Next.Length)
                return;
            double gradient=Func.Gradient(LastInputSignal);
            double delta=PendingError*gradient;
            PendingError=SignalsReceived=0;
            if(Inspector!=null && Inspector.TraceLevel>=1)
                Inspector.Trace(this,"BP - last input={0},gradient={1},delta={2}",LastInputSignal,gradient,delta);
            foreach(Connection x in Previous) {
                double w=x.Weight.Value;
                x.Weight.Correct(delta*x.Previous.LastOutputSignal*learnFactor + momentum*x.Weight.LastChange);
                if(x.Previous is Neuron)
                    ((Neuron)x.Previous).BP(delta*w,learnFactor,momentum);
            }
        }




    }
}
