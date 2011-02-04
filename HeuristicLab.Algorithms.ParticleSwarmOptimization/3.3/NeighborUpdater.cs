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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  [Item("Neighbor Updater", "Determines the best quality and point among the defined list of neigbors for every particle and its individual neighborhood.")]
  [StorableClass]
  public class NeighborUpdater : SingleSuccessorOperator {

    #region Parameters
    public IScopeTreeLookupParameter<RealVector> PointsParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["Point"]; }
    }
    public IScopeTreeLookupParameter<IntegerVector> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntegerVector>)Parameters["Neighbors"]; }
    }
    public IScopeTreeLookupParameter<RealVector> BestNeighborPointParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["BestNeighborPoint"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    #endregion

    #region Parameter Values
    public ItemArray<RealVector> Points {
      get { return PointsParameter.ActualValue; }
    }
    public ItemArray<IntegerVector> Neighbors {
      get { return NeighborsParameter.ActualValue; }
    }
    public ItemArray<RealVector> BestNeighborPoints {
      get { return BestNeighborPointParameter.ActualValue; }
      set { BestNeighborPointParameter.ActualValue = value; }
    }
    public ItemArray<DoubleValue> Qualities {
      get { return QualityParameter.ActualValue; }
    }
    public bool Maximization {
      get { return MaximizationParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning
    public NeighborUpdater() {
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Point", "The position of the particle."));
      Parameters.Add(new ScopeTreeLookupParameter<IntegerVector>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("BestNeighborPoint", "The position of the best neighboring particle."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The list of qualities of all particles."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "Whether the problem is a maximization problem."));
    }
    [StorableConstructor]
    protected NeighborUpdater(bool deserializing) : base(deserializing) { }
    protected NeighborUpdater(NeighborUpdater original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new NeighborUpdater(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      if (Neighbors != null & Neighbors.Length > 0) {
        if (BestNeighborPoints == null || BestNeighborPoints.Length != Neighbors.Length)
          BestNeighborPoints = new ItemArray<RealVector>(Neighbors.Length);
        for (int n = 0; n<Neighbors.Length; n++) {
          var pairs = Qualities.Zip(Points, (q, p) => new { Quality = q, Point = p })
            .Where((p, i) => i == n || Neighbors[n].Contains(i));
          BestNeighborPoints[n] = Maximization ? 
          pairs.OrderByDescending(p => p.Quality.Value).First().Point :
          pairs.OrderBy(p => p.Quality.Value).First().Point;
        }
        BestNeighborPointParameter.ActualValue = BestNeighborPoints;
      }
      return base.Apply();
    }
  }
}
