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
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Swarm Updater", "Updates personal best point and quality as well as global best point and quality.")]
  [StorableClass]
  public sealed class RealVectorSwarmUpdater : SingleSuccessorOperator, IRealVectorSwarmUpdater {

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
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestQuality"]; }
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
    public OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier> VelocityBoundsScalingOperatorParameter {
      get { return (OptionalConstrainedValueParameter<IDiscreteDoubleValueModifier>)Parameters["VelocityBoundsScalingOperator"]; }
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

    #region Parameter values
    private DoubleValue BestQuality {
      get { return BestQualityParameter.ActualValue; }
      set { BestQualityParameter.ActualValue = value; }
    }
    private RealVector BestRealVector {
      get { return BestRealVectorParameter.ActualValue; }
      set { BestRealVectorParameter.ActualValue = value; }
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
    private DoubleMatrix VelocityBounds {
      get { return VelocityBoundsParameter.ActualValue; }
    }
    private DoubleMatrix CurrentVelocityBounds {
      get { return CurrentVelocityBoundsParameter.ActualValue; }
      set { CurrentVelocityBoundsParameter.ActualValue = value; }
    }
    private DoubleValue VelocityBoundsScale {
      get { return VelocityBoundsScaleParameter.ActualValue; }
      set { VelocityBoundsScaleParameter.ActualValue = value; }
    }
    private DoubleValue VelocityBoundsStartValue {
      get { return VelocityBoundsStartValueParameter.ActualValue; }
    }
    private IDiscreteDoubleValueModifier VelocityBoundsScalingOperator {
      get { return VelocityBoundsScalingOperatorParameter.Value; }
    }
    private ResultCollection Results {
      get { return ResultsParameter.ActualValue; }
    }
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
      Parameters.Add(new LookupParameter<DoubleValue>("BestQuality", "Overall best quality."));
      Parameters.Add(new LookupParameter<RealVector>("BestRealVector", "Global best particle position"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "Particle's quality"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("PersonalBestQuality", "Particle's personal best quality"));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("NeighborBestQuality", "Global best particle quality"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "Particle's position"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("PersonalBest", "Particle's personal best position"));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("NeighborBest", "Neighborhood (or global in case of totally connected neighborhood) best particle position"));
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
      Parameters.Add(new LookupParameter<IntValue>("VelocityBoundsIndex", "The current index.", "CurrentIteration"));
      Parameters.Add(new ValueLookupParameter<IntValue>("VelocityBoundsStartIndex", "The start index at which to start modifying 'Value'.", new IntValue(0)));
      Parameters.Add(new ValueLookupParameter<IntValue>("VelocityBoundsEndIndex", "The end index by which 'Value' should have reached 'EndValue'.", "MaxIterations"));
      VelocityBoundsStartIndexParameter.Hidden = true;
      VelocityBoundsEndIndexParameter.Hidden = true;
      #endregion

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorSwarmUpdater(this, cloner);
    }

    #endregion

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
      UpdateGlobalBest();
      UpdateNeighborBest();
      UpdatePersonalBest();
      return UpdateVelocityBounds();
    }

    private void UpdateGlobalBest() {
      if (BestQuality == null)
        BestQuality = new DoubleValue();
      BestQuality.Value = Maximization ? Quality.Max(v => v.Value) : Quality.Min(v => v.Value);
      BestRealVector = (RealVector)RealVector[Quality.FindIndex(v => v.Value == BestQuality.Value)].Clone();
    }

    private void UpdateNeighborBest() {
      if (Neighbors.Length > 0) {
        var neighborBest = new ItemArray<RealVector>(Neighbors.Length);
        var neighborBestQuality = new ItemArray<DoubleValue>(Neighbors.Length);
        for (int n = 0; n < Neighbors.Length; n++) {
          var pairs = Quality.Zip(RealVector, (q, p) => new { Quality = q, Point = p })
            .Where((p, i) => i == n || Neighbors[n].Contains(i));
          var bestNeighbor = Maximization ?
            pairs.OrderByDescending(p => p.Quality.Value).First() :
            pairs.OrderBy(p => p.Quality.Value).First();
          neighborBest[n] = bestNeighbor.Point;
          neighborBestQuality[n] = bestNeighbor.Quality;
        }
        NeighborBest = neighborBest;
        NeighborBestQuality = neighborBestQuality;
      }
    }

    private void UpdatePersonalBest() {
      if (PersonalBestQuality.Length == 0)
        PersonalBestQuality = (ItemArray<DoubleValue>)Quality.Clone();
      for (int i = 0; i < RealVector.Length; i++) {
        if (Maximization && Quality[i].Value > PersonalBestQuality[i].Value ||
          !Maximization && Quality[i].Value < PersonalBestQuality[i].Value) {
          PersonalBestQuality[i].Value = Quality[i].Value;
          PersonalBest[i] = RealVector[i];
        }
      }
    }



    private IOperation UpdateVelocityBounds() {
      if (CurrentVelocityBounds == null)
        CurrentVelocityBounds = (DoubleMatrix)VelocityBounds.Clone();

      if (VelocityBoundsScalingOperator == null)
        return new OperationCollection() {
          ExecutionContext.CreateChildOperation(ResultsCollector),        
          base.Apply()
        };


      DoubleMatrix matrix = CurrentVelocityBounds;
      if (VelocityBoundsScale == null && VelocityBoundsStartValue != null) {
        VelocityBoundsScale = new DoubleValue(VelocityBoundsStartValue.Value);
      }
      for (int i = 0; i < matrix.Rows; i++) {
        for (int j = 0; j < matrix.Columns; j++) {
          if (matrix[i, j] >= 0) {
            matrix[i, j] = VelocityBoundsScale.Value;
          } else {
            matrix[i, j] = (-1) * VelocityBoundsScale.Value;
          }
        }
      }

      return new OperationCollection() {
        ExecutionContext.CreateChildOperation(ResultsCollector),
        ExecutionContext.CreateChildOperation(VelocityBoundsScalingOperator),        
        base.Apply()
      };
    }
  }
}
