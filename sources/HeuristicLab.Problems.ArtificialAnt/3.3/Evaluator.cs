#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Operators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ArtificialAnt {
  [Item("ArtificialAntEvaluator", "Evaluates an artificial ant solution.")]
  [StorableClass]
  public class Evaluator : SingleSuccessorOperator, ISingleObjectiveEvaluator {

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters["SymbolicExpressionTree"]; }
    }
    public Evaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the evaluated artificial ant solution."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>("SymbolicExpressionTree", "The artificial ant solution encoded as a symbolic expression tree that should be evaluated"));
    }

    public sealed override IOperation Apply() {

      SymbolicExpressionTree solution = SymbolicExpressionTreeParameter.ActualValue;
      AntInterpreter interpreter = new AntInterpreter();
      interpreter.MaxTimeSteps = 600;
      interpreter.Run(solution);

      QualityParameter.ActualValue = new DoubleValue(interpreter.FoodEaten);
      return null;
    }
  }
}
