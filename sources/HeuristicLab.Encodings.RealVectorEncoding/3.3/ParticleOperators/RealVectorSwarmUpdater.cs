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
    public IValueLookupParameter<DoubleValue> MaxVelocityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaxVelocity"]; }
    }
    public ILookupParameter<DoubleValue> CurrentMaxVelocityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentMaxVelocity"]; }
    }
    public LookupParameter<ResultCollection> ResultsParameter {
      get { return (LookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    #region Max Velocity Updating
    public IConstrainedValueParameter<IDiscreteDoubleValueModifier> MaxVelocityScalingOperatorParameter {
      get { return (IConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["MaxVelocityScalingOperator"]; }
    }
    public IValueLookupParameter<DoubleValue> FinalMaxVelocityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["FinalMaxVelocity"]; }
    }
    public ILookupParameter<IntValue> MaxVelocityIndexParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaxVelocityIndex"]; }
    }
    public IValueLookupParameter<IntValue> MaxVelocityStartIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaxVelocityStartIndex"]; }
    }
    public IValueLookupParameter<IntValue> MaxVelocityEndIndexParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaxVelocityEndIndex"]; }
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
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaxVelocity", "Speed limit for each particle.", new DoubleValue(1)));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentMaxVelocity", "Current value of the speed limit."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "Results"));

      #region Velocity Bounds Updating
      Parameters.Add(new OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>("MaxVelocityScalingOperator", "Modifies the value"));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("FinalMaxVelocity", "The value of maximum velocity if PSO has reached maximum iterations.", new DoubleValue(1E-10)));
      Parameters.Add(new LookupParameter<IntValue>("MaxVelocityIndex", "The current index.", "Iterations"));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaxVelocityStartIndex", "The start index at which to start modifying 'Value'.", new IntValue(0)));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaxVelocityEndIndex", "The end index by which 'Value' should have reached 'EndValue'.", "MaxIterations"));
      MaxVelocityStartIndexParameter.Hidden = true;
      MaxVelocityEndIndexParameter.Hidden = true;
      #endregion

      Initialize();
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
    }
    private void Initialize() {
      ResultsCollector = new ResultsCollector();
      ResultsCollector.CollectedValues.Add(CurrentMaxVelocityParameter);
      ResultsCollector.CollectedValues.Add(MaxVelocityParameter);

      foreach (IDiscreteDoubleValueModifier op in ApplicationManager.Manager.GetInstances<IDiscreteDoubleValueModifier>()) {
        MaxVelocityScalingOperatorParameter.ValidValues.Add(op);
        op.ValueParameter.ActualName = CurrentMaxVelocityParameter.Name;
        op.StartValueParameter.ActualName = MaxVelocityParameter.Name;
        op.EndValueParameter.ActualName = FinalMaxVelocityParameter.Name;
        op.IndexParameter.ActualName = MaxVelocityIndexParameter.Name;
        op.StartIndexParameter.ActualName = MaxVelocityStartIndexParameter.Name;
        op.EndIndexParameter.ActualName = MaxVelocityEndIndexParameter.Name;
      }
      MaxVelocityScalingOperatorParameter.Value = null;
    }

    public override IOperation Apply() {
      var max = MaximizationParameter.ActualValue.Value;
      // Update of the personal bests
      var points = RealVectorParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;
      var particles = points.Select((p, i) => new { Particle = p, Index = i })
        .Zip(qualities, (p, q) => Tuple.Create(p.Index, p.Particle, q.Value)).ToList();
      UpdatePersonalBest(max, particles);

      // SPSO: update of the neighbor bests from the personal bests
      var personalBestPoints = PersonalBestParameter.ActualValue;
      var personalBestQualities = PersonalBestQualityParameter.ActualValue;
      particles = personalBestPoints.Select((p, i) => new { Particle = p, Index = i })
        .Zip(personalBestQualities, (p, q) => Tuple.Create(p.Index, p.Particle, q.Value)).ToList();
      UpdateNeighborBest(max, particles);

      var next = new OperationCollection() { base.Apply() };
      next.Insert(0, ExecutionContext.CreateChildOperation(ResultsCollector));
      if (MaxVelocityScalingOperatorParameter.Value != null) {
        next.Insert(0, ExecutionContext.CreateChildOperation(MaxVelocityScalingOperatorParameter.Value));
      } else CurrentMaxVelocityParameter.ActualValue = new DoubleValue(MaxVelocityParameter.ActualValue.Value);
      return next;
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
      } else {
        // Neighbor best = Global best
        var best = maximization ? particles.MaxItems(x => x.Item3).First() : particles.MinItems(x => x.Item3).First();
        NeighborBestParameter.ActualValue = new ItemArray<RealVector>(particles.Select(x => best.Item2));
        NeighborBestQualityParameter.ActualValue = new ItemArray<DoubleValue>(particles.Select(x => new DoubleValue(best.Item3)));
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
          personalBest[p.Item1] = (RealVector)p.Item2.Clone();
        }
      }
      PersonalBestParameter.ActualValue = personalBest;
    }
  }
}
