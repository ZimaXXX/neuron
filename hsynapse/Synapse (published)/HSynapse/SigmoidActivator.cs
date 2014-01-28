using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Sigmoid:IContinuousActivator {
        double Beta;

        public Sigmoid(): this(1) {
        }

        public Sigmoid(double beta) {
            Beta = beta;
        }

        #region IContinuousActivator Members

        public double Activation(double value) {
            return 1/(1+Math.Pow(Math.E, -Beta*value));
        }

        public double Gradient(double value) {
            double fx=Activation(value);
            return Beta*fx*(1-fx);
        }

        #endregion
    }
}
