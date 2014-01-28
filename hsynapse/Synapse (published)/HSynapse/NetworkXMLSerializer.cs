using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace HAKGERSoft.Synapse {

    public abstract class NetworkXMLSerializer:NetworkSerializer {
        
        protected NetworkStructure GetStructure(XElement root){
            var unitElems=root.Elements("unit");
            var weightElems=root.Elements("weight");
            Dictionary<string,Tuple<Link,int>> unitsMap=GetUnits(unitElems).ToDictionary(pair => pair.Item1.GetDescription() ,pair => pair);
            Dictionary<string,Weight> weightsMap=GetWeights(weightElems).ToDictionary(pair => pair.Item1,pair => pair.Item2);
            var connectionElems=root.Elements("connection");
            Dictionary<Link,List<Connection>> mapNext=new Dictionary<Link,List<Connection>>();
            Dictionary<Link,List<Connection>> mapPrevious=new Dictionary<Link,List<Connection>>();
            
            foreach(XElement element in connectionElems){
                Connection c=new Connection(weightsMap[element.Value],unitsMap[element.Attribute("from").Value].Item1,(NeuronBase)unitsMap[element.Attribute("to").Value].Item1);
                if(!mapNext.ContainsKey(c.Previous))
                    mapNext.Add(c.Previous,new List<Connection>());
                mapNext[c.Previous].Add(c);
                if(!mapPrevious.ContainsKey(c.Next))
                    mapPrevious.Add(c.Next,new List<Connection>());
                mapPrevious[c.Next].Add(c);
            }
            Link[] structure=unitsMap.Select(x => x.Value.Item1).ToArray();
            foreach(Link link in structure) {
                if(mapNext.ContainsKey(link))
                    link.Next=mapNext[link].ToArray();
                if(link is NeuronBase)
                    if(mapPrevious.ContainsKey(link))
                        ((NeuronBase)link).Previous=mapPrevious[link].ToArray();
            }
            Bias bias=unitsMap.Select(x => x.Value.Item1).FirstOrDefault(x => x is Bias) as Bias;

            int layersCount=int.Parse(root.Element("layers").Value);
            List<List<Link>> layers=new List<List<Link>>(layersCount);
            for(var i=0;i<layersCount;i++)
                layers.Add(new List<Link>());

            foreach(var kvp in unitsMap) {
                int l=kvp.Value.Item2;
                if(l!=-1)
                    layers[l].Add(kvp.Value.Item1);
            }
            return new NetworkStructure(layers.Select(x => x.ToArray()).ToArray(),bias);
        }

        protected IEnumerable<Tuple<Link,int>> GetUnits(IEnumerable<XElement> elements){
            foreach(XElement element in elements) {
                Type type=Type.GetType(Aliases[element.Attribute("type").Value]);
                Link unit=(Link)Activator.CreateInstance(type);
                unit.Identity=element.Value;
                if(unit is NeuronBase) {
                    Type funcType=Type.GetType(Aliases[element.Attribute("func").Value]);
                    IContinuousActivator func=(IContinuousActivator)Activator.CreateInstance(funcType);
                    ((NeuronBase)unit).Func=func;
                }
                int l=-1;
                if(element.Attribute("layer")!=null)
                    l=int.Parse(element.Attribute("layer").Value);
                yield return new Tuple<Link,int>(unit,l);
            }
        }

        protected IEnumerable<Tuple<string,Weight>> GetWeights(IEnumerable<XElement> elements) {
            foreach(XElement element in elements) {
                Type type=Type.GetType(Aliases[element.Attribute("type").Value]);
                Weight weight=(Weight)Activator.CreateInstance(type);
                weight.Value=double.Parse(element.Value);
                string index=element.Attribute("index").Value;
                yield return new Tuple<string,Weight>(index,weight);
            }
        }

        protected IEnumerable<XElement> GetElements(NetworkStructure structure) {
            HashSet<Connection> hsc=new HashSet<Connection>();
            int l=0;
            foreach(Link[] layer in structure.Layers) {
                foreach(Link link in layer) {
                    XElement elem=new XElement("unit",
                        new XAttribute("type",Aliases[link.GetType().ToString()]),
                        new XAttribute("layer",l.ToString()),
                        link.GetDescription());
                    if(link is NeuronBase)
                        elem.Add(new XAttribute("func",Aliases[((NeuronBase)link).Func.GetType().ToString()]));
                    yield return elem;

                    if(link.Next!=null)
                        foreach(Connection connection in link.Next)
                            hsc.Add(connection);
                    if(link is NeuronBase)
                        if(((NeuronBase)link).Previous!=null)
                            foreach(Connection connection in ((NeuronBase)link).Previous)
                                hsc.Add(connection);
                }
                l++;
            }
            if(structure.Bias!=null) {
                yield return new XElement("unit",
                    new XAttribute("type",Aliases[structure.Bias.GetType().ToString()]),
                    new XAttribute("value",structure.Bias.Value.ToString("R")),
                    structure.Bias.GetDescription());
                foreach(Connection connection in structure.Bias.Next)
                    hsc.Add(connection);

            }
            HashSet<Weight> wsw=new HashSet<Weight>();
            Dictionary<Weight,int> weightsDict=new Dictionary<Weight,int>();
            int idx=1;
            foreach(Connection connection in hsc) {
                if(!weightsDict.ContainsKey(connection.Weight))
                    weightsDict.Add(connection.Weight,idx++);
                yield return new XElement("connection",
                    new XAttribute("from",connection.Previous.GetDescription()),
                    new XAttribute("to",connection.Next.GetDescription()),
                    weightsDict[connection.Weight].ToString()
                    );
            }
            foreach(var kvp in weightsDict)
                yield return new XElement("weight",new XAttribute("type",Aliases[kvp.Key.GetType().ToString()]),new XAttribute("index",kvp.Value.ToString()),kvp.Key.Value.ToString("R"));
        }



    }
}
