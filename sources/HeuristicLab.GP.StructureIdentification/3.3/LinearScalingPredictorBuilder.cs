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
      AddVariableInfo(new VariableInfo("Beta", "Beta parameter for linear scaling as calculated by LinearScaler", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Alpha", "Alpha parameter for linear scaling as calculated by LinearScaler", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("UpperEstimationLimit", "Upper limit for estimated value (optional)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LowerEstimationLimit", "Lower limit for estimated value (optional)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Predictor", "The predictor combines the function tree and the evaluator and can be used to generate estimated values", typeof(IPredictor), VariableKind.New));
    }

    public override string Description {
      get { return "Extracts the function tree scales the output of the tree and combines the scaled tree with a HL3TreeEvaluator to a predictor for the model analyzer."; }
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel model = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      //double punishmentFactor = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data;
      //Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      //int start = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      //int end = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      //string targetVariable = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      double alpha = GetVariableValue<DoubleData>("Alpha", scope, true).Data;
      double beta = GetVariableValue<DoubleData>("Beta", scope, true).Data;
      DoubleData lowerLimit = GetVariableValue<DoubleData>("LowerEstimationLimit", scope, true, false);
      DoubleData upperLimit = GetVariableValue<DoubleData>("UpperEstimationLimit", scope, true, false);
      IPredictor predictor;
      if (lowerLimit == null || upperLimit == null)
        predictor = CreatePredictor(model, beta, alpha, double.NegativeInfinity, double.PositiveInfinity);
      else
        predictor = CreatePredictor(model, beta, alpha, lowerLimit.Data, upperLimit.Data);
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Predictor"), predictor));
      return null;
    }

    public static IPredictor CreatePredictor(IGeneticProgrammingModel model, double beta, double alpha, double lowerLimit, double upperLimit) {

      var evaluator = new HL3TreeEvaluator();
      evaluator.LowerEvaluationLimit = lowerLimit;
      evaluator.UpperEvaluationLimit = upperLimit;
      var resultModel = new GeneticProgrammingModel(MakeSum(MakeProduct(model.FunctionTree, beta), alpha));
      return new Predictor(evaluator, resultModel, lowerLimit, upperLimit);
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
  }
}
