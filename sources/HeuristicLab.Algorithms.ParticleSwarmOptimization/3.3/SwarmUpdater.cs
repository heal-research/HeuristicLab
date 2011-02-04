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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Swarm Updater", "Updates personal best point and quality as well as global best point and quality.")]
  [StorableClass]
  public sealed class SwarmUpdater : SingleSuccessorOperator {
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
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public ILookupParameter<RealVector> PointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Point"]; }
    }
    public ILookupParameter<RealVector> PersonalBestPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["PersonalBestPoint"]; }
    }
    public ILookupParameter<RealVector> BestPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestPoint"]; }
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
    private double BestQuality {
      get { return BestQualityParameter.ActualValue.Value; }
      set { BestQualityParameter.ActualValue = new DoubleValue(value); }
    }
    private RealVector Point {
      get { return PointParameter.ActualValue; }
    }
    private RealVector PersonalBestPoint {
      get { return PersonalBestPointParameter.ActualValue; }
      set { PersonalBestPointParameter.ActualValue = value; }
    }
    private RealVector BestPoint {
      get { return BestPointParameter.ActualValue; }
      set { BestPointParameter.ActualValue = value; }
    }
    private bool Maximization {
      get { return MaximizationParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    private SwarmUpdater(bool deserializing) : base(deserializing) { }
    private SwarmUpdater(SwarmUpdater original, Cloner cloner) : base(original, cloner) { }
    public SwarmUpdater()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "Particle's quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("PersonalBestQuality", "Particle's personal best quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "Global best particle quality"));
      Parameters.Add(new LookupParameter<RealVector>("Point", "Particle's position"));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBestPoint", "Particle's personal best position"));
      Parameters.Add(new LookupParameter<RealVector>("BestPoint", "Globa best particle position"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SwarmUpdater(this, cloner);
    }

    #endregion

    public override IOperation Apply() {
      if (Maximization && Quality > PersonalBestQuality ||
         !Maximization && Quality < PersonalBestQuality) {
        PersonalBestQuality = Quality;
        PersonalBestPoint = Point;
        if (Maximization && PersonalBestQuality > BestQuality ||
           !Maximization && PersonalBestQuality < BestQuality) {
          BestQuality = PersonalBestQuality;
          BestPoint = PersonalBestPoint;
        }
      }
      return base.Apply();
    }
  }
}
