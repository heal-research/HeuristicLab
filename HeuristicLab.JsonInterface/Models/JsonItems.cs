using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class IntJsonItem : JsonItem<int> {}
  public class IntArrayJsonItem: JsonItem<int[], int> {}

  public class DoubleJsonItem: JsonItem<double> {}
  public class DoubleArrayJsonItem: JsonItem<double[], double> {}

  public class BoolJsonItem: JsonItem<bool> {}

  public class StringJsonItem: JsonItem<string> {}

  public class DateTimeJsonItem: JsonItem<DateTime> {}
}
