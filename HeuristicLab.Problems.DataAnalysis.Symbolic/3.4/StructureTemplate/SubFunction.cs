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
    private const string FunctionArgumentsParameterName = "Function Arguments";
    #endregion

    #region Parameters
    public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter => (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[GrammarParameterName];
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter => (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName];
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter => (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName];
    public IValueParameter<ReadOnlyItemList<StringValue>> FunctionArgumentsParameter => (IValueParameter<ReadOnlyItemList<StringValue>>)Parameters[FunctionArgumentsParameterName];
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

    public IEnumerable<string> FunctionArguments { // TODO: gehört weg
      get => FunctionArgumentsParameter.Value.Select(x => x.Value); 
      set {
        var varSym = (Variable)Grammar.GetSymbol("Variable");
        if (varSym == null)
          throw new ArgumentException($"No variable symbol existent.");

        FunctionArgumentsParameter.Value = new ItemList<StringValue>(value.Select(x => new StringValue(x))).AsReadOnly();

        varSym.AllVariableNames = FunctionArguments;
        varSym.VariableNames = FunctionArguments;
        varSym.Enabled = true;
      } 
    }
    #endregion

    #region Constructors
    public SubFunction() {
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, new LinearScalingGrammar()));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, new IntValue(30)));
      Parameters.Add(new ValueParameter<ReadOnlyItemList<StringValue>>(FunctionArgumentsParameterName, new ReadOnlyItemList<StringValue>()));
    }

    protected SubFunction(SubFunction original, Cloner cloner) { }

    [StorableConstructor]
    protected SubFunction(StorableConstructorFlag _) : base(_) {}
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new SubFunction(this, cloner);
    #endregion
  }
}
