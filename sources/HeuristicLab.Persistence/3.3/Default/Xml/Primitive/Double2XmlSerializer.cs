using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  public class Double2XmlSerializer : PrimitiveXmlSerializerBase<double> {

    public static double ParseG17(string s) {
      double d;
      if (double.TryParse(s,
        NumberStyles.AllowDecimalPoint |
        NumberStyles.AllowExponent |
        NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out d))
        return d;
      throw new FormatException(
        String.Format("Invalid G17 number format \"{0}\" could not be parsed", s));
    }

    public static string FormatG17(double d) {
      return d.ToString("g17", CultureInfo.InvariantCulture);
    }

    public override XmlString Format(double d) {
      return new XmlString(FormatG17(d));
    }

    public override double Parse(XmlString t) {
      return ParseG17(t.Data);
    }
  }

  
  

}