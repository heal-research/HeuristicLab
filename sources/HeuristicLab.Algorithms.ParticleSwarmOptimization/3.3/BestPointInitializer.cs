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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  [Item("Best Point Initializer", "Determines the best quality and point of the current particle population.")]
  [StorableClass]
  public class BestPointInitializer : SingleSuccessorOperator {

    #region Parameter properties
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
    }
    public IScopeTreeLookupParameter<RealVector> PointParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["Point"]; }
    }
    public ILookupParameter<RealVector> BestPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestPoint"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    #endregion

    #region Parameter values
    public ItemArray<DoubleValue> Quality {
      get { return QualityParameter.ActualValue; }
    }
    public double BestQuality {
      get { return BestQualityParameter.ActualValue.Value; }
      set { BestQualityParameter.ActualValue = new DoubleValue(value); }
    }
    public ItemArray<RealVector> Point {
      get { return PointParameter.ActualValue; }
    }
    public RealVector BestPoint {
      get { return BestPointParameter.ActualValue; }
      set { BestPointParameter.ActualValue = value; }
    }
    public bool Maximization {
      get { return MaximizationParameter.ActualValue.Value; }
    }
    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    protected BestPointInitializer(bool deserializing) : base(deserializing) { }
    protected BestPointInitializer(BestPointInitializer original, Cloner cloner)
      : base(original, cloner) {
    }

    public BestPointInitializer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "Particle's quality"));
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "Global best particle quality"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Point", "Particle's position"));
      Parameters.Add(new LookupParameter<RealVector>("BestPoint", "Globa best particle position"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestPointInitializer(this, cloner);
    }

    #endregion

    public override IOperation Apply() {
      BestQuality = Maximization ? Quality.Max(v => v.Value) : Quality.Min(v => v.Value);
      int bestIndex = Quality.FindIndex(v => v.Value == BestQuality);
      BestPoint = (RealVector)Point[bestIndex].Clone();
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
