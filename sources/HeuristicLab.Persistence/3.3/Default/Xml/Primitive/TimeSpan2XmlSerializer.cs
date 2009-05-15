using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Globalization;


namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class TimeSpan2XmlSerializer : PrimitiveXmlSerializerBase<TimeSpan> {

    public override XmlString Format(TimeSpan o) {
      return new XmlString(o.ToString());
    }

    public override TimeSpan Parse(XmlString t) {
      try {
        return TimeSpan.Parse(t.Data);
      } catch (FormatException x) {
        throw new PersistenceException("Cannot parse TimeSpan string representation.", x);
      } catch (OverflowException x) {
        throw new PersistenceException("Overflow during TimeSpan parsing.", x);
      }
    }
  }  
}