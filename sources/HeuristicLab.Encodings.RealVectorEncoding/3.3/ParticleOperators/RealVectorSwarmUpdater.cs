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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
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
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> PersonalBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PersonalBestQuality"]; }
    }
    public ILookupParameter<DoubleValue> NeighborsBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["NeighborsBestQuality"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public ILookupParameter<RealVector> PersonalBestParameter {
      get { return (ILookupParameter<RealVector>)Parameters["PersonalBest"]; }
    }
    public ILookupParameter<RealVector> NeighborsBestParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NeighborsBest"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    #endregion

    #region Parameter values
    private double Quality {
      get { return QualityParameter.ActualValue.Value; }
    }
    private double PersonalBestQuality {
      get { return PersonalBestQualityParameter.ActualValue.Value; }
      set { PersonalBestQualityParameter.ActualValue = new DoubleValue(value); }
    }
    private double NeighborsBestQuality {
      get { return NeighborsBestQualityParameter.ActualValue.Value; }
      set { NeighborsBestQualityParameter.ActualValue = new DoubleValue(value); }
    }
    private RealVector RealVector {
      get { return RealVectorParameter.ActualValue; }
    }
    private RealVector PersonalBest {
      get { return PersonalBestParameter.ActualValue; }
      set { PersonalBestParameter.ActualValue = value; }
    }
    private RealVector NeighborsBest {
      get { return NeighborsBestParameter.ActualValue; }
      set { NeighborsBestParameter.ActualValue = value; }
    }
    private bool Maximization {
      get { return MaximizationParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    private RealVectorSwarmUpdater(bool deserializing) : base(deserializing) { }
    private RealVectorSwarmUpdater(RealVectorSwarmUpdater original, Cloner cloner) : base(original, cloner) { }
    public RealVectorSwarmUpdater()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "Particle's quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("PersonalBestQuality", "Particle's personal best quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("NeighborsBestQuality", "Global best particle quality"));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "Particle's position"));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBest", "Particle's personal best position"));
      Parameters.Add(new LookupParameter<RealVector>("NeighborsBest", "Neighborhood (or global in case of totally connected neighborhood) best particle position"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorSwarmUpdater(this, cloner);
    }

    #endregion

    public override IOperation Apply() {
      if (Maximization && Quality > PersonalBestQuality ||
         !Maximization && Quality < PersonalBestQuality) {
        PersonalBestQuality = Quality;
        PersonalBest = RealVector;
        if (Maximization && PersonalBestQuality > NeighborsBestQuality ||
           !Maximization && PersonalBestQuality < NeighborsBestQuality) {
             NeighborsBestQuality = PersonalBestQuality;
             NeighborsBest = PersonalBest;
        }
      }
      return base.Apply();
    }

  }
}
