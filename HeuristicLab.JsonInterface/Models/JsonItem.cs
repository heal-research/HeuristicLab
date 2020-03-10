using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Main data class for json interface.
  /// </summary>
  public abstract class JsonItem : IJsonItem {

    public class JsonItemValidator : IJsonItemValidator {
      private IDictionary<int, bool> Cache = new Dictionary<int, bool>();
      private JsonItem Root { get; set; }
      public JsonItemValidator(JsonItem root) {
        Root = root;
      }

      public bool Validate(ref IList<IJsonItem> faultyItems) {
        faultyItems = new List<IJsonItem>();
        return ValidateHelper(Root, ref faultyItems);
      }

      // TODO: return ValidationResult ?
      private bool ValidateHelper(JsonItem item, ref IList<IJsonItem> faultyItems) {
        int hash = item.GetHashCode();
        if (Cache.TryGetValue(hash, out bool r))
          return r;

        bool res = item.Validate();
        if (!res) faultyItems.Add(item);
        Cache.Add(hash, res);
        if(item.Children != null) {
          foreach (var child in item.Children)
            if (!ValidateHelper(child as JsonItem, ref faultyItems))
              res = false && res;
        }
        return res;
      }
    }

    public virtual string Name { get; set; }

    public virtual string Description { get; set; }

    private string fixedPath = "";
    public virtual string Path {
      get {
        if (!string.IsNullOrWhiteSpace(fixedPath))
          return fixedPath;

        IJsonItem tmp = Parent;
        StringBuilder builder = new StringBuilder(this.Name);
        while(tmp != null) {
          builder.Insert(0, tmp.Name + ".");
          tmp = tmp.Parent;
        }
        return builder.ToString();
      }
    }

    //public virtual object Value { get; set; }

    //public virtual IEnumerable<object> Range { get; set; }
    
    // TODO jsonIgnore dataType?

    [JsonIgnore]
    public virtual IList<IJsonItem> Children { get; protected set; }

    [JsonIgnore]
    public virtual IJsonItem Parent { get; set; }

    [JsonIgnore]
    public virtual bool Active { get; set; }

    #region Constructors
    public JsonItem() { }

    public JsonItem(IEnumerable<IJsonItem> childs) {
      AddChildren(childs);
    }
    #endregion
    
    #region Public Methods
    public void AddChildren(params IJsonItem[] childs) => 
      AddChildren(childs as IEnumerable<IJsonItem>);

    public void AddChildren(IEnumerable<IJsonItem> childs) {
      if (childs == null) return;
      if (Children == null)
        Children = new List<IJsonItem>();
      foreach (var child in childs) {
        Children.Add(child);
        child.Parent = this;
      }
    }

    public virtual IJsonItemValidator GetValidator() => new JsonItemValidator(this);

    public void FixatePath() => fixedPath = Path;
    public void LoosenPath() => fixedPath = "";

    public virtual void SetFromJObject(JObject jObject) {
      //Value = jObject[nameof(IJsonItem.Value)]?.ToObject<object>();
      //Range = jObject[nameof(IJsonItem.Range)]?.ToObject<object[]>();
    }
    #endregion

    #region Helper
    /*
     * TODO protected abstract bool Validate();
     */
    protected abstract bool Validate();
    /*
    protected virtual bool IsInRange() {
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

    protected bool IsInRangeList(object value) {
      foreach (var x in Range)
        if (x.Equals(value)) return true;
      return false;
    }

    protected bool IsInNumericRange(object value) =>
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

    protected bool IsInNumericRange<T>(object value) where T : IComparable {
      object min = Range.First(), max = Range.Last();
      return
        value != null && min != null && max != null && value is T && min is T && max is T &&
        (((T)min).CompareTo(value) == -1 || ((T)min).CompareTo(value) == 0) &&
        (((T)max).CompareTo(value) == 1 || ((T)max).CompareTo(value) == 0);
    }
    */
    #endregion
  }
}