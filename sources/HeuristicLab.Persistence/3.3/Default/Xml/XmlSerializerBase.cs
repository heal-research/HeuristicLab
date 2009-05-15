using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {
  
  public abstract class XmlSerializerBase<T> : PrimitiveSerializerBase<T, XmlString> { }

}