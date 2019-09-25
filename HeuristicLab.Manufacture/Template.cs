using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json;

namespace HeuristicLab.Manufacture {   
  /*
  public class ParameterData { //Blueprint, Component,  ?
    public string Name { get; set; }
    public string Type { get; set; }
    public IList<ParameterData> FreeParameters { get; set; }
    public IList<ParameterData> StaticParameters { get; set; }

  }*/

  public class ParameterData {
    public string Name { get; set; }
    public string Type { get; set; }
    public object Default { get; set; }
    public string Path { get; set; }
    public IList<object> Range { get; set; }

    public IList<ParameterData> Parameters { get; set; }
    public IList<ParameterData> Operators { get; set; }

    [JsonIgnore]
    public ParameterData this[string index] {
      get {
        if (Parameters == null) return null;
        foreach (var p in Parameters)
          if (p.Name == index) return p;
        return null;
      }
      set {
        if (Parameters == null) 
          Parameters = new List<ParameterData>();
        ParameterData data = this[index];
        if (data != null && CheckConstraints(value))
          Merge(data, value);
        else
          Parameters.Add(value);
      }
    }

    public override bool Equals(object obj) {
      if (!(obj is ParameterData)) 
        return false;
      else 
        return obj.Cast<ParameterData>().Name == this.Name;
    }

    public override int GetHashCode() {
      return Name.GetHashCode();
    }

    [JsonIgnore]
    public IList<ParameterData> ParameterizedItems { get; set; }

    [JsonIgnore]
    public ParameterData Reference { get; set; }

    #region Helper
    public static void Merge(ParameterData target, ParameterData from) {
      target.Name = from.Name ?? target.Name;
      target.Type = from.Type ?? target.Type;
      target.Range = from.Range ?? target.Range;
      target.Path = from.Path ?? target.Path;
      target.Default = from.Default ?? target.Default;
      target.Reference = from.Reference ?? target.Reference;
      target.Parameters = from.Parameters ?? target.Parameters;
      target.ParameterizedItems = from.ParameterizedItems ?? target.ParameterizedItems;
    }

    private bool CheckConstraints(ParameterData data) => 
      data.Range != null && data.Default != null && (
      IsInRangeList(data.Range, data.Default) ||
      IsInNumericRange<long>(data.Default, data.Range[0], data.Range[1]) ||
      IsInNumericRange<double>(data.Default, data.Range[0], data.Range[1]));

    private bool IsInRangeList(IEnumerable<object> list, object value) {
      foreach (var x in list)
        if (x.Equals(value)) return true;
      return false;
    }

    private bool IsInNumericRange<T>(object value, object min, object max) where T : IComparable =>
      (value != null && min != null && max != null && value is T && min is T && max is T &&
        (((T)min).CompareTo(value) == -1 || ((T)min).CompareTo(value) == 0) &&
        (((T)max).CompareTo(value) == 1 || ((T)max).CompareTo(value) == 0));
    #endregion
  }
}
