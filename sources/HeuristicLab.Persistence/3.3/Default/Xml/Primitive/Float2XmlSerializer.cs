using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Reflection;
using System.Globalization;

namespace HeuristicLab.Persistence.Default.Xml.Primitive {

  public class Float2XmlSerializer : PrimitiveXmlSerializerBase<float> {

    public static float ParseG8(string s) {
      float f;
      if (float.TryParse(s,
        NumberStyles.AllowDecimalPoint |
        NumberStyles.AllowExponent |
        NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out f))
        return f;
      throw new FormatException(
        String.Format("Invalid float G8 number format \"{0}\" could not be parsed", s));
    }

    public static string FormatG8(float f) {
      return f.ToString("g8", CultureInfo.InvariantCulture);
    }

    public override XmlString Format(float f) {
      return new XmlString(FormatG8(f));
    }

    public override float Parse(XmlString t) {
      return ParseG8(t.Data);
    }
  }
}