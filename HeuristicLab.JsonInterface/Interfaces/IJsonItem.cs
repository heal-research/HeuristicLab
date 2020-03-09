using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public interface IJsonItem {
    bool Active { get; set; }
    string Name { get; set; }

    string Description { get; set; }

    string Path {
      get;
    }

    [JsonIgnore]
    IList<IJsonItem> Children { get; } //TODO: IEnumerable

    [JsonIgnore]
    IJsonItem Parent { get; set; }

    object Value { get; set; }

    IEnumerable<object> Range { get; set; }
    
    IJsonItemValidator GetValidator();

    void AddChildren(params IJsonItem[] childs);

    void AddChildren(IEnumerable<IJsonItem> childs);

    /// <summary>
    /// This method fixates the path. 
    /// After calling, the path cannot be changed by changing the name or parent.
    /// </summary>
    void FixatePath();

    /// <summary>
    /// This method looses the path again after a call of FixatePath.
    /// After calling, the path is calculated by the position in item tree again.
    /// </summary>
    void LoosenPath();

    void SetFromJObject(JObject jObject);
  }
}
