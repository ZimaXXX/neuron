using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace HAKGERSoft.Synapse {

    public class ConvolutionalXMLSerializer: NetworkXMLSerializer {

        public XDocument Serialize(ConvolutionalNetwork network) {
            return new XDocument(new XElement("root",
                new XElement("layers",network.Structure.Layers.Length.ToString()),
                GetElements(network.Structure)));
        }

        public ConvolutionalNetwork Deserialize(XDocument xdoc) {
            NetworkStructure structure=GetStructure(xdoc.Element("root"));
            return new ConvolutionalNetwork(structure);
        }




    }
}
