using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Link: Unit {
        public Connection[] Next;
        internal double LastOutputSignal;
        protected double LastInputSignal;

        public virtual void Reset(Func<double> weightGenerator) {
            LastInputSignal=LastOutputSignal=0;
            if(Next!=null)
                foreach(Connection connection in Next)
                    connection.Weight.Reset(weightGenerator);
        }

        public void Split(double value){
            LastInputSignal=LastOutputSignal=value;
            if(Inspector!=null && Inspector.TraceLevel>=1)
                Inspector.Trace(this,"Got signal {0}, split it to {1} neurons",value,Next.Length);
            foreach(Connection i in Next)
                i.Next.Pulse(i,value); 
        }

    }
}
