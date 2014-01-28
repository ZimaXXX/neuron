using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HAKGERSoft.Synapse {

    public abstract class LayerConnector {
        public abstract void Connect(Link[] prior,NeuronBase[] next, Bias bias); 

    }

}
