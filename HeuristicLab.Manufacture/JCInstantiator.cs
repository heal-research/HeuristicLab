using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.Manufacture {

  
  public class JCInstantiator {
    //private Dictionary<string, JCProperty> Properties = new Dictionary<string, JCProperty>();
    private Dictionary<string, string> TypeList = new Dictionary<string, string>();

    #region Constants
    private const string SParametersID = "StaticParameters";
    private const string FParametersID = "FreeParameters";
    private const string TListID = "TypeList";
    private const string AlgorithmID = "Algorithm";
    private const string ProblemID = "Problem";
    #endregion

    public IAlgorithm Instantiate(string configFile) {
      JArray config = JArray.Parse(File.ReadAllText(configFile));
      JCObject obj = config[0].ToObject<JCObject>();
      IAlgorithm algorithm = CreateObject<IAlgorithm>(obj);

      foreach(var sp in obj.StaticParameters) {
        if(algorithm.Parameters.TryGetValue(sp.Name, out IParameter param)) {
          Transformer.Inject(param, sp);
        }
      }

      foreach (var sp in obj.FreeParameters) {
        if (algorithm.Parameters.TryGetValue(sp.Name, out IParameter param)) {
          if(IsInRangeList(sp.Range, sp.Default) || 
            IsInNumericRange<long>(sp.Default, sp.Range[0], sp.Range[1]) || 
            IsInNumericRange<double>(sp.Default, sp.Range[0], sp.Range[1]))
          Transformer.Inject(param, sp);
        }
      }


      return algorithm;


      /*
      TypeList = config[SParametersID][TListID].ToObject<Dictionary<string, string>>();

      JCObject algorithmData = config[SParametersID][AlgorithmID].ToObject<JCObject>();
      JCObject problemData = config[SParametersID][ProblemID].ToObject<JCObject>();

      IAlgorithm algorithm = CreateTypeListObject<IAlgorithm>(algorithmData.Name);
      algorithm.Problem = CreateTypeListObject<IProblem>(problemData.Name);

      ProcessParameters(algorithmData, algorithm);
      ProcessParameters(problemData, algorithm.Problem);

      ParameterData[] freeAlgorithmParameters = config[FParametersID][algorithmData.Name].ToObject<ParameterData[]>();
      ParameterData[] freeProblemParameters = config[FParametersID][problemData.Name].ToObject<ParameterData[]>();
      UseFreeParams(algorithmData.Name, freeAlgorithmParameters, algorithm);
      UseFreeParams(problemData.Name, freeProblemParameters, algorithm.Problem);
      return algorithm;
      */
    }

    private T CreateObject<T>(JCObject obj) {
      Type type = Type.GetType(obj.Type);
      return (T)Activator.CreateInstance(type);
    }

    private T CreateTypeListObject<T>(string key) {
      if (TypeList.TryGetValue(key, out string value)) {
        Type type = Type.GetType(value);
        return (T)Activator.CreateInstance(type);
      } else throw new TypeLoadException($"The type '{key}' does not exists in TypeList.");
    }

    private void ProcessParameters(JCObject obj, IParameterizedItem instance) {
      foreach (var param in obj.StaticParameters)
        if (param.Default != null)
          SetParameterValue(FindPropertyByKey(instance, param.Name), param.Default);
    }

    private void SetParameterValue (IParameter parameter, object value) {
      if (Util.IsTypeOf(parameter, typeof(IFixedValueParameter<>))) return;
      if (Util.IsTypeOf(parameter, typeof(IConstrainedValueParameter<>)))
        SetCVParameter(parameter, value);
      else if (Util.IsTypeOf(parameter.ActualValue, typeof(ValueTypeMatrix<>)))
        SetVTMatrix(parameter, value);
      else if (Util.IsTypeOf(parameter.ActualValue, typeof(ValueTypeArray<>)))
        SetVTArray(parameter, value);
      else if (Util.IsTypeOf(parameter.ActualValue, typeof(EnumValue<>)))
        SetEnum(parameter, value);
      else if (!(value is string))
        SetValue(parameter, value);
      else
        SetNewInstance(parameter, value);
    }
    #region Helper for SetParameterValue
    private void SetCVParameter(IParameter parameter, object value) {
      foreach (var x in ((dynamic)parameter).ValidValues)
        if (x.GetType().Name == (string)value)
          parameter.ActualValue = x;
    }

    private void SetVTMatrix(IParameter parameter, object value) {
      var matrixType = ((dynamic)parameter.ActualValue).CloneAsMatrix().GetType();
      var data = ((JToken)value).ToObject(matrixType);
      parameter.ActualValue = (IItem)Activator.CreateInstance(parameter.ActualValue.GetType(), new object[] { data });
    }

    private void SetVTArray(IParameter parameter, object value) {
      var arrayType = ((dynamic)parameter.ActualValue).CloneAsArray().GetType();
      var data = ((JToken)value).ToObject(arrayType);
      parameter.ActualValue = (IItem)Activator.CreateInstance(parameter.ActualValue.GetType(), new object[] { data });
    }

    private void SetEnum(IParameter parameter, object value) {
      var enumType = ((dynamic)parameter.ActualValue).Value.GetType();
      var data = Enum.Parse(enumType, (string)value);
      ((dynamic)parameter.ActualValue).Value = data;
    }

    private void SetValue(IParameter parameter, object value) {
      var x = (dynamic)parameter.ActualValue;
      if (value is IConvertible)
        x.Value = Convert.ChangeType(value, x.Value.GetType());
      else
        x.Value = value;
    }

    private void SetNewInstance(IParameter parameter, object value) => 
      parameter.ActualValue = (IItem)Activator.CreateInstance(Type.GetType(TypeList[(string)value]));
    #endregion

    private IParameter FindPropertyByKey(IParameterizedItem item, string key) {
      if (item.Parameters.TryGetValue(key, out IParameter param))
        return param;
      else throw new KeyNotFoundException();
    }


    /*
    JCProperty property = FindPropertyByPath(instance, param.Path);
    property.Value = param.Default;
    Properties.Add(obj.Name + "." + param.Name, property); 
    
    private JCProperty FindPropertyByPath(object start, string path) {
      string[] parts = path.Split('.');
      object instance = start;
      for (int i = 0; i < parts.Length - 1; ++i) {
        if (parts[i].StartsWith("Parameters['")) {
          IParameterizedItem pItem = (IParameterizedItem)instance;
          string tmp = parts[i].Replace("Parameters['", "").Replace("']", "");
          if (pItem.Parameters.ContainsKey(tmp))
            instance = pItem.Parameters[tmp];
        } else {
          PropertyInfo propInfo = instance.GetType().GetProperty(parts[i]);
          instance = propInfo.GetValue(instance);
        }
      }
      return new JCProperty(instance.GetType().GetProperty(parts[parts.Length - 1]), instance, TypeList);
    }
    */
    private bool IsInRangeList(IEnumerable<object> list, object value) {
      foreach (var x in list)
        if (x.Equals(value)) return true;
      return false;
    }

    private void UseFreeParams(string root, ParameterData[] parameters, IParameterizedItem instance) {
      foreach (var param in parameters) {
        string key = $"{root}.{param.Name}";
        if((param.Range == null && param.Default != null) ||
          (param.Range != null && IsInRangeList(param.Range, param.Default)) ||
          IsInNumericRange<long>(param.Default, param.Range[0], param.Range[1]) ||
          IsInNumericRange<double>(param.Default, param.Range[0], param.Range[1]))
          SetParameterValue(FindPropertyByKey(instance, param.Name), param.Default);
        //Properties[key].Value = param.Default;
        else
          throw new ArgumentOutOfRangeException(key);
      }
    }

    private bool IsInNumericRange<T>(object value, object min, object max) where T : IComparable =>
      (value != null && min != null && max != null && value is T && min is T && max is T &&
        (((T)min).CompareTo(value) == -1 || ((T)min).CompareTo(value) == 0) && 
        (((T)max).CompareTo(value) ==  1 || ((T)max).CompareTo(value) == 0));
  }
}
