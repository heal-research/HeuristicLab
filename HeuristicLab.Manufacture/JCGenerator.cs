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

namespace ParameterTest {
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

    public string GenerateTemplate(IAlgorithm algorithm, IProblem problem) {
      algorithm.Problem = problem;
      foreach(var x in BJCO(algorithm)) {
        Console.WriteLine(x.ToString());
      }
      /*
      JCObject algorithmData = BuildJCObject(algorithm);
      JCObject problemData = BuildJCObject(problem);

      ProcessStaticParameters(algorithmData, problemData);
      ProcessFreeParameters(algorithmData, problemData);
      ProcessTypeList();

      return template.ToString(Formatting.Indented);
      */
      return "";
    }

    #region Helper
    private JCObject BuildJCObject(IItem item) {
      JCObject obj = new JCObject();
      obj.Name = item.ItemName;
      obj.Parameters = BuildJCParameter(item).Parameters;
      return obj;
    }

    private IList<JCObject> BJCO(IParameterizedItem item) {
      List<JCObject> list = new List<JCObject>();
      JCObject obj = new JCObject();
      obj.Name = item.ItemName;
      obj.StaticParameters = new List<ParameterData>();
      obj.FreeParameters = new List<ParameterData>();

      list.Add(obj);

      foreach (var param in item.Parameters) {
        if(!param.Hidden) {
          ParameterData newParam = new ParameterData();
          newParam.Name = param.Name;
          if (!Util.IsTypeOf(param, typeof(ILookupParameter<>))) {
            if (param.ActualValue is IntValue) {
              SetParameterValueStats(newParam, ((IntValue)param.ActualValue).Value, 0, int.MaxValue);
            } else if (param.ActualValue is BoolValue) {
              SetParameterValueStats(newParam, ((BoolValue)param.ActualValue).Value, false, true);
            } else if (param.ActualValue is PercentValue) {
              SetParameterValueStats(newParam, ((PercentValue)param.ActualValue).Value, 0d, 1d);
            } else if (param.ActualValue is DoubleValue) {
              SetParameterValueStats(newParam, ((DoubleValue)param.ActualValue).Value, 0d, double.MaxValue);
            } else if (Util.IsTypeOf(param.ActualValue, typeof(EnumValue<>))) {
              var enumType = ((dynamic)param.ActualValue).Value.GetType();
              SetParameterValueStats(newParam,
                Enum.GetName(enumType, ((dynamic)param.ActualValue).Value),
                Enum.GetNames(enumType));
            } else if (param.ActualValue is StringValue) {
              SetParameterValueStats(newParam, ((StringValue)param.ActualValue).Value);
            } else if (Util.IsTypeOf(param.ActualValue, typeof(ValueTypeMatrix<>))) {
              SetParameterValueStats(newParam, ((dynamic)param.ActualValue).CloneAsMatrix());
            } else if (Util.IsTypeOf(param.ActualValue, typeof(ValueTypeArray<>))) {
              SetParameterValueStats(newParam, ((dynamic)param.ActualValue).CloneAsArray());
            } else {
              AddType(param.ActualValue);
              //newParam.Path = newParam.Path.Substring(0, newParam.Path.Length - 6);
              SetParameterValueStats(newParam, param.ActualValue?.GetType().Name, ExtractPossibleValues(item, param.DataType));
            }
          }


          obj.StaticParameters.Add(newParam);

          if (param is IParameterizedItem) {
            list.AddRange(BJCO((IParameterizedItem)param));
          }
          if (Util.IsTypeOf(param, typeof(IConstrainedValueParameter<>))) {
            foreach (var validValue in ((dynamic)param).ValidValues) {
              if (newParam.Range == null) newParam.Range = new List<object>();
              object v = (object)validValue;
              try {
                if (!newParam.Range.Contains(v))
                  newParam.Range.Add(v);
              } catch { }
              if (validValue is IParameterizedItem) {
                list.AddRange(BJCO((IParameterizedItem)validValue));
              }
            }
          }
        }
      }
      return list;
    }



    private IEnumerable<IParameterizedItem> GetIParameterItems(IParameterizedItem item) {
      List<IParameterizedItem> items = new List<IParameterizedItem>();
      items.Add(item);
      foreach(var param in item.Parameters) {
        if(param is IParameterizedItem) {
          items.AddRange(GetIParameterItems((IParameterizedItem)param));
        }
        if(Util.IsTypeOf(param, typeof(IConstrainedValueParameter<>))){
          foreach(var validValue in ((dynamic)param).ValidValues) {
            if(validValue is IParameterizedItem) {
              items.AddRange(GetIParameterItems((IParameterizedItem)validValue));
            }
          }
        }
      }
      return items;
    }

    private JsonSerializer Settings() => new JsonSerializer() {
      TypeNameHandling = TypeNameHandling.None,
      NullValueHandling = NullValueHandling.Ignore
    };

    private void ProcessFreeParameters(JCObject algorithmData, JCObject problemData) {
      JObject freeParams = new JObject();
      JToken alg = JToken.FromObject(algorithmData.Parameters, Settings());
      JToken prob = JToken.FromObject(problemData.Parameters, Settings());
      
      IList<JObject> objToRemove = new List<JObject>();
      TransformNodes(x => {
        var p = x.ToObject<ParameterData>();
        if (p.Default == null || (p.Default != null && p.Default.GetType() == typeof(string) && p.Range == null)) {
          objToRemove.Add(x);
        } else {
          x.Property("Path")?.Remove();
          x.Property("Type")?.Remove();
        }
      },alg, prob);
      foreach (var x in objToRemove) x.Remove();

      freeParams.Add(algorithmData.Name, alg);
      freeParams.Add(problemData.Name, prob);
      template["FreeParameters"] = freeParams;
    }

    private void ProcessStaticParameters(JCObject algorithmData, JCObject problemData) {
      template["StaticParameters"]["Algorithm"] = JObject.FromObject(algorithmData, Settings());
      template["StaticParameters"]["Problem"] = JObject.FromObject(problemData, Settings());
      TransformNodes(x => x.Property("Range")?.Remove(),
        template["StaticParameters"]["Algorithm"]["Parameters"],
        template["StaticParameters"]["Problem"]["Parameters"]);
    }

    private void ProcessTypeList() {
      template["StaticParameters"]["TypeList"] = JObject.FromObject(TypeList);
    }

    private void TransformNodes(Action<JObject> action, params JToken[] tokens) {
      foreach(JObject obj in tokens.SelectMany(x => x.Children<JObject>())) {
        action(obj);
      }
    }
    #endregion

    
    private ParameterData BuildJCParameter(IItem item, string startPath = "") {
      ParameterData obj = new ParameterData();
      obj.Name = item.ItemName;
      obj.Path = startPath;

      AddType(item, obj.Name);

      try {
        ExtractParameterInfo(item, obj);
      } catch { }
      ExtractOperatorInfo(item, obj);
      return obj;
    }

    private bool ProcessValueTypeParameters(ParameterData jCParameter, IParameter parameter) {
      if (parameter.ActualValue is IntValue)
        SetParameterValueStats(jCParameter, 
          ((IntValue)parameter.ActualValue).Value, 
          0, int.MaxValue);
      else if (parameter.ActualValue is BoolValue)
        SetParameterValueStats(jCParameter, 
          ((BoolValue)parameter.ActualValue).Value, 
          false, true);
      else if (parameter.ActualValue is PercentValue)
        SetParameterValueStats(jCParameter, 
          ((PercentValue)parameter.ActualValue).Value, 
          0d, 1d);
      else if (parameter.ActualValue is DoubleValue)
        SetParameterValueStats(jCParameter, 
          ((DoubleValue)parameter.ActualValue).Value, 
          0d, double.MaxValue);
      else return false;
      return true;
    }

    private bool ProcessEnumTypeParameters(ParameterData jCParameter, IParameter parameter) {
      if (Util.IsTypeOf(parameter.ActualValue, typeof(EnumValue<>))) {
        var enumType = ((dynamic)parameter.ActualValue).Value.GetType();
        SetParameterValueStats(jCParameter,
          Enum.GetName(enumType, ((dynamic)parameter.ActualValue).Value),
          Enum.GetNames(enumType));
      } else return false;
      return true;
    }

    private bool ProcessMatrixTypeParameters(ParameterData jCParameter, IParameter parameter) {
      if (Util.IsTypeOf(parameter.ActualValue, typeof(ValueTypeMatrix<>))) {
        SetParameterValueStats(jCParameter, ((dynamic)parameter.ActualValue).CloneAsMatrix());
      } else return false;
      return true;
    }

    private bool ProcessArrayTypeParameters(ParameterData jCParameter, IParameter parameter) {
      if (Util.IsTypeOf(parameter.ActualValue, typeof(ValueTypeArray<>))) {
        SetParameterValueStats(jCParameter, ((dynamic)parameter.ActualValue).CloneAsArray());
      } else return false;
      return true;
    }

    private void SetParameterValueStats(ParameterData parameter, object value, params object[] range) {
      parameter.Default = value;
      parameter.Range = range.Length == 0 ? null : range;
    }

    
    private void ExtractParameterInfo(IItem item, ParameterData obj) {
      if (item is IParameterizedItem) {
        foreach (var param in ((IParameterizedItem)item).Parameters) {
          if (!param.Hidden) {

            ParameterData newParam = BuildJCParameter(param);
            newParam.Name = param.Name;
            var tmpType = param.GetType();
            newParam.Path = (obj.Path != "" ? obj.Path + "." : "") + param.Name + ".Value.Value";

            if (obj.Parameters == null)
              obj.Parameters = new List<ParameterData>();
            obj.Parameters.Add(newParam);


            if (param.ActualValue is IntValue) {
              SetParameterValueStats(newParam, ((IntValue)param.ActualValue).Value, 0, int.MaxValue);
            } else if (param.ActualValue is BoolValue) {
              SetParameterValueStats(newParam, ((BoolValue)param.ActualValue).Value, false, true);
            } else if (param.ActualValue is PercentValue) {
              SetParameterValueStats(newParam, ((PercentValue)param.ActualValue).Value, 0d, 1d);
            } else if (param.ActualValue is DoubleValue) {
              SetParameterValueStats(newParam, ((DoubleValue)param.ActualValue).Value, 0d, double.MaxValue);
            } else if (Util.IsTypeOf(param.ActualValue, typeof(EnumValue<>))) {
              var enumType = ((dynamic)param.ActualValue).Value.GetType();
              SetParameterValueStats(newParam, 
                Enum.GetName(enumType, ((dynamic)param.ActualValue).Value), 
                Enum.GetNames(enumType));
            } else if (param.ActualValue is StringValue) {
              SetParameterValueStats(newParam, ((StringValue)param.ActualValue).Value);
            } else if (Util.IsTypeOf(param.ActualValue, typeof(ValueTypeMatrix<>))) {
              SetParameterValueStats(newParam, ((dynamic)param.ActualValue).CloneAsMatrix());
            } else if (Util.IsTypeOf(param.ActualValue, typeof(ValueTypeArray<>))) {
              SetParameterValueStats(newParam, ((dynamic)param.ActualValue).CloneAsArray());
            } else {
              AddType(param.ActualValue);
              newParam.Path = newParam.Path.Substring(0, newParam.Path.Length - 6);
              SetParameterValueStats(newParam, param.ActualValue?.GetType().Name, ExtractPossibleValues(item, param.DataType));
            }
          }
        }
      }
    }

    private object[] ExtractPossibleValues(IItem item, Type paramType) {
      IList<object> list = new List<object>();
      if (item is Algorithm) {
        IEnumerable<IItem> items = ((Algorithm)item).Problem.Operators.Where(x => paramType.IsAssignableFrom(x.GetType()));
        foreach (var op in items) {
          string name = op.GetType().Name;
          list.Add(name);
          AddType(op, name);
        }
      }
      return list.ToArray();
    }

    private void AddType(object obj, string key = null) {
      if (obj != null) {
        if (key == null) key = obj.GetType().Name;
        try {
          if (!TypeList.ContainsKey(key))
            TypeList.Add(key, obj.GetType().AssemblyQualifiedName);
        } catch { }
      }
    }

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
    }
  }
}
