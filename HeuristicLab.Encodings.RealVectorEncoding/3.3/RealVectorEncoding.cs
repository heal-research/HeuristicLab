#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("RealVectorEncoding", "Describes a real vector encoding.")]
  [StorableClass]
  public sealed class RealVectorEncoding : Encoding<RealVector> {
    #region Encoding Parameters
    [Storable]
    private IFixedValueParameter<IntValue> lengthParameter;
    public IFixedValueParameter<IntValue> LengthParameter {
      get { return lengthParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Length parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Length parameter value must not be null.");
        if (lengthParameter == value) return;

        if (lengthParameter != null) Parameters.Remove(lengthParameter);
        lengthParameter = value;
        Parameters.Add(lengthParameter);
        OnLengthParameterChanged();
      }
    }
    [Storable]
    private IValueParameter<DoubleMatrix> boundsParameter;
    public IValueParameter<DoubleMatrix> BoundsParameter {
      get { return boundsParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Bounds parameter must not be null.");
        if (boundsParameter == value) return;

        if (boundsParameter != null) Parameters.Remove(boundsParameter);
        boundsParameter = value;
        Parameters.Add(boundsParameter);
        OnBoundsParameterChanged();
      }
    }
    #endregion

    public int Length {
      get { return LengthParameter.Value.Value; }
      set { LengthParameter.Value.Value = value; }
    }
    public DoubleMatrix Bounds {
      get { return BoundsParameter.Value; }
      set { BoundsParameter.Value = value; }
    }

    [StorableConstructor]
    private RealVectorEncoding(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
      DiscoverOperators();
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new RealVectorEncoding(this, cloner); }
    private RealVectorEncoding(RealVectorEncoding original, Cloner cloner)
      : base(original, cloner) {
      lengthParameter = cloner.Clone(original.lengthParameter);
      boundsParameter = cloner.Clone(original.boundsParameter);
      RegisterParameterEvents();
    }

    public RealVectorEncoding() : this("RealVector", 10) { }
    public RealVectorEncoding(string name) : this(name, 10) { }
    public RealVectorEncoding(int length) : this("RealVector", length) { }
    public RealVectorEncoding(string name, int length, double min = -1000, double max = 1000)
      : base(name) {
      if (min >= max) throw new ArgumentException("min must be less than max", "min");

      var bounds = new DoubleMatrix(1, 2);
      bounds[0, 0] = min;
      bounds[0, 1] = max;

      lengthParameter = new FixedValueParameter<IntValue>(Name + ".Length", new IntValue(length));
      boundsParameter = new ValueParameter<DoubleMatrix>(Name + ".Bounds", bounds);
      Parameters.Add(lengthParameter);
      Parameters.Add(boundsParameter);

      SolutionCreator = new UniformRandomRealVectorCreator();
      RegisterParameterEvents();
      DiscoverOperators();
    }

    public RealVectorEncoding(string name, int length, IList<double> min, IList<double> max)
      : base(name) {
      if (min.Count == 0) throw new ArgumentException("Bounds must be given for the real parameters.");
      if (min.Count != max.Count) throw new ArgumentException("min must be of the same length as max", "min");
      if (min.Zip(max, (mi, ma) => mi >= ma).Any(x => x)) throw new ArgumentException("min must be less than max in each dimension", "min");

      var bounds = new DoubleMatrix(min.Count, 2);
      for (int i = 0; i < min.Count; i++) {
        bounds[i, 0] = min[i];
        bounds[i, 1] = max[i];
      }
      lengthParameter = new FixedValueParameter<IntValue>(Name + ".Length", new IntValue(length));
      boundsParameter = new ValueParameter<DoubleMatrix>(Name + ".Bounds", bounds);
      Parameters.Add(lengthParameter);
      Parameters.Add(boundsParameter);

      SolutionCreator = new UniformRandomRealVectorCreator();
      RegisterParameterEvents();
      DiscoverOperators();
    }

    private void OnLengthParameterChanged() {
      RegisterLengthParameterEvents();
      ConfigureOperators(Operators);
    }
    private void OnBoundsParameterChanged() {
      RegisterBoundsParameterEvents();
      ConfigureOperators(Operators);
    }

    private void RegisterParameterEvents() {
      RegisterLengthParameterEvents();
      RegisterBoundsParameterEvents();
    }
    private void RegisterLengthParameterEvents() {
      LengthParameter.ValueChanged += (o, s) => ConfigureOperators(Operators);
      LengthParameter.Value.ValueChanged += (o, s) => ConfigureOperators(Operators);
    }
    private void RegisterBoundsParameterEvents() {
      BoundsParameter.ValueChanged += (o, s) => ConfigureOperators(Operators);
      boundsParameter.Value.ToStringChanged += (o, s) => ConfigureOperators(Operators);
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static RealVectorEncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IRealVectorOperator),
          typeof (IRealVectorCreator),
          typeof (IRealVectorCrossover),
          typeof (IRealVectorManipulator),
          typeof (IRealVectorStdDevStrategyParameterOperator),
          typeof (IRealVectorSwarmUpdater),
          typeof (IRealVectorParticleCreator),
          typeof (IRealVectorParticleUpdater),
          typeof (IRealVectorMultiNeighborhoodShakingOperator),
          typeof (IRealVectorBoundsChecker),
          typeof (IRealVectorMoveOperator),
          typeof (IRealVectorMoveGenerator),
          typeof (IRealVectorSolutionOperator),
          typeof (IRealVectorSolutionsOperator),
          typeof (IRealVectorBoundedOperator)
      };
    }
    private void DiscoverOperators() {
      var assembly = typeof(IRealVectorOperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);

      foreach (var strategyVectorCreator in Operators.OfType<IRealVectorStdDevStrategyParameterCreator>())
        strategyVectorCreator.BoundsParameter.ValueChanged += strategyVectorCreator_BoundsParameter_ValueChanged;
    }
    #endregion


    private void strategyVectorCreator_BoundsParameter_ValueChanged(object sender, EventArgs e) {
      var boundsParameter = (IValueLookupParameter<DoubleMatrix>)sender;
      if (boundsParameter.Value == null) return;
      foreach (var strategyVectorManipulator in Operators.OfType<IRealVectorStdDevStrategyParameterManipulator>())
        strategyVectorManipulator.BoundsParameter.Value = (DoubleMatrix)boundsParameter.Value.Clone();
    }

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      ConfigureCreators(operators.OfType<IRealVectorCreator>());
      ConfigureCrossovers(operators.OfType<IRealVectorCrossover>());
      ConfigureManipulators(operators.OfType<IRealVectorManipulator>());
      ConfigureStdDevStrategyParameterOperators(operators.OfType<IRealVectorStdDevStrategyParameterOperator>());
      ConfigureSwarmUpdaters(operators.OfType<IRealVectorSwarmUpdater>());
      ConfigureParticleCreators(operators.OfType<IRealVectorParticleCreator>());
      ConfigureParticleUpdaters(operators.OfType<IRealVectorParticleUpdater>());
      ConfigureShakingOperators(operators.OfType<IRealVectorMultiNeighborhoodShakingOperator>());
      ConfigureBoundsCheckers(operators.OfType<IRealVectorBoundsChecker>());
      ConfigureMoveGenerators(operators.OfType<IRealVectorMoveGenerator>());
      ConfigureMoveOperators(operators.OfType<IRealVectorMoveOperator>());
      ConfigureAdditiveMoveOperator(operators.OfType<IRealVectorAdditiveMoveOperator>());
      ConfigureRealVectorSolutionOperators(operators.OfType<IRealVectorSolutionOperator>());
      ConfigureRealVectorSolutionsOperators(operators.OfType<IRealVectorSolutionsOperator>());
      ConfigureRealVectorBoundedOperators(operators.OfType<IRealVectorBoundedOperator>());
    }

    #region Specific Operator Wiring
    private void ConfigureCreators(IEnumerable<IRealVectorCreator> creators) {
      foreach (var creator in creators) {
        creator.LengthParameter.ActualName = LengthParameter.Name;
      }
    }
    private void ConfigureCrossovers(IEnumerable<IRealVectorCrossover> crossovers) {
      foreach (var crossover in crossovers) {
        crossover.ChildParameter.ActualName = Name;
        crossover.ParentsParameter.ActualName = Name;
      }
    }
    private void ConfigureManipulators(IEnumerable<IRealVectorManipulator> manipulators) {
      foreach (var manipulator in manipulators) {
        var sm = manipulator as ISelfAdaptiveManipulator;
        if (sm != null) {
          var p = sm.StrategyParameterParameter as ILookupParameter;
          if (p != null) {
            p.ActualName = Name + ".Strategy";
          }
        }
      }
    }
    private void ConfigureStdDevStrategyParameterOperators(IEnumerable<IRealVectorStdDevStrategyParameterOperator> strategyOperators) {
      var bounds = new DoubleMatrix(Bounds.Rows, Bounds.Columns);
      for (var i = 0; i < Bounds.Rows; i++) {
        bounds[i, 0] = 0;
        bounds[i, 1] = 0.1 * (Bounds[i, 1] - Bounds[i, 0]);
      }
      foreach (var s in strategyOperators) {
        var c = s as IRealVectorStdDevStrategyParameterCreator;
        if (c != null) {
          c.BoundsParameter.Value = (DoubleMatrix)bounds.Clone();
          c.LengthParameter.ActualName = LengthParameter.Name;
          c.StrategyParameterParameter.ActualName = Name + ".Strategy";
        }
        var m = s as IRealVectorStdDevStrategyParameterManipulator;
        if (m != null) {
          m.BoundsParameter.Value = (DoubleMatrix)bounds.Clone();
          m.StrategyParameterParameter.ActualName = Name + ".Strategy";
        }
        var mm = s as StdDevStrategyVectorManipulator;
        if (mm != null) {
          mm.GeneralLearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Length));
          mm.LearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Math.Sqrt(Length)));
        }
        var x = s as IRealVectorStdDevStrategyParameterCrossover;
        if (x != null) {
          x.ParentsParameter.ActualName = Name + ".Strategy";
          x.StrategyParameterParameter.ActualName = Name + ".Strategy";
        }
      }
    }
    private void ConfigureSwarmUpdaters(IEnumerable<IRealVectorSwarmUpdater> swarmUpdaters) {
      // swarm updaters don't have additional parameters besides the solution parameter
    }
    private void ConfigureParticleCreators(IEnumerable<IRealVectorParticleCreator> particleCreators) {
      foreach (var particleCreator in particleCreators) {
      }
    }
    private void ConfigureParticleUpdaters(IEnumerable<IRealVectorParticleUpdater> particleUpdaters) {
      // particle updaters don't have additional parameters besides solution and bounds parameter
    }
    private void ConfigureShakingOperators(IEnumerable<IRealVectorMultiNeighborhoodShakingOperator> shakingOperators) {
      // shaking operators don't have additional parameters besides solution and bounds parameter
    }
    private void ConfigureBoundsCheckers(IEnumerable<IRealVectorBoundsChecker> boundsCheckers) {
      foreach (var boundsChecker in boundsCheckers) {
        boundsChecker.RealVectorParameter.ActualName = Name;
        boundsChecker.BoundsParameter.ActualName = BoundsParameter.Name;
      }
    }
    private void ConfigureMoveOperators(IEnumerable<IRealVectorMoveOperator> moveOperators) {
      // move operators don't have additional parameters besides the solution parameter
    }
    private void ConfigureMoveGenerators(IEnumerable<IRealVectorMoveGenerator> moveGenerators) {
      // move generators don't have additional parameters besides solution and bounds parameter
    }
    private void ConfigureAdditiveMoveOperator(IEnumerable<IRealVectorAdditiveMoveOperator> additiveMoveOperators) {
      foreach (var additiveMoveOperator in additiveMoveOperators) {
        additiveMoveOperator.AdditiveMoveParameter.ActualName = Name + ".AdditiveMove";
      }
    }
    private void ConfigureRealVectorSolutionOperators(IEnumerable<IRealVectorSolutionOperator> solutionOperators) {
      foreach (var solutionOperator in solutionOperators)
        solutionOperator.RealVectorParameter.ActualName = Name;
    }
    private void ConfigureRealVectorSolutionsOperators(IEnumerable<IRealVectorSolutionsOperator> solutionsOperators) {
      foreach (var solutionsOperator in solutionsOperators)
        solutionsOperator.RealVectorsParameter.ActualName = Name;
    }
    private void ConfigureRealVectorBoundedOperators(IEnumerable<IRealVectorBoundedOperator> boundedOperators) {
      foreach (var boundedOperator in boundedOperators) {
        boundedOperator.BoundsParameter.ActualName = BoundsParameter.Name;
      }
    }
    #endregion
  }
}