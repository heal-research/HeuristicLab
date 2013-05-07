#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  [Item("Lawn Mower Evaluator", "Evaluator for the lawn mower demo GP problem.")]
  public class Evaluator : SingleSuccessorOperator, ISingleObjectiveEvaluator {

    private const string QualityParameterName = "Quality";
    private const string LawnMowerProgramParameterName = "LawnMowerProgram";
    private const string LawnWidthParameterName = "LawnWidth";
    private const string LawnLengthParameterName = "LawnLength";

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<ISymbolicExpressionTree> LawnMowerProgramParameter {
      get { return (ILookupParameter<ISymbolicExpressionTree>)Parameters[LawnMowerProgramParameterName]; }
    }
    public ILookupParameter<IntValue> LawnWidthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[LawnWidthParameterName]; }
    }
    public ILookupParameter<IntValue> LawnLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[LawnLengthParameterName]; }
    }

    [StorableConstructor]
    protected Evaluator(bool deserializing) : base(deserializing) { }
    protected Evaluator(Evaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    public Evaluator() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The solution quality of the lawn mower program."));
      Parameters.Add(new LookupParameter<ISymbolicExpressionTree>(LawnMowerProgramParameterName, "The lawn mower program to evaluate represented as symbolic expression tree."));
      Parameters.Add(new LookupParameter<IntValue>(LawnWidthParameterName, "The width of the lawn to mow."));
      Parameters.Add(new LookupParameter<IntValue>(LawnLengthParameterName, "The length of the lawn to mow."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Evaluator(this, cloner);
    }
    public override IOperation Apply() {
      int length = LawnLengthParameter.ActualValue.Value;
      int width = LawnWidthParameter.ActualValue.Value;
      ISymbolicExpressionTree tree = LawnMowerProgramParameter.ActualValue;

      bool[,] lawn = Interpreter.EvaluateLawnMowerProgram(length, width, tree);

      // count number of squares that have been mowed
      int numberOfMowedCells = 0;
      for (int i = 0; i < length; i++)
        for (int j = 0; j < width; j++)
          if (lawn[i, j]) {
            numberOfMowedCells++;
          }

      QualityParameter.ActualValue = new DoubleValue(numberOfMowedCells);
      return base.Apply();
    }
  }
}
