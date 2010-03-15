using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  /// <summary>
  /// Common base class for primitive XML serializers.
  /// </summary>
  /// <typeparam name="T">The source type being serialized to XMl.</typeparam>
  public abstract class PrimitiveXmlSerializerBase<T> : XmlSerializerBase<T> { }

}