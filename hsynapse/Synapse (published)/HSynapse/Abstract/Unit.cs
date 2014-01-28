using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HAKGERSoft.Synapse {

    public abstract class Unit: NetworkElement {
        internal string Identity;

        public override string GetDescription() {
            return Identity;
        }
    }
}
