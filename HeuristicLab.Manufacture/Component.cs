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

  public class Component {
    public string Name { get; set; }
    public string Type { get; set; }
    public object Default { get; set; }
    public string Path { get; set; }
    public IList<object> Range { get; set; }

    public IList<Component> Parameters { get; set; }
    public IList<Component> Operators { get; set; }

    [JsonIgnore]
    public Component this[string index] {
      get {
        if (Parameters == null) return null;
        foreach (var p in Parameters)
          if (p.Name == index) return p;
        return null;
      }
      set {
        if (Parameters == null) 
          Parameters = new List<Component>();
        Component data = this[index];
        if (data != null && CheckConstraints(value))
          Merge(data, value);
        else
          Parameters.Add(value);
      }
    }

    public override bool Equals(object obj) {
      if (!(obj is Component)) 
        return false;
      else 
        return obj.Cast<Component>().Name == this.Name;
    }

    public override int GetHashCode() {
      return Name.GetHashCode();
    }

    [JsonIgnore]
    public IList<Component> ParameterizedItems { get; set; }

    [JsonIgnore]
    public Component Reference { get; set; }

    #region Helper
    public static void Merge(Component target, Component from) {
      target.Name = from.Name ?? target.Name;
      target.Type = from.Type ?? target.Type;
      target.Range = from.Range ?? target.Range;
      target.Path = from.Path ?? target.Path;
      target.Default = from.Default ?? target.Default;
      target.Reference = from.Reference ?? target.Reference;
      target.Parameters = from.Parameters ?? target.Parameters;
      target.ParameterizedItems = from.ParameterizedItems ?? target.ParameterizedItems;
    }

    private bool CheckConstraints(Component data) => 
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
