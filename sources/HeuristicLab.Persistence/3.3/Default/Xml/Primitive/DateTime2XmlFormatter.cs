using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  [EmptyStorableClass]
  public class DateTime2XmlFormatter : FormatterBase<DateTime, XmlString> {

    public override XmlString Format(DateTime dt) {
      return new XmlString(dt.Ticks.ToString());
    }

    public override DateTime Parse(XmlString x) {
      return new DateTime(long.Parse(x.Data));
    }

  }


}