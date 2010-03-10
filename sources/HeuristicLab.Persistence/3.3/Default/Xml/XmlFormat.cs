using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {

  [StorableClass(StorableClassType.Empty)]
  public class XmlFormat : FormatBase<XmlString> {
    public override string Name { get { return "XML"; } }
  }

}