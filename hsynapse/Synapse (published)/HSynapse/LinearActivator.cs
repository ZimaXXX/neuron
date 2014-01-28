using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public class Linear:IContinuousActivator {
        double A;

        public Linear(): this(1) {
        }

        public Linear(double a) {
            A = a;
        }

        #region IContinuousActivator Members

        public double Activation(double value) {
            return A*value;
        }

        public double Gradient(double value) {
            return A;
        }

        #endregion
    }
}
