using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public class MultilayerPerceptron: FeedForwardNetwork {

        public MultilayerPerceptron(NetworkStructure structure)
            : base(structure) {
        }

        public MultilayerPerceptron(NetworkStructure structure,double lo,double hi)
            : this(structure) {
            Reset(lo,hi);
        }

        double _Mtm=0;
        public double Momentum{
            get {
                return _Mtm;
            }
            set {
                Utility.Verify(value,x => x<=1 && x>=0,"Invalid momentum value");
                _Mtm=value;
            }
        }

        public double[] Pulse(double[] values) {
            Utility.Verify(values,x => x.Length==Structure.Input.Length,"Invalid values length");
            if(this.Structure.Bias!=null)
                this.Structure.Bias.Pulse();
            for(int i=0;i!=values.Length;i++)
                Structure.Input[i].Split(values[i]);
            return Structure.Output.Select(x => x.LastOutputSignal).ToArray();
        }

        public double GetPulseError(double[] input, double[] sample) {
            Utility.Verify(sample,x => Structure.Output.Length==x.Length,"Invalid sample vector");
            return MSE(Pulse(input), sample);
        }

        public BPResponse BP(BPRequest bpr) {
            Utility.Verify(() => bpr!=null,"BP bpr");
            Stopwatch StopWatch=new Stopwatch();
            StopWatch.Start();
            bool trace=this.Inspector!=null;
            if(trace)
                Inspector.Trace("Backpropagation start, MaxEpochs={0}, MinEpochs={1}",bpr.MinEpochs.ToString(),bpr.MaxEpochs.ToString());
            BPResponseEstimate estimate= BPResponseEstimate.Unknown;

            NeuronBase[] output=Structure.Output;
            List<BPEpochResponse> bper=new List<BPEpochResponse>(bpr.MaxEpochs);

            for(int i=0;i<bpr.MaxEpochs;i++) {
                bool succ=true;
                if(trace)
                    Inspector.Trace("Backpropagation- start epoch {0}",(i+1).ToString());
                TrainingData[] ts= bpr.GetTrainingData().ToArray();
                List<double> errors=new List<double>(ts.Length);

                for(int j=0; j<ts.Length; j++){
                    bool testPass=true;
                    if(ts[j].TestingSets!=null && ts[j].TestingSets.Any(x => GetPulseError(x,ts[j].Output)>bpr.MaxMSE))
                        testPass=false;
                    double mse=GetPulseError(ts[j].Input,ts[j].Output);
                    errors.Add(mse);

                    if(mse>bpr.MaxMSE || !testPass) {
                        for(var k=0;k!=output.Length;k++)
                            ((Neuron)output[k]).BP(ts[j].Output[k]-output[k].LastOutputSignal,LearnFactor,Momentum);
                        succ=false;
                    }
                }
                double eps=GetTotalEps(errors.ToArray());
                if(trace)
                    Inspector.Trace("End of epoch {0}, total Eps={1}",(i+1).ToString(),eps);

                bper.Add(new BPEpochResponse(errors.ToArray(),eps));
                if(i>=bpr.MinEpochs && succ) {
                    estimate=BPResponseEstimate.Success;
                    break;
                }
                if(i>=bpr.MaxEpochs-1){
                    if(!succ)
                        estimate=BPResponseEstimate.Failure;
                    break;
                }
            }
            StopWatch.Stop();

            BPResponse response=new BPResponse(estimate,bper.ToArray(),StopWatch.Elapsed);
            return response;
        }

        double GetTotalEps(double[] errors){
            double sum=0;
            foreach(double e in errors)
                sum+=e;
            return sum/2;
        }

        double MSE(double[] vec, double[] sample) {
            double sum=0;
            Utility.EachPair(sample, vec, (s,v) => sum += Math.Pow((s-v), 2));
            return sum/2;
            //return Math.Sqrt(sum/sample.Length);
        }

        string ValuesToString(double[] values) {
            return string.Join(" - ",values.Select(x => x.ToString("0.00")).ToArray());
        }

        


    }
}
