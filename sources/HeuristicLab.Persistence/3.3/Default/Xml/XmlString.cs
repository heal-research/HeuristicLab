using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml {
  
  public class XmlString : ISerialData {

    [Storable]
    public string Data { get; private set; }

    public XmlString(string data) {
      Data = data;
    }
  }
}