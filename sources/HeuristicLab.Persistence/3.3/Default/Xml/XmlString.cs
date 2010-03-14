using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {

  [StorableClass]  
  public class XmlString : ISerialData {

    [Storable]
    public string Data { get; private set; }

    private XmlString() { }

    public XmlString(string data) {
      Data = data;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("XmlString(").Append(Data).Append(')');
      return sb.ToString();
    }
  }
}