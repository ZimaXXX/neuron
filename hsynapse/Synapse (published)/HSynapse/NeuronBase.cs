using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public abstract class NeuronBase: Link {
        public Connection[] Previous;
        public IContinuousActivator Func;

        public bool IsOutput {
            get {
                return this.Next==null;
            }
        }

        public abstract void Pulse(Connection sender,double value);
        public abstract void BP(double error,double learnFactor);


    }
}
