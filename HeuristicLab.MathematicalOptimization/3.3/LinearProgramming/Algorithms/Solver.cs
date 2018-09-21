using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {

  public class Solver : ParameterizedNamedItem, ISolver {

    [Storable]
    protected IFixedValueParameter<StringValue> libraryNameParam;


    [Storable]
    protected IFixedValueParameter<EnumValue<LinearProgrammingType>> programmingTypeParam;

    public Solver() {
      //  Parameters.Add(useMixedIntegerProgrammingParam = new FixedValueParameter<BoolValue>(nameof(UseMixedIntegerProgramming), (BoolValue)new BoolValue(false).AsReadOnly()));
      Parameters.Add(programmingTypeParam = new FixedValueParameter<EnumValue<LinearProgrammingType>>(nameof(LinearProgrammingType), new EnumValue<LinearProgrammingType>()));
    }

    [StorableConstructor]
    protected Solver(bool deserializing)
      : base(deserializing) { }

    protected Solver(Solver original, Cloner cloner)
      : base(original, cloner) {
      libraryNameParam = cloner.Clone(original.libraryNameParam);
      programmingTypeParam = cloner.Clone(original.programmingTypeParam);
    }

    public string LibraryName {
      get => libraryNameParam?.Value.Value;
      set => libraryNameParam.Value.Value = value;
    }

    public virtual OptimizationProblemType OptimizationProblemType { get; }

    public LinearProgrammingType LinearProgrammingType {
      get => programmingTypeParam.Value.Value;
      set => programmingTypeParam.Value.Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) => new Solver(this, cloner);
  }
}