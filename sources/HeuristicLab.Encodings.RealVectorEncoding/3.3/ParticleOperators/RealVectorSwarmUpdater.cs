#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("RealVectorSwarmUpdater", "Updates personal best point and quality as well as global best point and quality.")]
  [StorableClass]
  public sealed class RealVectorSwarmUpdater : SingleSuccessorOperator, IRealVectorSwarmUpdater, ISingleObjectiveOperator {

    [Storable]
    private ResultsCollector ResultsCollector;

    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameter properties
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> PersonalBestQualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["PersonalBestQuality"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> NeighborBestQualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["NeighborBestQuality"]; }
    }
    public IScopeTreeLookupParameter<RealVector> RealVectorParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public IScopeTreeLookupParameter<RealVector> PersonalBestParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["PersonalBest"]; }
    }
    public IScopeTreeLookupParameter<RealVector> NeighborBestParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["NeighborBest"]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> SwarmBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["SwarmBestQuality"]; }
    }
    public ILookupParameter<RealVector> BestRealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestRealVector"]; }
    }
    public IScopeTreeLookupParameter<IntArray> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntArray>)Parameters["Neighbors"]; }
    }
    public IValueLookupParameter<DoubleMatrix> VelocityBoundsParameter {
      get { return (ValueLookupParameter<DoubleMatrix>)Parameters["VelocityBounds"]; }
    }
    public ILookupParameter<DoubleMatrix> CurrentVelocityBoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["CurrentVelocityBounds"]; }
    }
    public LookupParameter<ResultCollection> ResultsParameter {
      get { return (LookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    #region Velocity Bounds Updating
    public ILookupParameter<DoubleValue> VelocityBoundsScaleParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["VelocityBoundsScale"]; }
    }
    public IConstrainedValueParameter<IDiscreteDoubleValueModifier> VelocityBoundsScalingOperatorParameter {
      get { return (IConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["VelocityBoundsScalingOperator"]; }
    }
    public IValueLookupParameter<DoubleValue> VelocityBoundsStartValueParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["VelocityBoundsStartValue"]; }
    }
    public IValueLookupParameter<DoubleValue> VelocityBoundsEndValueParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["VelocityBoundsEndValue"]; }
    }
    public ILookupParameter<IntValue> VelocityBoundsIndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters["VelocityBoundsIndex"]; }
    }
    public IValueLookupParameter<IntValue> VelocityBoundsStartIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["VelocityBoundsStartIndex"]; }
    }
    public IValueLookupParameter<IntValue> VelocityBoundsEndIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["VelocityBoundsEndIndex"]; }
    }
    #endregion

    #endregion
    
    #region Construction & Cloning

    [StorableConstructor]
    private RealVectorSwarmUpdater(bool deserializing) : base(deserializing) { }
    private RealVectorSwarmUpdater(RealVectorSwarmUpdater original, Cloner cloner)
      : base(original, cloner) {
      ResultsCollector = cloner.Clone(original.ResultsCollector);
    }
    public RealVectorSwarmUpdater()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("SwarmBestQuality", "Swarm's best quality."));
      Parameters.Add(new LookupParameter<RealVector>("BestRealVector", "Global best particle position."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "Particles' qualities."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("PersonalBestQuality", "Particles' personal best qualities."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("NeighborBestQuality", "Best neighbor particles' qualities."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "Particles' positions."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("PersonalBest", "Particles' personal best positions."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("NeighborBest", "Neighborhood (or global in case of totally connected neighborhood) best particle positions."));
      Parameters.Add(new ScopeTreeLookupParameter<IntArray>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("VelocityBounds", "Maximum velocity for each dimension.", new DoubleMatrix(new double[,] { { -1, 1 } })));
      Parameters.Add(new LookupParameter<DoubleMatrix>("CurrentVelocityBounds", "Current value of velocity bounds."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "Results"));

      #region Velocity Bounds Updating
      Parameters.Add(new LookupParameter<DoubleValue>("VelocityBoundsScale", "Scale parameter."));
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("VelocityBoundsScalingOperator", "Modifies the value"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("VelocityBoundsStartValue", "The start value of 'Value'.", new DoubleValue(1)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("VelocityBoundsEndValue", "The end value of 'Value'.", new DoubleValue(1E-10)));
      Parameters.Add(new LookupParameter<IntValue>("VelocityBoundsIndex", "The current index.", "Iterations"));
      Parameters.Add(new ValueLookupParameter<IntValue>("VelocityBoundsStartIndex", "The start index at which to start modifying 'Value'.", new IntValue(0)));
      Parameters.Add(new ValueLookupParameter<IntValue>("VelocityBoundsEndIndex", "The end index by which 'Value' should have reached 'EndValue'.", "MaxIterations"));
      VelocityBoundsStartIndexParameter.Hidden = true;
      VelocityBoundsEndIndexParameter.Hidden = true;
      #endregion

      Initialize();
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorSwarmUpdater(this, cloner);
    }

    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("SwarmBestQuality")) {
        ILookupParameter<DoubleValue> oldBestQualityParameter = Parameters["BestQuality"] as ILookupParameter<DoubleValue>;
        Parameters.Add(new LookupParameter<DoubleValue>("SwarmBestQuality", "Swarm's best quality."));
        if (oldBestQualityParameter.ActualName != oldBestQualityParameter.Name)
          SwarmBestQualityParameter.ActualName = oldBestQualityParameter.ActualName;
        Parameters.Remove("BestQuality");
      }
      RegisterEvents();
    }

    private void RegisterEvents() {
      VelocityBoundsStartValueParameter.ValueChanged += new EventHandler(VelocityBoundsStartValueParameter_ValueChanged);
      VelocityBoundsStartValueParameter.Value.ValueChanged += new EventHandler(VelocityBoundsStartValueParameter_Value_ValueChanged);
    }

    void VelocityBoundsStartValueParameter_Value_ValueChanged(object sender, EventArgs e) {
      UpdateVelocityBoundsParamater();
    }

    void UpdateVelocityBoundsParamater() {
      if (VelocityBoundsParameter.Value == null) {
        VelocityBoundsParameter.Value = new DoubleMatrix(1, 2);
      } else if (VelocityBoundsParameter.Value.Columns != 2) {
        VelocityBoundsParameter.Value = new DoubleMatrix(VelocityBoundsParameter.Value.Rows, 2);
      }
      if (VelocityBoundsStartValueParameter.Value != null) {
        DoubleMatrix matrix = VelocityBoundsParameter.Value;
        for (int i = 0; i < matrix.Rows; i++) {
          matrix[i, 0] = (-1) * VelocityBoundsStartValueParameter.Value.Value;
          matrix[i, 1] = VelocityBoundsStartValueParameter.Value.Value;
        }
      }
    }

    void VelocityBoundsStartValueParameter_ValueChanged(object sender, EventArgs e) {
      if (VelocityBoundsStartValueParameter.Value != null) {
        VelocityBoundsStartValueParameter.Value.ValueChanged += new EventHandler(VelocityBoundsStartValueParameter_Value_ValueChanged);
      }
      UpdateVelocityBoundsParamater();
    }

    private void Initialize() {
      ResultsCollector = new ResultsCollector();
      ResultsCollector.CollectedValues.Add(CurrentVelocityBoundsParameter);
      ResultsCollector.CollectedValues.Add(VelocityBoundsParameter);

      foreach (IDiscreteDoubleValueModifier op in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>()) {
        VelocityBoundsScalingOperatorParameter.ValidValues.Add(op);
        op.ValueParameter.ActualName = VelocityBoundsScaleParameter.Name;
        op.StartValueParameter.ActualName = VelocityBoundsStartValueParameter.Name;
        op.EndValueParameter.ActualName = VelocityBoundsEndValueParameter.Name;
        op.IndexParameter.ActualName = VelocityBoundsIndexParameter.Name;
        op.StartIndexParameter.ActualName = VelocityBoundsStartIndexParameter.Name;
        op.EndIndexParameter.ActualName = VelocityBoundsEndIndexParameter.Name;
      }
      VelocityBoundsScalingOperatorParameter.Value = null;
    }

    public override IOperation Apply() {
      var max = MaximizationParameter.ActualValue.Value;
      var points = RealVectorParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var particles = points.Select((p, i) => new { Particle = p, Index = i })
        .Zip(qualities, (p, q) => Tuple.Create(p.Index, p.Particle, q.Value)).ToList();
      UpdateGlobalBest(max, particles);
      UpdateNeighborBest(max, particles);
      UpdatePersonalBest(max, particles);
      return UpdateVelocityBounds();
    }

    private void UpdateGlobalBest(bool maximization, IList<Tuple<int, RealVector, double>> particles) {
      var best = maximization ? particles.MaxItems(x => x.Item3).First() : particles.MinItems(x => x.Item3).First();
      var bestQuality = SwarmBestQualityParameter.ActualValue;
      if (bestQuality == null) {
        SwarmBestQualityParameter.ActualValue = new DoubleValue(best.Item3);
      } else bestQuality.Value = best.Item3;
      BestRealVectorParameter.ActualValue = (RealVector)best.Item2.Clone();
    }

    private void UpdateNeighborBest(bool maximization, IList<Tuple<int, RealVector, double>> particles) {
      var neighbors = NeighborsParameter.ActualValue;
      if (neighbors.Length > 0) {
        var neighborBest = new ItemArray<RealVector>(neighbors.Length);
        var neighborBestQuality = new ItemArray<DoubleValue>(neighbors.Length);
        for (int n = 0; n < neighbors.Length; n++) {
          var pairs = particles.Where(x => x.Item1 == n || neighbors[n].Contains(x.Item1));
          var bestNeighbor = (maximization ? pairs.MaxItems(p => p.Item3)
                                           : pairs.MinItems(p => p.Item3)).First();
          neighborBest[n] = bestNeighbor.Item2;
          neighborBestQuality[n] = new DoubleValue(bestNeighbor.Item3);
        }
        NeighborBestParameter.ActualValue = neighborBest;
        NeighborBestQualityParameter.ActualValue = neighborBestQuality;
      }
    }

    private void UpdatePersonalBest(bool maximization, IList<Tuple<int, RealVector, double>> particles) {
      var personalBest = PersonalBestParameter.ActualValue;
      var personalBestQuality = PersonalBestQualityParameter.ActualValue;

      if (personalBestQuality.Length == 0) {
        personalBestQuality = new ItemArray<DoubleValue>(particles.Select(x => new DoubleValue(x.Item3)));
        PersonalBestQualityParameter.ActualValue = personalBestQuality;
      }
      foreach (var p in particles) {
        if (maximization && p.Item3 > personalBestQuality[p.Item1].Value ||
          !maximization && p.Item3 < personalBestQuality[p.Item1].Value) {
          personalBestQuality[p.Item1].Value = p.Item3;
          personalBest[p.Item1] = p.Item2;
        }
      }
      PersonalBestParameter.ActualValue = personalBest;
    }

    private IOperation UpdateVelocityBounds() {
      var currentVelocityBounds = CurrentVelocityBoundsParameter.ActualValue;

      if (currentVelocityBounds == null) {
        currentVelocityBounds = (DoubleMatrix)VelocityBoundsParameter.ActualValue.Clone();
        CurrentVelocityBoundsParameter.ActualValue = currentVelocityBounds;
      }
      if (VelocityBoundsScalingOperatorParameter.Value == null)
        return new OperationCollection() {
          ExecutionContext.CreateChildOperation(ResultsCollector),        
          base.Apply()
        };

      var velocityBoundsScale = VelocityBoundsScaleParameter.ActualValue;
      var velocityBoundsStartValue = VelocityBoundsStartValueParameter.ActualValue;

      if (velocityBoundsScale == null && velocityBoundsStartValue != null) {
        velocityBoundsScale = new DoubleValue(velocityBoundsStartValue.Value);
        VelocityBoundsScaleParameter.ActualValue = velocityBoundsScale;
      }
      for (int i = 0; i < currentVelocityBounds.Rows; i++) {
        for (int j = 0; j < currentVelocityBounds.Columns; j++) {
          if (currentVelocityBounds[i, j] >= 0) {
            currentVelocityBounds[i, j] = velocityBoundsScale.Value;
          } else {
            currentVelocityBounds[i, j] = (-1) * velocityBoundsScale.Value;
          }
        }
      }

      return new OperationCollection() {
        ExecutionContext.CreateChildOperation(ResultsCollector),
        ExecutionContext.CreateChildOperation(VelocityBoundsScalingOperatorParameter.Value),
        base.Apply()
      };
    }
  }
}
