using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class BaseConverter : IJsonItemConverter
  {
    public abstract int Priority { get; }

    public abstract bool CanConvertType(Type t);

    public abstract Type ConvertableType { get; }

    public abstract void Inject(IItem item, IJsonItem data, IJsonItemConverter root);
    public abstract IJsonItem Extract(IItem value, IJsonItemConverter root);

    #region Helper

    protected IItem Instantiate(Type type, params object[] args) =>
      (IItem)Activator.CreateInstance(type,args);
    #endregion
  }
}
