using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class DateTimeJsonItem : IntervalRestrictedValueJsonItem<DateTime> {
    protected override bool Validate() => Minimum.CompareTo(Value) >= 0 && Maximum.CompareTo(Value) <= 0;
  }
}
