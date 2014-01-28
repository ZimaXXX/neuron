using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Connection: NetworkElement {
        public readonly Weight Weight;
        public readonly NeuronBase Next;
        public readonly Link Previous;

        public Connection(Link previous,NeuronBase next)
            : this(new Weight(),previous,next) {
        }

        public Connection(Weight weight,Link previous,NeuronBase next) {
            Utility.Verify(()=> weight!=null && previous!=null && next!=null);
            Weight=weight;
            Next=next;
            Previous=previous;
        }

        public double Calculate(double input) {
            return input*Weight.Value;
        }

        public override string GetDescription() {
            return "connection";
        }


    }
}
