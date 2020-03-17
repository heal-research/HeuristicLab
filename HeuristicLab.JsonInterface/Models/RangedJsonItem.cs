using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public abstract class RangedJsonItem<T> : JsonItem, IRangedJsonItem<T>
    where T : IComparable {
    public T MinValue { get; set; }
    public T MaxValue { get; set; }
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected override ValidationResult Validate() {
      IList<string> errors = new List<string>();
      bool successMin = (Minimum.CompareTo(MinValue) <= 0 && Maximum.CompareTo(MinValue) >= 0);
      bool successMax = (Minimum.CompareTo(MaxValue) <= 0 && Maximum.CompareTo(MaxValue) >= 0);
      if (!successMin) errors.Add($"[{Path}]: Value {MinValue} is not between {Minimum} and {Maximum}.");
      if (!successMax) errors.Add($"[{Path}]: Value {MaxValue} is not between {Minimum} and {Maximum}.");
      return new ValidationResult((successMin && successMax), errors);

    }
      
  }
}
