#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.ArtificialAnt {
  [Item("ArtificialAntEvaluator", "Evaluates an artificial ant solution.")]
  [StorableClass]
  public class Evaluator : InstrumentedOperator, ISingleObjectiveEvaluator {

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters["SymbolicExpressionTree"]; }
    }
    public ILookupParameter<BoolMatrix> WorldParameter {
      get { return (ILookupParameter<BoolMatrix>)Parameters["World"]; }
    }
    public ILookupParameter<IntValue> MaxTimeStepsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaxTimeSteps"]; }
    }

    [StorableConstructor]
    protected Evaluator(bool deserializing) : base(deserializing) { }
    protected Evaluator(Evaluator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new Evaluator(this, cloner); }
    public Evaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the evaluated artificial ant solution."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>("SymbolicExpressionTree", "The artificial ant solution encoded as a symbolic expression tree that should be evaluated"));
      Parameters.Add(new LookupParameter<BoolMatrix>("World", "The world for the artificial ant with scattered food items."));
      Parameters.Add(new LookupParameter<IntValue>("MaxTimeSteps", "The maximal number of time steps that the artificial ant should be simulated."));
    }

    public sealed override IOperation InstrumentedApply() {
      SymbolicExpressionTree expression = SymbolicExpressionTreeParameter.ActualValue;
      BoolMatrix world = WorldParameter.ActualValue;
      IntValue maxTimeSteps = MaxTimeStepsParameter.ActualValue;

      AntInterpreter interpreter = new AntInterpreter();
      interpreter.MaxTimeSteps = maxTimeSteps.Value;
      interpreter.World = world;
      interpreter.Expression = expression;
      interpreter.Run();

      QualityParameter.ActualValue = new DoubleValue(interpreter.FoodEaten);
      return base.InstrumentedApply();
    }
  }
}
