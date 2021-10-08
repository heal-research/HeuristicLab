using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("7464E84B-65CC-440A-91F0-9FA920D730F9")]
  [Item(Name = "Structured Symbolic Regression Single Objective Problem (single-objective)", Description = "A problem with a structural definition and unfixed subfunctions.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class StructuredSymbolicRegressionSingleObjectiveProblem : SingleObjectiveBasicProblem<MultiEncoding>, IRegressionProblem, IProblemInstanceConsumer<RegressionProblemData> {

    #region Constants
    private const string ProblemDataParameterName = "ProblemData";
    private const string StructureDefinitionParameterName = "Structure Definition";
    private const string GrammarParameterName = "Grammar";
    private const string StructureTemplateParameterName = "Structure Template";
    private const string MainTreeResultParameterName = "Main Tree";
    #endregion

    #region Parameter
    public IValueParameter<IRegressionProblemData> ProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName];
    public IFixedValueParameter<StringValue> StructureDefinitionParameter => (IFixedValueParameter<StringValue>)Parameters[StructureDefinitionParameterName];
    public IFixedValueParameter<StructureTemplate> StructureTemplateParameter => (IFixedValueParameter<StructureTemplate>)Parameters[StructureTemplateParameterName];
    public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter => (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[GrammarParameterName];
    public IResultParameter<ISymbolicExpressionTree> MainTreeResultParameter => (IResultParameter<ISymbolicExpressionTree>)Parameters[MainTreeResultParameterName];
    #endregion

    #region Properties
    public IRegressionProblemData ProblemData { 
      get => ProblemDataParameter.Value; 
      set {
        ProblemDataParameter.Value = value;
        ProblemDataChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public string StructureDefinition {
      get => StructureDefinitionParameter.Value.Value;
      set => StructureDefinitionParameter.Value.Value = value;
    }

    public StructureTemplate StructureTemplate {
      get => StructureTemplateParameter.Value;
    }

    public ISymbolicDataAnalysisGrammar Grammar {
      get => GrammarParameter.Value;
      set => GrammarParameter.Value = value;
    }

    IParameter IDataAnalysisProblem.ProblemDataParameter => ProblemDataParameter;
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData => ProblemData;

    public override bool Maximization => false;
    #endregion

    #region EventHandlers
    public event EventHandler ProblemDataChanged;
    #endregion

    #region Constructors & Cloning
    public StructuredSymbolicRegressionSingleObjectiveProblem() {
      var problemData = new ShapeConstrainedRegressionProblemData();
      var grammar = new LinearScalingGrammar();
      var varSym = (Variable)grammar.GetSymbol("Variable");
      varSym.AllVariableNames = problemData.InputVariables.Select(x => x.Value);
      varSym.VariableNames = problemData.InputVariables.Select(x => x.Value);
      varSym.Enabled = true;

      var structureTemplate = new StructureTemplate();
      structureTemplate.Changed += (sender, args) => {
        foreach (var e in Encoding.Encodings.ToArray())
          Encoding.Remove(e);

        foreach(var sf in structureTemplate.SubFunctions.Values) {
          Encoding.Add(new SymbolicExpressionTreeEncoding(sf.Name, sf.Grammar, sf.MaximumSymbolicExpressionTreeLength, sf.MaximumSymbolicExpressionTreeDepth));
        }
      };

      Parameters.Add(new ValueParameter<RegressionProblemData>(ProblemDataParameterName, problemData));
      Parameters.Add(new FixedValueParameter<StructureTemplate>(StructureTemplateParameterName, structureTemplate));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, grammar));
      Parameters.Add(new ResultParameter<ISymbolicExpressionTree>(MainTreeResultParameterName, ""));
      MainTreeResultParameter.DefaultValue = new SymbolicExpressionTree();
    }

    public StructuredSymbolicRegressionSingleObjectiveProblem(StructuredSymbolicRegressionSingleObjectiveProblem original, Cloner cloner) { }

    [StorableConstructor]
    protected StructuredSymbolicRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    #endregion

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) =>
      new StructuredSymbolicRegressionSingleObjectiveProblem(this, cloner);
    #endregion

    public override double Evaluate(Individual individual, IRandom random) {
      var templateTree = (ISymbolicExpressionTree)StructureTemplate.Tree.Clone();
      var subFunctionDict = StructureTemplate.SubFunctions;
          
      // build main tree
      foreach (var n in templateTree.IterateNodesPrefix()) {
        if(n is SubFunctionTreeNode subFunctionTreeNode) {
          if(subFunctionDict.TryGetValue(subFunctionTreeNode, out SubFunction subFunction)) {
            var subFunctionTree = individual.SymbolicExpressionTree(subFunction.Name);
            var parent = n.Parent;
            // remove all subtrees

            var subtreeCount = parent.SubtreeCount;
            for (int idx = 0; idx < subtreeCount; ++idx)
              parent.RemoveSubtree(idx);

            // add new tree
            parent.AddSubtree(subFunctionTree.Root);
          }
        }
      }

      MainTreeResultParameter.ActualValue = templateTree;
      /*
      foreach (var kvp in individual.Values) {
        if(kvp.Value is SymbolicExpressionTree tree) {
          foreach(var n in tree.IterateNodesPrefix()) {
            if(n.Symbol is Variable v) {
              var t = v.VariableNames;
            }
          }
        }
      }
      */
      return 0.0;
    }

    public void Load(RegressionProblemData data) {
      ProblemData = data;
    }
  }
}
