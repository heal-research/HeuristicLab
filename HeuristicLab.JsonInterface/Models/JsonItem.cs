using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {

  public readonly struct ValidationResult {

    public static ValidationResult Successful() => new ValidationResult(true, Enumerable.Empty<string>());
    public static ValidationResult Faulty(IEnumerable<string> errors) => new ValidationResult(false, errors);
    public static ValidationResult Faulty(string error) => new ValidationResult(false, error);

    public ValidationResult(bool success, IEnumerable<string> errors) {
      Success = success;
      Errors = errors;
    }

    public ValidationResult(bool success, string error) {
      Success = success;
      Errors = Enumerable.Repeat(error, 1);
    }

    public bool Success { get; }
    public IEnumerable<string> Errors { get; }
  }

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

      public ValidationResult Validate() {
        List<string> errors = new List<string>();
        bool success = true;
        foreach(var x in Root) {
          JsonItem item = x as JsonItem;
          if(item.Active) {
            var res = ((JsonItem)x).Validate();
            //if one success is false -> whole validation is false
            success = success && res.Success;
            errors.AddRange(res.Errors);
          }
        }
        return new ValidationResult(success, errors);
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
    public virtual IEnumerable<IJsonItem> Children { get; protected set; }

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
      if(Children is IList<IJsonItem> list) {
        foreach (var child in childs) {
          list.Add(child);
          child.Parent = this;
        }
      }
    }

    public IJsonItemValidator GetValidator() => new JsonItemValidator(this);

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
    protected abstract ValidationResult Validate();
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