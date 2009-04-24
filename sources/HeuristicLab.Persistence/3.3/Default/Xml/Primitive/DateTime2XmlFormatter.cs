using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class DateTime2XmlFormatter : PrimitiveXmlFormatterBase<DateTime> {

    public override XmlString Format(DateTime dt) {
      return new XmlString(dt.Ticks.ToString());
    }

    public override DateTime Parse(XmlString x) {
      try {
        return new DateTime(long.Parse(x.Data));
      } catch (Exception e) {
        throw new PersistenceException("Exception caugth while trying to reconstruct DateTime object.", e);
      }
    }

  }


}