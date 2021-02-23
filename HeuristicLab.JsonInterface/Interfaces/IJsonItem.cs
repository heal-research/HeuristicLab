using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  [JsonObject]
  public interface IJsonItem : IEnumerable<IJsonItem> {
    /// <summary>
    /// Only active items are included in templates.
    /// </summary>
    bool Active { get; set; }

    /// <summary>
    /// The name of the JsonItem. Can be changed freely after fixating the path. 
    /// If the path is not fix, changing the name will have affect of the path.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// A description for the JsonItem.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// The JsonItem path in the related object graph.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// IEnumerable of all sub JsonItems.
    /// </summary>
    [JsonIgnore]
    IEnumerable<IJsonItem> Children { get; }

    /// <summary>
    /// If the JsonItem is a children of an other JsonItem, the parent will be this other JsonItem.
    /// </summary>
    [JsonIgnore]
    IJsonItem Parent { get; set; }

    /// <summary>
    /// Returns a validator with integrated caching to validate the JsonItem and all children.
    /// </summary>
    /// <returns>JsonItemValidator</returns>    
    IJsonItemValidator GetValidator();

    /// <summary>
    /// Add sub JsonItems.
    /// </summary>
    /// <param name="childs"></param>
    void AddChildren(params IJsonItem[] childs);
    void AddChildren(IEnumerable<IJsonItem> childs);

    /// <summary>
    /// Method to generate a Newtonsoft JObject, which describes the JsonItem.
    /// </summary>
    /// <returns>Newtonsoft JObject</returns>
    JObject GenerateJObject();

    /// <summary>
    /// To set all necessary JsonItem properties with an given Newtonsoft JObject.
    /// </summary>
    /// <param name="jObject">Newtonsoft JObject</param>
    void SetJObject(JObject jObject);
  }
}
