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

    #region Private Fields
    private string name;
    private object value;
    private IList<object> range;
    #endregion
    
    public string Name { 
      get => name; 
      set {
        name = value;
        Path = Name;
        UpdatePath();
      } 
    }
    public string Type { get; set; }
    public string Path { get; set; }
    public IList<JsonItem> Parameters { get; set; }
    public IList<JsonItem> Operators { get; set; }
    public object Value {
      get => value; 
      set {
        this.value = value;
        CheckConstraints();
      } 
    }
    public IList<object> Range { 
      get => range; 
      set {
        range = value;
        CheckConstraints();
      } 
    }
    public string ActualName { get; set; }

    #region JsonIgnore Properties
    [JsonIgnore]
    public JsonItem Reference { get; set; }

    [JsonIgnore]
    public bool IsConfigurable => (Value != null && Range != null);

    [JsonIgnore]
    public bool IsParameterizedItem => Parameters != null;
    #endregion

    #region Public Static Methods
    public static void Merge(JsonItem target, JsonItem from) {
      target.Name = from.Name ?? target.Name;
      target.Type = from.Type ?? target.Type;
      target.Range = from.Range ?? target.Range;
      target.Path = from.Path ?? target.Path;
      target.Value = from.Value ?? target.Value;
      target.Reference = from.Reference ?? target.Reference;
      target.ActualName = from.ActualName ?? target.ActualName;
      target.Parameters = from.Parameters ?? target.Parameters;
      target.Operators = from.Operators ?? target.Operators;
    }
    #endregion

    #region Public Methods
    public void UpdatePath() {
      if (Parameters != null)
        UpdatePathHelper(Parameters);

      if (Operators != null)
        UpdatePathHelper(Operators);

      if(Reference != null)
        UpdatePathHelper(Reference);
    }
    #endregion

    #region Helper
    private void UpdatePathHelper(params JsonItem[] items) => 
      UpdatePathHelper((IEnumerable<JsonItem>)items);

    private void UpdatePathHelper(IEnumerable<JsonItem> items) {
      foreach (var item in items) {
        item.Path = $"{Path}.{item.Name}";
        item.UpdatePath();
      }
    }

    private void CheckConstraints() {
      if (Range != null && Value != null && !IsInRange())
        throw new ArgumentOutOfRangeException("Default", "Default is not in range.");
    }


    private bool IsInRange() => IsInRangeList() || IsInNumericRange();

    private bool IsInRangeList() {
      foreach (var x in Range)
        if (x.Equals(Value)) return true;
      return false;
    }

    private bool IsInNumericRange() =>
      IsInNumericRange<long>()
      || IsInNumericRange<int>()
      || IsInNumericRange<short>()
      || IsInNumericRange<byte>()
      || IsInNumericRange<float>()
      || IsInNumericRange<double>();

    private bool IsInNumericRange<T>() where T : IComparable {
      object value = Value, min = Range[0], max = Range[1];
      return value != null && min != null && max != null && value is T && min is T && max is T &&
            (((T)min).CompareTo(value) == -1 || ((T)min).CompareTo(value) == 0) &&
            (((T)max).CompareTo(value) == 1 || ((T)max).CompareTo(value) == 0);
    }
      
    #endregion
  }
}