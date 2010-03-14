using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {

  /// <summary>
  /// A simple XML format, that can be used to either stream
  /// or save to a file.
  /// </summary>
  [StorableClass]
  public class XmlFormat : FormatBase<XmlString> {
    public override string Name { get { return "XML"; } }
  }

}