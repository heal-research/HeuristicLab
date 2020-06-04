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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [Item("IntegerVectorEncoding", "Describes an integer vector encoding.")]
  [StorableType("15D6E55E-C39F-4784-8350-14A0FD47CF0E")]
  public sealed class IntegerVectorEncoding : VectorEncoding<IntegerVector> {
    [Storable] public IValueParameter<IntMatrix> BoundsParameter { get; private set; }

    public IntMatrix Bounds {
      get { return BoundsParameter.Value; }
      set {
        if (value == null) throw new ArgumentNullException("Bounds must not be null.");
        if (Bounds == value) return;
        BoundsParameter.Value = value;
      }
    }

    [StorableConstructor]
    private IntegerVectorEncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      DiscoverOperators();
      RegisterEventHandlers();
    }

    private IntegerVectorEncoding(IntegerVectorEncoding original, Cloner cloner)
      : base(original, cloner) {
      BoundsParameter = cloner.Clone(original.BoundsParameter);
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new IntegerVectorEncoding(this, cloner); }


    public IntegerVectorEncoding() : this("IntegerVector", 10) { }
    public IntegerVectorEncoding(string name) : this(name, 10) { }
    public IntegerVectorEncoding(int length) : this("integerVector", length) { }
    public IntegerVectorEncoding(string name, int length, int min = int.MinValue, int max = int.MaxValue, int? step = null)
      : base(name, length) {
      if (min >= max) throw new ArgumentException("min must be less than max", "min");
      if (step.HasValue && step.Value <= 0) throw new ArgumentException("step must be greater than zero or null", "step");

      var bounds = new IntMatrix(1, step.HasValue ? 3 : 2);
      bounds[0, 0] = min;
      bounds[0, 1] = max;
      if (step.HasValue) bounds[0, 2] = step.Value;

      BoundsParameter = new ValueParameter<IntMatrix>(Name + ".Bounds", bounds);
      Parameters.Add(BoundsParameter);

      SolutionCreator = new UniformRandomIntegerVectorCreator();
      DiscoverOperators();
      RegisterEventHandlers();
    }
    public IntegerVectorEncoding(string name, int length, IList<int> min, IList<int> max, IList<int> step = null)
      : base(name, length) {
      if (min.Count == 0) throw new ArgumentException("Bounds must be given for the integer parameters.");
      if (min.Count != max.Count) throw new ArgumentException("min must be of the same length as max", "min");
      if (step != null && min.Count != step.Count) throw new ArgumentException("step must be of the same length as min or null", "step");
      if (min.Zip(max, (mi, ma) => mi >= ma).Any(x => x)) throw new ArgumentException("min must be less than max in each dimension", "min");

      var bounds = new IntMatrix(min.Count, step != null ? 3 : 2);
      for (int i = 0; i < min.Count; i++) {
        bounds[i, 0] = min[i];
        bounds[i, 1] = max[i];
        if (step != null) bounds[i, 2] = step[i];
      }

      BoundsParameter = new ValueParameter<IntMatrix>(Name + ".Bounds", bounds);
      Parameters.Add(BoundsParameter);

      SolutionCreator = new UniformRandomIntegerVectorCreator();
      DiscoverOperators();
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      IntMatrixParameterChangeHandler.Create(BoundsParameter, () => {
        ConfigureOperators(Operators);
        OnBoundsChanged();
      });
    }


    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static IntegerVectorEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
        typeof (IIntegerVectorOperator),
        typeof (IIntegerVectorCreator),
        typeof (IIntegerVectorCrossover),
        typeof (IIntegerVectorManipulator),
        typeof (IIntegerVectorStdDevStrategyParameterOperator),
        typeof (IIntegerVectorMultiNeighborhoodShakingOperator),
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IIntegerVectorOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      base.ConfigureOperators(operators);
      ConfigureBoundedOperators(operators.OfType<IBoundedIntegerVectorOperator>());
      ConfigureCreators(operators.OfType<IIntegerVectorCreator>());
      ConfigureCrossovers(operators.OfType<IIntegerVectorCrossover>());
      ConfigureManipulators(operators.OfType<IIntegerVectorManipulator>());
      ConfigureShakingOperators(operators.OfType<IIntegerVectorMultiNeighborhoodShakingOperator>());
      ConfigureStrategyVectorOperator(operators.OfType<IIntegerVectorStdDevStrategyParameterOperator>());
    }

    #region Specific Operator Wiring
    private void ConfigureBoundedOperators(IEnumerable<IBoundedIntegerVectorOperator> boundedOperators) {
      foreach (var boundedOperator in boundedOperators) {
        boundedOperator.BoundsParameter.ActualName = BoundsParameter.Name;
      }
    }

    private void ConfigureCreators(IEnumerable<IIntegerVectorCreator> creators) {
      foreach (var creator in creators) {
        creator.IntegerVectorParameter.ActualName = Name;
        creator.BoundsParameter.ActualName = BoundsParameter.Name;
        creator.LengthParameter.ActualName = LengthParameter.Name;
      }
    }

    private void ConfigureCrossovers(IEnumerable<IIntegerVectorCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ChildParameter.ActualName = Name;
        crossover.ParentsParameter.ActualName = Name;
      }
    }

    private void ConfigureManipulators(IEnumerable<IIntegerVectorManipulator> manipulators) {
      foreach (var manipulator in manipulators) {
        manipulator.IntegerVectorParameter.ActualName = Name;
        manipulator.IntegerVectorParameter.Hidden = true;
        var sm = manipulator as ISelfAdaptiveManipulator;
        if (sm != null) {
          var p = sm.StrategyParameterParameter as ILookupParameter;
          if (p != null) {
            p.ActualName = Name + "Strategy";
          }
        }
      }
    }

    private void ConfigureShakingOperators(IEnumerable<IIntegerVectorMultiNeighborhoodShakingOperator> shakingOperators) {
      foreach (var shakingOperator in shakingOperators) {
        shakingOperator.IntegerVectorParameter.ActualName = Name;
      }
    }

    private void ConfigureStrategyVectorOperator(IEnumerable<IIntegerVectorStdDevStrategyParameterOperator> strategyVectorOperators) {
      var bounds = new DoubleMatrix(Bounds.Rows, Bounds.Columns);
      for (var i = 0; i < bounds.Rows; i++) {
        bounds[i, 1] = (int)Math.Ceiling(0.33 * (Bounds[i, 1] - Bounds[i, 0]));
        bounds[i, 0] = 0;
        if (bounds.Columns > 2) bounds[i, 2] = Bounds[i, 2];
      }
      foreach (var s in strategyVectorOperators) {
        var c = s as IIntegerVectorStdDevStrategyParameterCreator;
        if (c != null) {
          c.BoundsParameter.Value = (DoubleMatrix)bounds.Clone();
          c.LengthParameter.ActualName = Name;
          c.StrategyParameterParameter.ActualName = Name + "Strategy";
        }
        var m = s as IIntegerVectorStdDevStrategyParameterManipulator;
        if (m != null) {
          m.BoundsParameter.Value = (DoubleMatrix)bounds.Clone();
          m.StrategyParameterParameter.ActualName = Name + "Strategy";
        }
        var mm = s as StdDevStrategyVectorManipulator;
        if (mm != null) {
          mm.GeneralLearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Length));
          mm.LearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Math.Sqrt(Length)));
        }
        var x = s as IIntegerVectorStdDevStrategyParameterCrossover;
        if (x != null) {
          x.ParentsParameter.ActualName = Name + "Strategy";
          x.StrategyParameterParameter.ActualName = Name + "Strategy";
        }
      }
    }
    #endregion

    protected override void OnLengthChanged() {
      ConfigureOperators(Operators);
      base.OnLengthChanged();
    }

    public event EventHandler BoundsChanged;
    private void OnBoundsChanged() {
      BoundsChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
