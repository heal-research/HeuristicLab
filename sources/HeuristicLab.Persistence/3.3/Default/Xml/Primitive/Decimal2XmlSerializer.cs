using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  internal sealed class Decimal2XmlSerializer : PrimitiveXmlSerializerBase<decimal> {

    public static decimal ParseG30(string s) {
      decimal d;
      if (decimal.TryParse(s,
        NumberStyles.AllowDecimalPoint |
        NumberStyles.AllowExponent |
        NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d))
        return d;
      throw new FormatException(
        String.Format("Invalid decimal G30 number format \"{0}\" could not be parsed", s));
    }

    public static string FormatG30(decimal d) {
      return d.ToString("g30", CultureInfo.InvariantCulture);
    }

    public override XmlString Format(decimal d) {
      return new XmlString(FormatG30(d));
    }

    public override decimal Parse(XmlString t) {
      return ParseG30(t.Data);
    }
  }
}