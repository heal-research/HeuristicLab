using System.Collections;
using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Core;
using System.Globalization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [EmptyStorableClass]
  public class DoubleList2XmlSerializer : NumberEnumeration2XmlSerializerBase<List<double>> {

    protected override void Add(IEnumerable enumeration, object o) {
      ((List<double>)enumeration).Add((int)o);
    }

    protected override IEnumerable Instantiate() {
      return new List<double>();
    }

    protected override string FormatValue(object o) {
      return ((double)o).ToString("r", CultureInfo.InvariantCulture);
    }

    protected override object ParseValue(string o) {
      return double.Parse(o);
    }

  }
}