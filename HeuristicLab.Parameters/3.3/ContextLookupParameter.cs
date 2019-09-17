using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Parameters {
  [Item("ContextLookupParameter", "A parameter that looks up contexts by type.")]
  [StorableType("4ac189c8-6cf3-48fd-bf79-392d35a872db")]
  public class ContextLookupParameter<T> : ContextParameter, IContextLookupParameter<T>
      where T : class, IParameterizedItem {

    public new T ActualValue {
      get { return (T)base.ActualValue; }
    }

    [StorableConstructor]
    protected ContextLookupParameter(StorableConstructorFlag _) : base(_) { }
    protected ContextLookupParameter(ContextLookupParameter<T> original, Cloner cloner) : base(original, cloner) { }
    public ContextLookupParameter() : base("ContextLookup." + typeof(T).Name, string.Empty, typeof(T)) {
      Hidden = true;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ContextLookupParameter<T>(this, cloner);
    }

    protected override IItem GetActualValueFromContext() {
      IItem item = null;
      var context = ExecutionContext;
      while (context != null) {
        if (context.Item != null && typeof(T).IsAssignableFrom(context.Item.GetType())) {
          item = context.Item;
          break;
        }
        context = context.Parent;
      }
      return item;
    }

    protected override void SetActualValue(IItem value) {
      throw new NotSupportedException("The context lookup parameter may not be used to set an item.");
    }
  }
}
