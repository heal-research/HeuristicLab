#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Swarm Updater", "Updates personal best point and quality as well as global best point and quality.")]
  [StorableClass]
  public sealed class RealVectorSwarmUpdater : SingleSuccessorOperator, IRealVectorSwarmUpdater {
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
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ILookupParameter<RealVector> BestPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestPoint"]; }
    }
    public IScopeTreeLookupParameter<IntArray> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntArray>)Parameters["Neighbors"]; }
    }
    public ValueLookupParameter<IDiscreteDoubleMatrixModifier> VelocityBoundsUpdaterParameter {
      get { return (ValueLookupParameter<IDiscreteDoubleMatrixModifier>)Parameters["VelocityBoundsUpdater"]; }
    }
    public LookupParameter<DoubleMatrix> VelocityBoundsParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["VelocityBounds"]; }
    }
    #endregion

    #region Parameter values
    private DoubleValue BestQuality {
      get { return BestQualityParameter.ActualValue; }
    }
    private RealVector BestPoint {
      get { return BestPointParameter.ActualValue; }
      set { BestPointParameter.ActualValue = value; }
    }
    private ItemArray<DoubleValue> Quality {
      get { return QualityParameter.ActualValue; }
    }
    private ItemArray<DoubleValue> PersonalBestQuality {
      get { return PersonalBestQualityParameter.ActualValue; }
      set { PersonalBestQualityParameter.ActualValue = value; }
    }
    private ItemArray<DoubleValue> NeighborBestQuality {
      get { return NeighborBestQualityParameter.ActualValue; }
      set { NeighborBestQualityParameter.ActualValue = value; }
    }
    private ItemArray<RealVector> RealVector {
      get { return RealVectorParameter.ActualValue; }
    }
    private ItemArray<RealVector> PersonalBest {
      get { return PersonalBestParameter.ActualValue; }
      set { PersonalBestParameter.ActualValue = value; }
    }
    private ItemArray<RealVector> NeighborBest {
      get { return NeighborBestParameter.ActualValue; }
      set { NeighborBestParameter.ActualValue = value; }
    }
    private bool Maximization {
      get { return MaximizationParameter.ActualValue.Value; }
    }
    private ItemArray<IntArray> Neighbors {
      get { return NeighborsParameter.ActualValue; }
    }
    private IDiscreteDoubleMatrixModifier VelocityBoundsUpdater {
      get { return VelocityBoundsUpdaterParameter.ActualValue; }
    }
    private DoubleMatrix VelocityBounds {
      get { return VelocityBoundsParameter.ActualValue; }
      set { VelocityBoundsParameter.ActualValue = value; }
    }
    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    private RealVectorSwarmUpdater(bool deserializing) : base(deserializing) { }
    private RealVectorSwarmUpdater(RealVectorSwarmUpdater original, Cloner cloner) : base(original, cloner) { }
    public RealVectorSwarmUpdater()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "Overall best quality."));
      Parameters.Add(new LookupParameter<RealVector>("BestPoint", "Global best particle position"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "Particle's quality"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("PersonalBestQuality", "Particle's personal best quality"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("NeighborBestQuality", "Global best particle quality"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "Particle's position"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("PersonalBest", "Particle's personal best position"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("NeighborBest", "Neighborhood (or global in case of totally connected neighborhood) best particle position"));
      Parameters.Add(new ScopeTreeLookupParameter<IntArray>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ValueLookupParameter<IDiscreteDoubleMatrixModifier>("VelocityBoundsUpdater", "Modifies the velocity bounds in the course of optimization."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("VelocityBounds", "Maximum velocity for each dimension."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorSwarmUpdater(this, cloner);
    }

    #endregion

    public override IOperation Apply() {
      InitializeBestPoint();
      UpdateNeighbors();
      UpdateSwarm();
      if (VelocityBoundsUpdater != null) {
        var ops = new OperationCollection();
        ops.Add(ExecutionContext.CreateChildOperation(VelocityBoundsUpdater));
        ops.Add(ExecutionContext.CreateOperation(Successor));
        return ops;
      } else {
        return base.Apply();
      }
    }

    private void InitializeBestPoint() {
      if (BestQuality == null)
        BestQualityParameter.ActualValue = new DoubleValue();
      BestQuality.Value = Maximization ? Quality.Max(v => v.Value) : Quality.Min(v => v.Value);
      int bestIndex = Quality.FindIndex(v => v.Value == BestQuality.Value);
      BestPoint = (RealVector)RealVector[bestIndex].Clone();
    }

    private void UpdateNeighbors() {
      if (Neighbors != null & Neighbors.Length > 0) {
        if (this.NeighborBest == null || NeighborBest.Length != Neighbors.Length)
          NeighborBest = new ItemArray<RealVector>(Neighbors.Length);
        for (int n = 0; n < Neighbors.Length; n++) {
          var pairs = Quality.Zip(RealVector, (q, p) => new { Quality = q, Point = p })
            .Where((p, i) => i == n || Neighbors[n].Contains(i));
          NeighborBest[n] = Maximization ?
          pairs.OrderByDescending(p => p.Quality.Value).First().Point :
          pairs.OrderBy(p => p.Quality.Value).First().Point;
        }
        NeighborBestParameter.ActualValue = NeighborBest;
      }
    }

    private void UpdateSwarm() {
      if (PersonalBestQuality.Length == 0) {
        PersonalBestQuality = (ItemArray<DoubleValue>)Quality.Clone();
        if (VelocityBounds == null)
          VelocityBounds = new DoubleMatrix(new double[,] { { -1, 1 } });
      }
      for (int i = 0; i < RealVector.Length; i++) {
        if (Maximization && Quality[i].Value > PersonalBestQuality[i].Value ||
          !Maximization && Quality[i].Value < PersonalBestQuality[i].Value) {
          PersonalBestQuality[i].Value = Quality[i].Value;
          PersonalBest[i] = RealVector[i];
        }
      }
      if (Neighbors.Length > 0) {
        var neighborBestQuality = NeighborBestQuality;
        var neighborBest = NeighborBest;
        if (NeighborBestQuality.Length == 0) {
          neighborBestQuality = (ItemArray<DoubleValue>)Quality.Clone();
          neighborBest = (ItemArray<RealVector>)RealVector.Clone();
        }
        for (int i = 0; i < RealVector.Length; i++) {
          if (Maximization && PersonalBestQuality[i].Value > NeighborBestQuality[i].Value ||
             !Maximization && PersonalBestQuality[i].Value < NeighborBestQuality[i].Value) {
            neighborBestQuality[i].Value = PersonalBestQuality[i].Value;
            neighborBest[i] = PersonalBest[i];
          }
        }
        NeighborBestQuality = neighborBestQuality;
        NeighborBest = neighborBest;
      }
    }
  }
}
