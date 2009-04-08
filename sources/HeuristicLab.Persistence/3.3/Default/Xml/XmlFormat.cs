using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml {

  [EmptyStorableClass]
  public class XmlFormat : FormatBase {
    public override string Name { get { return "XML"; } }
    public static readonly XmlFormat Instance = new XmlFormat();
    private XmlFormat() { }
  }

}