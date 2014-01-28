using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace HAKGERSoft.Synapse {

    public class MLPXMLSerializer: NetworkXMLSerializer {

        public XDocument Serialize(MultilayerPerceptron network) {
            return new XDocument(new XElement("root",
                new XElement("layers",network.Structure.Layers.Length.ToString()),
                GetElements(network.Structure)));
        }

        public MultilayerPerceptron Deserialize(XDocument xdoc){
            NetworkStructure structure=GetStructure(xdoc.Element("root"));
            return new MultilayerPerceptron(structure);
        }




    }
}
