using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {

  /// <summary>
  /// Main data class for json interface.
  /// </summary>
  public class JsonItem /*: IEnumerable<JsonItem>*/ {
    private readonly IDictionary<string, JsonItem> childs = new Dictionary<string, JsonItem>();
    private readonly IDictionary<string, object> properties = new Dictionary<string, object>();

    [JsonIgnore]
    public string Id { get; private set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Path {
      get {
        JsonItem tmp = Parent;
        StringBuilder builder = new StringBuilder(this.Id);
        while (tmp != null) {
          builder.Insert(0, tmp.Id + ".");
          tmp = tmp.Parent;
        }
        return builder.ToString();
      }
    }

    [JsonIgnore]
    public IEnumerable<KeyValuePair<string, JsonItem>> Childs => childs;

    [JsonIgnore]
    public IEnumerable<KeyValuePair<string, object>> Properties => properties;

    [JsonIgnore]
    public JsonItem Parent { get; set; }


    #region Constructors
    public JsonItem(IJsonConvertable convertable, JsonItemConverter converter) : this("", convertable, converter) { }
    public JsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) {
      Id = id;
      converter.AddToCache(convertable, this);
    }
    #endregion

    #region Public Methods
    public void AddChild(string name, JsonItem jsonItem) {
      jsonItem.Parent = this;
      jsonItem.Id = name;
      childs.Add(name, jsonItem);
    }
    public void MergeChilds(JsonItem jsonItem) {
      foreach(var kvp in jsonItem.Childs)
        AddChild(kvp.Key, kvp.Value);
    }
    public JsonItem GetChild(string name) => childs[name];

    public void AddProperty<T>(string name, T value) => properties.Add(name, value);
    public void AddProperty(KeyValuePair<string, object> property) => properties.Add(property);
    public void AddProperty(string name, JsonItem value) {
      value.Parent = this;
      this.AddProperty<JsonItem>(name, value); // Forbid to add JsonItems as props?
    }
    public void RemoveProperty(string name) => properties.Remove(name);
    public void MergeProperties(JsonItem value) {
      foreach (var property in value.Properties)
        properties.Add(property);
    }
    public T GetProperty<T>(string name) => (T)properties[name];

    /// <summary>
    /// Method to generate a Newtonsoft JObject, which describes the JsonItem.
    /// </summary>
    /// <returns>Newtonsoft JObject</returns>
    protected internal virtual JObject ToJObject() {
      var obj = JObject.FromObject(this, new JsonSerializer() {
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      });
      foreach(var prop in Properties) {
        if (prop.Value is JsonItem item)
          obj.Add(prop.Key, item.ToJObject());
        else
          obj.Add(prop.Key, JToken.FromObject(prop.Value));
      }
      return obj;
    }
    
    /// <summary>
    /// To set all necessary JsonItem properties with an given Newtonsoft JObject.
    /// </summary>
    /// <param name="jObject">Newtonsoft JObject</param>
    protected internal virtual void FromJObject(JObject jObject) {
      foreach(var prop in Properties.ToArray())
        properties[prop.Key] = jObject[prop.Key]?.ToObject(prop.Value.GetType()) ?? properties[prop.Key];
    }
    #endregion

    #region IEnumerable Support
    public virtual IEnumerable<JsonItem> Iterate() {
      yield return this;
      foreach (var kvp in Childs)
        foreach (var c in kvp.Value.Iterate())
          yield return c;
    }
    #endregion
  }
}