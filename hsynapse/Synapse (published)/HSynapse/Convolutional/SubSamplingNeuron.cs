using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class SubSamplingNeuron: NeuronBase {

        public SubSamplingNeuron() {
        }

        public override void Pulse(Connection sender,double value) {
            throw new NotImplementedException(); //todo
        }

        public override void BP(double error,double learnFactor) {
            throw new NotImplementedException(); //todo
        }


    }
}
