using System.Collections;
using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml.Primitive;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  public class DoubleList2XmlSerializer : NumberEnumeration2XmlSerializerBase<List<double>> {

    protected override void Add(IEnumerable enumeration, object o) {
      ((List<double>)enumeration).Add((double)o);
    }

    protected override IEnumerable Instantiate() {
      return new List<double>();
    }

    protected override string FormatValue(object o) {
      return Double2XmlSerializer.FormatG17((double)o);
    }

    protected override object ParseValue(string o) {
      return Double2XmlSerializer.ParseG17(o);
    }

  }
}