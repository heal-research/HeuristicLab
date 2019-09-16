using System;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Parameters {
  [Item("ContextLookupParameter", "A parameter that looks up contexts by type.")]
  [StorableType("4ac189c8-6cf3-48fd-bf79-392d35a872db")]
  public class ContextLookupParameter<T> : Parameter, IContextLookupParameter<T>, IStatefulItem
      where T : class, IParameterizedItem {

    public new T ActualValue {
      get { return (T)base.ActualValue; }
    }

    private Lazy<ThreadLocal<IItem>> cachedActualValues;
    protected IItem CachedActualValue {
      get { return cachedActualValues.Value.Value; }
      set { cachedActualValues.Value.Value = value; }
    }

    private Lazy<ThreadLocal<IExecutionContext>> executionContexts;
    public IExecutionContext ExecutionContext {
      get { return executionContexts.Value.Value; }
      set {
        if (value != executionContexts.Value.Value) {
          executionContexts.Value.Value = value;
          cachedActualValues.Value.Value = null;
        }
      }
    }

    [StorableConstructor]
    protected ContextLookupParameter(StorableConstructorFlag _) : base(_) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected ContextLookupParameter(ContextLookupParameter<T> original, Cloner cloner)
      : base(original, cloner) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    public ContextLookupParameter() : this("Anonymous") { }
    public ContextLookupParameter(string name, string description = null)
      : base(name, description, typeof(T)) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ContextLookupParameter<T>(this, cloner);
    }

    protected override IItem GetActualValue() {
      if (CachedActualValue != null) return CachedActualValue;

      IItem item = null;
      var context = ExecutionContext;
      while (context != null) {
        if (context.Item != null && typeof(T).IsAssignableFrom(context.Item.GetType())) {
          item = context.Item;
          break;
        }
        context = context.Parent;
      }
      CachedActualValue = item;
      return item;
    }

    protected override void SetActualValue(IItem value) {
      throw new NotSupportedException("The context lookup parameter may not be used to set an item.");
    }

    public virtual void InitializeState() {
    }
    public virtual void ClearState() {
      if (cachedActualValues.IsValueCreated) {
        cachedActualValues.Value.Dispose();
        cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      }
      if (executionContexts.IsValueCreated) {
        executionContexts.Value.Dispose();
        executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      }
    }
  }
}
