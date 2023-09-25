#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Dynamic;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic; 

[Item("Dynamic Symbolic Regression Problem", "Applies different dataset partitions as a regression problem.")]
[Creatable(CreatableAttribute.Categories.Problems, Priority = 210)]
[StorableType("47E9498A-545C-47FE-AD2E-9B10DB683320")]
public class DynamicSymbolicRegressionProblem
  : SingleObjectiveStatefulDynamicProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree, DynamicSymbolicRegressionProblemState>,
    IProblemInstanceConsumer<DynamicRegressionProblemData> {
  #region Propeties
  public override bool Maximization => Parameters.ContainsKey(StateParameterName) ? State.Maximization : true;
  
  private const string ProblemDataParameterName = "ProblemData";
  private const string GrammarParameterName = "Grammar";
  private const string InterpreterParameterName = "Interpreter";
  
  public IValueParameter<DynamicRegressionProblemData> ProblemDataParameter => (IValueParameter<DynamicRegressionProblemData>)Parameters[ProblemDataParameterName];
  public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter => (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[GrammarParameterName];
  public IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter => (IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName];
  
  public DynamicRegressionProblemData ProblemData { get { return ProblemDataParameter.Value; } set { ProblemDataParameter.Value = value; } }
  public ISymbolicDataAnalysisGrammar Grammar { get { return GrammarParameter.Value; } set { GrammarParameter.Value = value; } }
  public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter { get { return InterpreterParameter.Value; } set { InterpreterParameter.Value = value; } }
  
  #endregion
  
  
  #region Constructors and Cloning
  public DynamicSymbolicRegressionProblem() {
    Parameters.Add(new ValueParameter<DynamicRegressionProblemData>(ProblemDataParameterName));
    Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, new TypeCoherentExpressionGrammar()));
    Parameters.Add(new ValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(InterpreterParameterName, new SymbolicDataAnalysisExpressionTreeLinearInterpreter()) { Hidden = true });
    
    
    
    Load(new DynamicRegressionProblemData());
    RegisterProblemEventHandlers();
  }

  private void RegisterProblemEventHandlers() {
    InitialState.SolutionCreatorChanged += (a, b) => SolutionCreator = InitialState.SolutionCreator;
  }

  [StorableConstructor]
  protected DynamicSymbolicRegressionProblem(StorableConstructorFlag _) : base(_) { }

    
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    RegisterProblemEventHandlers();
  }

  protected DynamicSymbolicRegressionProblem(DynamicSymbolicRegressionProblem original, Cloner cloner) : base(original, cloner) { }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new DynamicSymbolicRegressionProblem(this, cloner);
  }
  #endregion

  #region ProblemMethods
  protected override double Evaluate(Individual individual, IRandom random, bool dummy) {
    return State.Evaluate(individual, random);
  }

  protected override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random, bool dummy) {
    base.Analyze(individuals, qualities, results, random, dummy);
    State.Analyze(individuals, qualities, results, random);
  }

  #endregion

  public void Load(DynamicRegressionProblemData data) {
    ProblemDataParameter.Value = data;
    
    Grammar.ConfigureVariableSymbols(data);
    InitialState = new DynamicSymbolicRegressionProblemState(data, Interpreter, Grammar);
    Encoding = InitialState.Encoding;
    SolutionCreator = InitialState.SolutionCreator;
    
    Grammar.ConfigureVariableSymbols(data);
  }
}
