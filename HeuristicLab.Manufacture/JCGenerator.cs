using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.TravelingSalesman;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace HeuristicLab.Manufacture {

  public class CustomWriter : JsonTextWriter {
    private const int ArrayFlatterLvl = 1;
    private Stack<Formatting> formattings = new Stack<Formatting>();
    private int lvl = 0;
    private bool isRangeArray = false;
    public override void WriteStartArray() {
      base.WriteStartArray();
      /*
      if(lvl > ArrayFlatterLvl) {
        formattings.Push(base.Formatting);
        base.Formatting = Formatting.None;
      }
      lvl++;
      */
      if(isRangeArray) base.Formatting = Formatting.None;
    }

    public override void WriteEndArray() {
      base.WriteEndArray();
      /*
      lvl--;
      if (lvl > ArrayFlatterLvl)
        base.Formatting = formattings.Pop();
      */
      if(isRangeArray) {
        base.Formatting = Formatting.Indented;
        isRangeArray = false;
      }
    }

    public override void WritePropertyName(string name) {
      base.WritePropertyName(name);
      isRangeArray = name == "Range";
    }

    public CustomWriter(TextWriter writer) : base(writer) { }

    public static string Serialize(JToken token) {
      JsonSerializer serializer = new JsonSerializer();
      StringWriter sw = new StringWriter();
      CustomWriter writer = new CustomWriter(sw);
      writer.Formatting = Formatting.Indented;
      serializer.Serialize(writer, token);
      return sw.ToString();
    }
  }

  public class JCGenerator {

    private JObject template = JObject.Parse(@"{
      'Metadata': {
        'Algorithm':'',
        'Problem':''
      },
      'Objects': [],
      'Types': {}
    }");

    private Dictionary<string, string> TypeList = new Dictionary<string, string>();
    
    public string GenerateTemplate(IAlgorithm algorithm, IProblem problem, params string[] freeParameters) {
      algorithm.Problem = problem;
      Component algorithmData = Transformer.Extract(algorithm);
      Component problemData = Transformer.Extract(problem);
      IList<Component> items = algorithmData.ParameterizedItems;
      foreach (var pItem in problemData.ParameterizedItems) items.Add(pItem);
      JArray jsonItems = new JArray();
      
      foreach(var item in items.Distinct()) {
        JObject token = JObject.FromObject(item, Settings());
        //token["StaticParameters"] = token["Parameters"];
        //token["FreeParameters"] = token["Parameters"];

        foreach (JObject x in token["Parameters"]) {
          x.Property("Parameters")?.Remove();
          x.Property("Type")?.Remove();
        }
        TypeList.Add(item.Name, item.Type);
        RefactorParameters(token);
        /*
        foreach (JObject x in token["StaticParameters"]) {
          x.Property("Parameters")?.Remove();
          x.Property("Type")?.Remove();
        }
        foreach (JObject x in token["FreeParameters"]) {
          x.Property("Parameters")?.Remove();
          x.Property("Type")?.Remove();
        }
        */
        //token.Cast<JObject>().Property("Parameters")?.Remove();
        token.Property("Default")?.Remove();
        token.Property("Type")?.Remove();

        //RefactorFreeParameters(token, freeParameters);
        //RefactorStaticParameters(token);
        if (/*token["StaticParameters"].HasValues || token["FreeParameters"].HasValues ||*/ token["Parameters"].HasValues || (token["Operators"]?.HasValues ?? false))
          jsonItems.Add(token);

        if(!token["Parameters"].HasValues) {
          token.Property("Parameters")?.Remove();
        }
      }

      template["Metadata"]["Algorithm"] = algorithm.Name;
      template["Metadata"]["Problem"] = problem.Name;
      template["Objects"] = jsonItems;
      template["Types"] = JObject.FromObject(TypeList);

      return CustomWriter.Serialize(template);
    }

    #region Helper
    private void RefactorFreeParameters(JToken token, string[] freeParameters) {

      //token["FreeParameters"] = token["StaticParameters"];

      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<Component>();

        /*bool isSelected = false;
        string name = x["Name"].ToObject<string>();
        foreach (var selected in freeParameters)
          isSelected = (name == selected || isSelected);
        */
        if (/*!isSelected ||*/ p.Default == null || (p.Default != null && p.Default.GetType() == typeof(string) && p.Range == null)) {
          objToRemove.Add(x);
        } else {
          x.Property("Path")?.Remove();
          x.Property("Type")?.Remove();
        }
      }, token["FreeParameters"]);
      foreach (var x in objToRemove) x.Remove();

    }

    private void RefactorStaticParameters(JToken token) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<Component>();
        x.Property("Range")?.Remove();
        if (p.Default == null) objToRemove.Add(x);
      }, token["StaticParameters"]);
      foreach (var x in objToRemove) x.Remove();
    }

    private void RefactorParameters(JToken token) {
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<Component>();
        if (p.Default == null) objToRemove.Add(x);
      }, token["Parameters"]);
      foreach (var x in objToRemove) x.Remove();
    }

    private JsonSerializer Settings() => new JsonSerializer() {
      TypeNameHandling = TypeNameHandling.None,
      NullValueHandling = NullValueHandling.Ignore
    };


    private void TransformNodes(Action<JObject> action, params JToken[] tokens) {
      foreach(JObject obj in tokens.SelectMany(x => x.Children<JObject>())) {
        action(obj);
      }
    }
    #endregion
  }
}
