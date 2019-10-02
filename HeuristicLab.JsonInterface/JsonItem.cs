using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class JsonItem {
    private IList<object> range;
    private object defaultValue;

    public string Name { get; set; }
    public string Type { get; set; }
    public object Default {
      get => defaultValue; 
      set {
        defaultValue = value;
        if(Range != null && value != null && !FulfillConstraints())
          throw new ArgumentOutOfRangeException("Default", "Default is not in range.");
      } 
    }
    public string Path { get; set; } = "";

    public IList<object> Range { 
      get => range; 
      set {
        range = value;
        if (Default != null && value != null && !FulfillConstraints())
          throw new ArgumentOutOfRangeException("Default", "Default is not in range.");
      } 
    }

    public IList<JsonItem> Parameters { get; set; }
    public IList<JsonItem> Operators { get; set; }
    
    public override bool Equals(object obj) => 
      (obj is JsonItem ? (obj.Cast<JsonItem>().Name == this.Name) : false);
      
    public override int GetHashCode() => Name.GetHashCode();

    public JsonItem Reference { get; set; }
    
    public static void Merge(JsonItem target, JsonItem from) {
      target.Name = from.Name ?? target.Name;
      target.Type = from.Type ?? target.Type;
      target.Range = from.Range ?? target.Range;
      target.Path = from.Path ?? target.Path;
      target.Default = from.Default ?? target.Default;
      target.Reference = from.Reference ?? target.Reference;
      target.Parameters = from.Parameters ?? target.Parameters;
      target.Operators = from.Operators ?? target.Operators;
    }

    public bool FulfillConstraints() => FulfillConstraints(this);

    public static bool FulfillConstraints(JsonItem data) =>
      data.Range != null && data.Default != null && (
      IsInRangeList(data.Range, data.Default) ||
      IsInNumericRange<long>(data.Default, data.Range[0], data.Range[1]) ||
      IsInNumericRange<int>(data.Default, data.Range[0], data.Range[1]) ||
      IsInNumericRange<short>(data.Default, data.Range[0], data.Range[1]) ||
      IsInNumericRange<byte>(data.Default, data.Range[0], data.Range[1]) ||
      IsInNumericRange<float>(data.Default, data.Range[0], data.Range[1]) ||
      IsInNumericRange<double>(data.Default, data.Range[0], data.Range[1]));

    public void UpdatePaths() {
      if (Parameters != null)
        foreach (var p in Parameters)
          p.Path = Path + "." + p.Name;

      if (Operators != null) 
        foreach (var p in Operators)
          p.Path = Path + "." + p.Name;
    }

    public void PrependPath(string str) {
      Path = $"{str}.{Path}";
      PrependPathHelper(Parameters, str);
      PrependPathHelper(Operators, str);
    }

    private void PrependPathHelper(IEnumerable<JsonItem> items, string str) {
      if (items != null)
        foreach (var p in items)
          p.PrependPath(str);
    }

    #region Helper
    private static bool IsInRangeList(IEnumerable<object> list, object value) {
      foreach (var x in list)
        if (x.Equals(value)) return true;
      return false;
    }

    private static bool IsInNumericRange<T>(object value, object min, object max) where T : IComparable =>
      (value != null && min != null && max != null && value is T && min is T && max is T &&
        (((T)min).CompareTo(value) == -1 || ((T)min).CompareTo(value) == 0) &&
        (((T)max).CompareTo(value) == 1 || ((T)max).CompareTo(value) == 0));
    #endregion
  }
}
