using System.Collections;
using System.Collections.Generic;
using System;
using HeuristicLab.Persistence.Core;

namespace HeuristicLab.Persistence.Default.Xml.Compact {

  [EmptyStorableClass]
  public class IntList2XmlFormatter : NumberEnumeration2XmlFormatterBase {

    public override Type Type {
      get {
        return typeof(List<int>);
      }
    }

    protected override void Add(IEnumerable enumeration, object o) {
      ((List<int>)enumeration).Add((int)o);
    }

    protected override object Instantiate() {
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