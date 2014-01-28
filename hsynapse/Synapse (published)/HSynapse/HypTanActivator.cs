using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class HyperbolicTangent:IContinuousActivator {

        public HyperbolicTangent() {
        }

        #region IContinuousActivator Members

        public double Activation(double value) {
            return Math.Tanh(value);
        }

        public double Gradient(double value) {
            double fx=Activation(value);
            return 1 - (fx*fx);
        }

        #endregion
    }
}
