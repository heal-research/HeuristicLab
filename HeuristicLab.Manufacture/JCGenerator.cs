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
    private Stack<Formatting> formattings = new Stack<Formatting>();
    private int lvl = 0;
    public override void WriteStartArray() {
      base.WriteStartArray();
      if(lvl > 1) {
        formattings.Push(base.Formatting);
        base.Formatting = Formatting.None;
      }
      lvl++;
    }

    public override void WriteEndArray() {
      base.WriteEndArray();
      lvl--;
      if (lvl > 1)
        base.Formatting = formattings.Pop();
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
      'Metadata': {},
      'FreeParameters': {},
      'StaticParameters': {
        'Algorithm':{},
        'Problem':{},
        'TypeList':{}
      }
    }");

    
    private Dictionary<string, string> TypeList = new Dictionary<string, string>();

    public string GenerateTemplate(IAlgorithm algorithm, IProblem problem, params string[] freeParameters) {
      algorithm.Problem = problem;
      IList<JCObject> items = BJCO(algorithm);
      JArray jsonItems = new JArray();
      
      foreach(var item in items) {
        JToken token = JObject.FromObject(item, Settings());

        RefactorFreeParameters(token, freeParameters);
        RefactorStaticParameters(token);
        if(token["StaticParameters"].HasValues || token["FreeParameters"].HasValues)
          jsonItems.Add(token);
      } 
      return CustomWriter.Serialize(jsonItems);
    }

    private void RefactorFreeParameters(JToken token, string[] freeParameters) {

      token["FreeParameters"] = token["StaticParameters"];

      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<ParameterData>();

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
        var p = x.ToObject<ParameterData>();
        x.Property("Range")?.Remove();
        if (p.Default == null) objToRemove.Add(x);
      }, token["StaticParameters"]);
      foreach (var x in objToRemove) x.Remove();
    }
      
    

    #region Helper
    private IList<JCObject> BJCO(IParameterizedItem item) {
      List<JCObject> list = new List<JCObject>();
      JCObject obj = new JCObject();
      obj.Name = item.ItemName;
      obj.Type = item.GetType().AssemblyQualifiedName;
      obj.StaticParameters = new List<ParameterData>();
      list.Add(obj);

      foreach (var param in item.Parameters) {
        if(!param.Hidden) {
          obj.StaticParameters.Add(Transformer.Extract(param));

          if (param is IParameterizedItem)
            list.AddRange(BJCO(param.Cast<IParameterizedItem>()));
          
          if (param.GetType().IsEqualTo(typeof(IConstrainedValueParameter<>)))
            foreach (var validValue in param.Cast<dynamic>().ValidValues)
              if (validValue is IParameterizedItem && 
                ((IParameterizedItem)validValue).Parameters.Any(p => !p.Hidden))
                list.AddRange(BJCO((IParameterizedItem)validValue));
        }
      }
      return list;
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

    /*
    private void ExtractOperatorInfo(IItem item, ParameterData obj) {
      if (item is IMultiOperator) {
        foreach (var op in ((IMultiOperator)item).Operators) {
          ParameterData newOperator = BuildJCParameter(op);
          if (obj.Operators == null)
            obj.Operators = new List<ParameterData>();
          obj.Operators.Add(newOperator);
          newOperator.Name = op.GetType().Name;

          newOperator.Path = obj.Path + ".Operators." + op.Name;
          MultiPermutationManipulator manipulator = new MultiPermutationManipulator();
        }
      }
    }*/
  }
}
