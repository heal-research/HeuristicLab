using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  internal sealed class DateTime2XmlSerializer : PrimitiveXmlSerializerBase<DateTime> {

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