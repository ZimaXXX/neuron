using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Weight  {
        public double Value;
        public double LastChange;

        public Weight(): this(0){
        }

        public Weight(double value) {
            Value=value;
        }

        public void Reset(Func<double> weightGenerator) {
            LastChange=0;
            Value=weightGenerator();
        }

        public virtual void Correct(double delta) {
            LastChange=delta;
            double weight=Value;
            Value+=delta;
        }




    }
}
