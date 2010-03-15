using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {

  /// <summary>
  /// Common base class for all primitive XML Serializers.
  /// </summary>
  /// <typeparam name="T">The source type being serialized to XML.</typeparam>
  public abstract class XmlSerializerBase<T> : PrimitiveSerializerBase<T, XmlString> { }

}