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

    public virtual JObject GenerateJObject() =>
      JObject.FromObject(this, new JsonSerializer() {
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      });

    public virtual void SetJObject(JObject jObject) { }
    #endregion

    #region Abstract Methods
    protected abstract bool Validate();
    #endregion

    #region IEnumerable Support
    public virtual IEnumerator<IJsonItem> GetEnumerator() {
      yield return this;
      
      if (Children != null) {
        foreach (var x in Children) {
          foreach (var c in x) {
            yield return c;
          }
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
  }
}