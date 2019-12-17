using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Main data class for json interface.
  /// </summary>
  public class JsonItem {

    #region Private Fields
    private string name;
    private object value;
    private IEnumerable<object> range;
    #endregion

    public string Name {
      get => name;
      set {
        if(Name != value) {
          string oldName = Name;
          name = value;
          // replace name in path if path != null
          if (Path != null) {
            var parts = Path.Split('.');
            parts[Array.IndexOf(parts, oldName)] = name;
            Path = string.Join(".", parts);
          } else
            Path = Name;
          
          UpdatePath();
        }
      }
    }
    public string Path { get; set; }
    public IList<JsonItem> Children { get; set; }
    public object Value {
      get => value;
      set {
        this.value = value;
        CheckConstraints();
      }
    }
    public IEnumerable<object> Range {
      get => range;
      set {
        range = value;
        CheckConstraints();
      }
    }
    public string ActualName { get; set; }

    #region Public Static Methods
    public static void Merge(JsonItem target, JsonItem from) {
      target.Name = from.Name ?? target.Name;
      target.Range = from.Range ?? target.Range;
      target.Path = from.Path ?? target.Path;
      target.Value = from.Value ?? target.Value;
      target.ActualName = from.ActualName ?? target.ActualName;
      if(target.Children != null) {
        if (from.Children != null)
          ((List<JsonItem>)from.Children).AddRange(target.Children); 
      } else {
        target.Children = from.Children;
      }
    }
    #endregion

    #region Public Methods
    public void AddParameter(JsonItem item) {
      if (Children == null)
        Children = new List<JsonItem>();
      Children.Add(item);
      item.Path = $"{Path}.{item.Name}";
      item.UpdatePath();
    }

    public void UpdatePath() {
      if (Children != null)
        UpdatePathHelper(Children);
    }
    #endregion

    #region Helper
    private void UpdatePathHelper(IEnumerable<JsonItem> items) {
      foreach (var item in items) {
        item.Path = $"{Path}.{item.Name}";
        item.UpdatePath();
      }
    }

    private void CheckConstraints() {
      if (Range != null && Value != null && !IsInRange())
        throw new ArgumentOutOfRangeException(nameof(Value), $"{nameof(Value)} is not in range.");
    }

    private bool IsInRange() {
      bool b1 = true, b2 = true;
      if (Value is IEnumerable && !(Value is string)) {
        foreach (var x in (IEnumerable)Value) {
          b1 = b1 ? IsInRangeList(x) : b1;
          b2 = b2 ? IsInNumericRange(x) : b2;
        }
      } 
      else {
        b1 = IsInRangeList(Value); 
        b2 = IsInNumericRange(Value);
      } 
      return b1 || b2;
    }

    private bool IsInRangeList(object value) {
      foreach (var x in Range)
        if (x.Equals(value)) return true;
      return false;
    }

    private bool IsInNumericRange(object value) =>
      IsInNumericRange<ulong>(value)
      || IsInNumericRange<uint>(value)
      || IsInNumericRange<ushort>(value)
      || IsInNumericRange<long>(value)
      || IsInNumericRange<int>(value)
      || IsInNumericRange<short>(value)
      || IsInNumericRange<byte>(value)
      || IsInNumericRange<float>(value)
      || IsInNumericRange<double>(value)
      || (value is float && float.IsNaN((float)value))
      || (value is double && double.IsNaN((double)value));

    private bool IsInNumericRange<T>(object value) where T : IComparable {
      object min = Range.First(), max = Range.Last();
      return
        value != null && min != null && max != null && value is T && min is T && max is T &&
        (((T)min).CompareTo(value) == -1 || ((T)min).CompareTo(value) == 0) &&
        (((T)max).CompareTo(value) == 1 || ((T)max).CompareTo(value) == 0);
    }
    #endregion

    #region BuildJsonItemMethods
    public static JsonItem BuildJsonItem(JObject obj) {
      object val = obj[nameof(Value)]?.ToObject<object>();
      if (val is JContainer jContainer) // for resolving array values
        val = jContainer.ToObject<object[]>();
        
      return new JsonItem() {
        Name = obj[nameof(Name)]?.ToString(),
        Path = obj[nameof(Path)]?.ToString(),
        Value = val,
        Range = obj[nameof(Range)]?.ToObject<object[]>(),
        ActualName = obj[nameof(ActualName)]?.ToString()
      };
    }
    #endregion
  }
}