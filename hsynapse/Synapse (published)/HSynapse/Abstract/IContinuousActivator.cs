using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

   public interface IContinuousActivator: IActivator {
       double Gradient(double value);
   }

}
