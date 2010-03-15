using System.Collections;
using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  internal sealed class IntList2XmlSerializer : NumberEnumeration2XmlSerializerBase<List<int>> {

    protected override void Add(IEnumerable enumeration, object o) {
      ((List<int>)enumeration).Add((int)o);
    }

    protected override IEnumerable Instantiate() {
      return new List<int>();
    }

    protected override string FormatValue(object o) {
      return o.ToString();
    }

    protected override object ParseValue(string o) {
      return int.Parse(o);
    }
  }

}