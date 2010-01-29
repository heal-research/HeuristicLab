#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Modeling;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Common;
namespace HeuristicLab.GP.StructureIdentification {
  public class LinearScalingPredictorBuilder : OperatorBase {
    public LinearScalingPredictorBuilder()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "The punishment factor limits the estimated values to a certain range", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "The dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "Start index of training set", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "End index of training set", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Name of the target variable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Predictor", "The predictor combines the function tree and the evaluator and can be used to generate estimated values", typeof(IPredictor), VariableKind.New));
    }

    public override string Description {
      get { return "Extracts the function tree scales the output of the tree and combines the scaled tree with a HL3TreeEvaluator to a predictor for the model analyzer."; }
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel model = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      double punishmentFactor = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int start = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      string targetVariable = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      IPredictor predictor = CreatePredictor(model, punishmentFactor, dataset, targetVariable, start, end);
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Predictor"), predictor));
      return null;
    }

    public static IPredictor CreatePredictor(IGeneticProgrammingModel model, double punishmentFactor,
  Dataset dataset, string targetVariable, int start, int end) {
      return CreatePredictor(model, punishmentFactor, dataset, dataset.GetVariableIndex(targetVariable), start, end);
    }


    public static IPredictor CreatePredictor(IGeneticProgrammingModel model, double punishmentFactor,
      Dataset dataset, int targetVariable, int start, int end) {

      var evaluator = new HL3TreeEvaluator();
      // evaluate for all rows
      evaluator.PrepareForEvaluation(dataset, model.FunctionTree);
      var result = from row in Enumerable.Range(start, end - start)
                   let y = evaluator.Evaluate(row)
                   let y_ = dataset.GetValue(row, targetVariable)
                   select new { Row = row, Estimation = y, Target = y_ };

      // calculate alpha and beta on the subset of rows with valid values 
      var filteredResult = result.Where(x => IsValidValue(x.Target) && IsValidValue(x.Estimation));
      var target = filteredResult.Select(x => x.Target);
      var estimation = filteredResult.Select(x => x.Estimation);
      double a, b;
      if (filteredResult.Count() > 2) {
        double tMean = target.Sum() / target.Count();
        double xMean = estimation.Sum() / estimation.Count();
        double sumXT = 0;
        double sumXX = 0;
        foreach (var r in result) {
          double x = r.Estimation;
          double t = r.Target;
          sumXT += (x - xMean) * (t - tMean);
          sumXX += (x - xMean) * (x - xMean);
        }
        b = sumXT / sumXX;
        a = tMean - b * xMean;
      } else {
        b = 1.0;
        a = 0.0;
      }
      double mean = dataset.GetMean(targetVariable, start, end);
      double range = dataset.GetRange(targetVariable, start, end);
      double minEstimatedValue = mean - punishmentFactor * range;
      double maxEstimatedValue = mean + punishmentFactor * range;
      evaluator.LowerEvaluationLimit = minEstimatedValue;
      evaluator.UpperEvaluationLimit = maxEstimatedValue;
      var resultModel = new GeneticProgrammingModel(MakeSum(MakeProduct(model.FunctionTree, b), a));
      return new Predictor(evaluator, resultModel, minEstimatedValue, maxEstimatedValue);
    }

    private static bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d);
    }


    private static IFunctionTree MakeSum(IFunctionTree tree, double x) {
      if (x.IsAlmost(0.0)) return tree;
      var sum = (new Addition()).GetTreeNode();
      sum.AddSubTree(tree);
      sum.AddSubTree(MakeConstant(x));
      return sum;
    }

    private static IFunctionTree MakeProduct(IFunctionTree tree, double a) {
      if (a.IsAlmost(1.0)) return tree;
      var prod = (new Multiplication()).GetTreeNode();
      prod.AddSubTree(tree);
      prod.AddSubTree(MakeConstant(a));
      return prod;
    }

    private static IFunctionTree MakeConstant(double x) {
      var constX = (ConstantFunctionTree)(new Constant()).GetTreeNode();
      constX.Value = x;
      return constX;
    }

    private static void CalculateScalingParameters(IEnumerable<double> xs, IEnumerable<double> ys, out double k, out double d) {
      if (xs.Count() != ys.Count()) throw new ArgumentException();
      double xMean = xs.Sum() / xs.Count();
      double yMean = ys.Sum() / ys.Count();

      var yEnumerator = ys.GetEnumerator();
      var xEnumerator = xs.GetEnumerator();

      double sumXY = 0.0;
      double sumXX = 0.0;
      while (xEnumerator.MoveNext() && yEnumerator.MoveNext()) {
        sumXY += (xEnumerator.Current - xMean) * (yEnumerator.Current - yMean);
        sumXX += (xEnumerator.Current - xMean) * (xEnumerator.Current - xMean);
      }

      k = sumXY / sumXX;
      d = yMean - k * xMean;
    }
  }
}
