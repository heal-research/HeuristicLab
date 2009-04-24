using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Xml {
  
  public abstract class XmlFormatterBase<T> : FormatterBase<T, XmlString> { }

}