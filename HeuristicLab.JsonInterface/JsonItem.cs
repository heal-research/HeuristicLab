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
          if (Path != null) {
            var parts = Path.Split('.');
            for (int i = 0; i < parts.Length; ++i)
              if (parts[i] == oldName)
                parts[i] = Name;

            Path = string.Join(".", parts);
          } else
            Path = Name;
          
          UpdatePath();
        }
      }
    }
    public string Type { get; set; }
    public string Path { get; set; }
    public IList<JsonItem> Parameters { get; set; } // -> für flachen aufbau -> childs?
    public IList<JsonItem> Operators { get; set; }
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
    public void AddParameter(JsonItem item) {
      if (Parameters == null)
        Parameters = new List<JsonItem>();
      Parameters.Add(item);
      item.Path = $"{Path}.{item.Name}";
      item.UpdatePath();
    }

    public void UpdatePath() {
      if (Parameters != null)
        UpdatePathHelper(Parameters);

      if (Operators != null)
        UpdatePathHelper(Operators);

      if (Reference != null)
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
    public static JsonItem BuildJsonItem(JObject obj, IDictionary<string, string> typeList) {
      object val = obj[nameof(Value)]?.ToObject<object>();
      if (val is JContainer) {
        //try {
          val = ((JContainer)val).ToObject<object[]>();
        /*} catch (Exception) { }
        try {
          val = ((JContainer)val).ToObject<object[,]>();
        } catch (Exception) { }*/
      }
        

      return new JsonItem() {
        Name = obj[nameof(Name)]?.ToString(),
        Path = obj[nameof(Path)]?.ToString(),
        Value = val,
        Range = obj[nameof(Range)]?.ToObject<object[]>(),
        Type = GetType(obj[nameof(Path)]?.ToObject<string>(), typeList),
        ActualName = obj[nameof(ActualName)]?.ToString(),
        Parameters = PopulateParameters(obj, typeList),
        Operators = PopulateOperators(obj, typeList)
      };
  }

    private static string GetType(string path, IDictionary<string, string> typeList) {
      if (!string.IsNullOrEmpty(path))
        if (typeList.TryGetValue(path, out string value))
          return value;
      return null;
    }

    private static IList<JsonItem> PopulateParameters(JObject obj, IDictionary<string, string> typeList) {
      IList<JsonItem> list = new List<JsonItem>();

      // add staticParameters
      if (obj[Constants.StaticParameters] != null)
        foreach (JObject param in obj[Constants.StaticParameters])
          list.Add(BuildJsonItem(param, typeList));

      // merge staticParameter with freeParameter
      if (obj[Constants.FreeParameters] != null) {
        foreach (JObject param in obj[Constants.FreeParameters]) {
          JsonItem tmp = BuildJsonItem(param, typeList);

          // search staticParameter from list
          JsonItem comp = null;
          foreach (var p in list)
            if (p.Name == tmp.Name) comp = p;
          if (comp == null)
            throw new InvalidDataException($"Invalid {Constants.FreeParameters.Trim('s')}: '{tmp.Name}'!");

          JsonItem.Merge(comp, tmp);
        }
      }
      return list;
    }

    private static IList<JsonItem> PopulateOperators(JObject obj, IDictionary<string, string> typeList) {
      IList<JsonItem> list = new List<JsonItem>();
      JToken operators = obj[nameof(JsonItem.Operators)];
      if (operators != null)
        foreach (JObject sp in operators)
          list.Add(BuildJsonItem(sp, typeList));
      return list;
    }
    #endregion
  }
}