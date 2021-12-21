using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("598B5DCB-95AC-465A-920B-E1E6DACFFA4B")]
  public class SubFunction : ParameterizedNamedItem {
    #region Constants
    private const string GrammarParameterName = "Grammar";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    #endregion

    #region Parameters
    public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter => (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[GrammarParameterName];
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter => (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName];
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter => (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName];
    #endregion

    #region Properties
    public ISymbolicDataAnalysisGrammar Grammar {
      get => GrammarParameter.Value;
      set => GrammarParameter.Value = value;
    }

    public int MaximumSymbolicExpressionTreeDepth {
      get => MaximumSymbolicExpressionTreeDepthParameter.Value.Value;
      set => MaximumSymbolicExpressionTreeDepthParameter.Value.Value = value;
    }

    public int MaximumSymbolicExpressionTreeLength {
      get => MaximumSymbolicExpressionTreeLengthParameter.Value.Value;
      set => MaximumSymbolicExpressionTreeLengthParameter.Value.Value = value;
    }

    [Storable]
    public IEnumerable<string> Arguments { get; set; }
    #endregion

    #region Events
    public event EventHandler Changed;
    #endregion

    #region Constructors
    public SubFunction() {
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, new ArithmeticExpressionGrammar()));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, new IntValue(8)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, new IntValue(20)));

      GrammarParameter.ValueChanged += OnParameterValueChanged;
      MaximumSymbolicExpressionTreeDepthParameter.Value.ValueChanged += OnParameterValueChanged; 
      MaximumSymbolicExpressionTreeLengthParameter.Value.ValueChanged += OnParameterValueChanged;
    }

    private void OnParameterValueChanged(object sender, EventArgs e) => Changed?.Invoke(this, EventArgs.Empty);

    protected SubFunction(SubFunction original, Cloner cloner) : base(original, cloner) {
      Arguments = original.Arguments;
    }

    [StorableConstructor]
    protected SubFunction(StorableConstructorFlag _) : base(_) {}
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new SubFunction(this, cloner);
    #endregion

    public override string ToString() => $"{Name}({string.Join(",", Arguments)})";

    public override int GetHashCode() => ToString().GetHashCode();

    public override bool Equals(object obj) => (obj is SubFunction other) && other.ToString() == ToString();
  }
}
