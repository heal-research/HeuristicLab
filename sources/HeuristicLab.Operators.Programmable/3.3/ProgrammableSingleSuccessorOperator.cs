
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Operators.Programmable {

  [Item("ProgrammableSingleSuccessorOperator", "An operator that can be programmed for arbitrary needs and handle a single successor.")]
  [StorableClass]
  public class ProgrammableSingleSuccessorOperator : ProgrammableOperator {

    public IOperator Successor {
      get {
        IParameter parameter;
        Parameters.TryGetValue("Successor", out parameter);
        OperatorParameter successorParameter = parameter as OperatorParameter;
        if (successorParameter == null)
          return null;
        return successorParameter.Value;
      }
      set {
        ((OperatorParameter)Parameters["Successor"]).Value = value;
      }
    }

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

    public override string MethodSuffix {
      get { return "return op.Successor == null ? null : context.CreateOperation(op.Successor);"; }
    }
  }
}
