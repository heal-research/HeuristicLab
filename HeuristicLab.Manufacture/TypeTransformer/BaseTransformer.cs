using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.Manufacture {
  public abstract class BaseTransformer : ITypeTransformer
  {
    public ParameterData Extract(IItem value) {
      ParameterData data = ExtractData(value);
      data.Name = String.IsNullOrEmpty(data.Name) ? value.ItemName : data.Name;
      return data;
    }

    public void Inject(IItem item, ParameterData data) => InjectData(item, data);

    public abstract void InjectData(IItem item, ParameterData data);
    public abstract ParameterData ExtractData(IItem value);

    #region Helper
    protected ValueType CastValue<ValueType>(object obj) {
      if (obj is JToken)
        return (obj.Cast<JToken>()).ToObject<ValueType>();
      else if (obj is IConvertible)
        return Convert.ChangeType(obj, typeof(ValueType)).Cast<ValueType>();
      else return (ValueType)obj;
    }

    protected IItem Instantiate(Type type, params object[] args) =>
      (IItem)Activator.CreateInstance(type,args);

    protected IItem Instantiate<T>(params object[] args) => Instantiate(typeof(T), args);
    #endregion
  }
}
