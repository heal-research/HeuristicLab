//using System;
//using HeuristicLab.Core;

//namespace HeuristicLab.JsonInterface {
//  public abstract class BaseConverter : IJsonItemConverter
//  {
//    public abstract int Priority { get; }
//    public abstract bool CanConvertType(Type t);
//    public abstract void Inject(IItem item, IJsonItem data, IJsonItemConverter root);
//    public abstract IJsonItem Extract(IItem value, IJsonItemConverter root);

//    #region Helper
//    protected IItem Instantiate(Type type, params object[] args) =>
//      (IItem)Activator.CreateInstance(type,args);
//    #endregion
//  }
//}
