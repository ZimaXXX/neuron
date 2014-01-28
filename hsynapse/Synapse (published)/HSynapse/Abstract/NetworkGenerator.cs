using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAKGERSoft.Synapse {

    public abstract class NetworkGenerator {

        protected void SetIdentity(Link[][] structure) {
            var idx=0;
            for(int i=0;i<structure.Length;i++) {
                for(int j=0;j<structure[i].Length;j++) {
                    string pref=i==0?"Link":"Neuron";
                    structure[i][j].Identity=string.Format("{0}.{1}, L{2}",pref,idx++,i);
                }
            }
        }
        


    }
}