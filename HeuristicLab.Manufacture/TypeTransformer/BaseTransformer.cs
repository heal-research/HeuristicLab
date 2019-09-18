using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using Newtonsoft.Json.Linq;

namespace ParameterTest {
  public abstract class BaseTransformer : ITypeTransformer
  {
    public abstract void SetValue(IItem item, ParameterData data);
    public abstract IItem FromData(ParameterData obj, Type targetType);
    public virtual ParameterData ToData(IItem value) => 
      new ParameterData() { Name = value.ItemName };

    protected ValueType CastValue<ValueType>(object obj) {
      if (obj is JToken)
        return ((JToken)obj).ToObject<ValueType>();
      else
        return (ValueType)obj;
    }

    protected IItem Instantiate(Type type, params object[] args) =>
      (IItem)Activator.CreateInstance(type,args);

    protected IItem Instantiate<T>(params object[] args) => Instantiate(typeof(T), args);
  }
}
