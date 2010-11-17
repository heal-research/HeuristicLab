
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Operators.Programmable {

  [Item("ProgrammableSingleSuccessorOperator", "An operator that can be programmed for arbitrary needs and handle a single successor.")]
  [StorableClass]
  public class ProgrammableSingleSuccessorOperator : ProgrammableOperator {

    [StorableConstructor]
    protected ProgrammableSingleSuccessorOperator(bool deserializing) : base(deserializing) { }
    protected ProgrammableSingleSuccessorOperator(ProgrammableSingleSuccessorOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public ProgrammableSingleSuccessorOperator()
      : base() {
      Parameters.Add(new OperatorParameter("Successor", "Operator that is executed next."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProgrammableSingleSuccessorOperator(this, cloner);
    }

    public override IOperation Apply() {
      IOperation operation = base.Apply();
      if (operation != null)
        return operation;
      IParameter parameter;
      Parameters.TryGetValue("Successor", out parameter);
      OperatorParameter successorParameter = parameter as OperatorParameter;
      if (successorParameter != null && successorParameter.Value != null)
        return ExecutionContext.CreateOperation(successorParameter.Value);
      else
        return null;
    }
  }
}
