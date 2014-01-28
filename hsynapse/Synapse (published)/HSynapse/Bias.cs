using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Bias: Link {
        public readonly double Value;

        public Bias():this(1){
        }

        public Bias(double value){
            Value=value;
            this.Identity="Bias";
        }

        //public static explicit operator double(Bias @this) {
        //    return @this.Value;
        //}

        //public static implicit operator Bias(double value) {
        //    return new Bias(value);
        //}

        public void Pulse() {
            LastOutputSignal=LastInputSignal=Value;
            if(Inspector!=null && Inspector.TraceLevel>=1)
                Inspector.Trace(this,"pulse signal {0} to {1} neurons",Value,Next.Length);
            foreach(Connection i in Next)
                i.Next.Pulse(i, Value);
        }


    }
}
