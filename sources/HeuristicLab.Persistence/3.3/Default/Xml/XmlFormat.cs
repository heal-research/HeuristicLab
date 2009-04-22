using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {

  [EmptyStorableClass]
  public class XmlFormat : FormatBase<XmlString> {
    public override string Name { get { return "XML"; } }
  }

}