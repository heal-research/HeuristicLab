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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.JsonInterface;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Shape-constrained symbolic regression problem (single-objective)", "Represents a single-objective shape-constrained regression problem.")]
  [StorableType("B35ADCA7-E902-4BEE-9DDE-DF8BBC1E27FE")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class ShapeConstrainedRegressionSingleObjectiveProblem : SymbolicRegressionSingleObjectiveProblem, IShapeConstrainedRegressionProblem {
    [StorableConstructor]
    protected ShapeConstrainedRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    protected ShapeConstrainedRegressionSingleObjectiveProblem(ShapeConstrainedRegressionSingleObjectiveProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new ShapeConstrainedRegressionSingleObjectiveProblem(this, cloner); }

    public ShapeConstrainedRegressionProblemData ShapeConstrainedRegressionProblemData {
      get => (ShapeConstrainedRegressionProblemData)ProblemData;
      set => ProblemData = value;
    }
    public ShapeConstrainedRegressionSingleObjectiveProblem()
      : base(new ShapeConstrainedRegressionProblemData(), new NMSESingleObjectiveConstraintsEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {

      ApplyLinearScalingParameter.Value.Value = true;
      Maximization.Value = false;
      SymbolicExpressionTreeGrammarParameter.Value = new LinearScalingGrammar();

      InitializeOperators();
    }


    private void InitializeOperators() {
      Operators.Add(new ShapeConstraintsAnalyzer());
      ParameterizeOperators();
    }

    public override void Load(IRegressionProblemData data) {
      var scProblemData = data as ShapeConstrainedRegressionProblemData;
      if (scProblemData == null) {
        scProblemData = new ShapeConstrainedRegressionProblemData(data.Dataset, data.AllowedInputVariables, data.TargetVariable,
                                                                  data.TrainingPartition, data.TestPartition) {
          Name = data.Name,
          Description = data.Description
        };
      }

      base.Load(scProblemData);
    }

    public override void Inject(JsonItem item, JsonItemConverter converter) {
      base.Inject(item, converter);
      // reinstantiate problem data with a shape constrained problem data
      ProblemData = new ShapeConstrainedRegressionProblemData(ProblemData);
      var constraintsText = item.GetProperty<string[]>(nameof(ShapeConstrainedRegressionProblemData.ShapeConstraints));
      var parsedConstraints = ShapeConstraintsParser.ParseConstraints(string.Join("\n", constraintsText));
      var constraints = ShapeConstrainedRegressionProblemData.ShapeConstraints;
      constraints.Clear();
      constraints.AddRange(parsedConstraints);
    }

    public override JsonItem Extract(JsonItemConverter converter) {
      var item = base.Extract(converter);
      var constraints = ShapeConstrainedRegressionProblemData.ShapeConstraints;
      item.AddProperty(
        nameof(ShapeConstrainedRegressionProblemData.ShapeConstraints), 
        constraints.Select(x => x.ToString()).ToArray());
      return item;
    }
  }
}
