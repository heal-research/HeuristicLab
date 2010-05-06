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

using System;
using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// Represents a SingleObjectiveTestFunctionSolution solution.
  /// </summary>
  [Item("SingleObjectiveTestFunctionSolution", "Represents a SingleObjectiveTestFunction solution.")]
  [StorableClass]
  public class SingleObjectiveTestFunctionSolution : Item {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Image; }
    }

    [Storable]
    private RealVector bestKnownRealVector;
    public RealVector BestKnownRealVector {
      get { return bestKnownRealVector; }
      set {
        if (bestKnownRealVector != value) {
          if (bestKnownRealVector != null) DeregisterBestKnownRealVectorEvents();
          bestKnownRealVector = value;
          if (bestKnownRealVector != null) RegisterBestKnownRealVectorEvents();
          OnBestKnownRealVectorChanged();
        }
      }
    }

    [Storable]
    private RealVector bestRealVector;
    public RealVector BestRealVector {
      get { return bestRealVector; }
      set {
        if (bestRealVector != value) {
          if (bestRealVector != null) DeregisterRealVectorEvents();
          bestRealVector = value;
          if (bestRealVector != null) RegisterRealVectorEvents();
          OnRealVectorChanged();
        }
      }
    }

    [Storable]
    private DoubleValue bestQuality;
    public DoubleValue BestQuality {
      get { return bestQuality; }
      set {
        if (bestQuality != value) {
          if (bestQuality != null) DeregisterQualityEvents();
          bestQuality = value;
          if (bestQuality != null) RegisterQualityEvents();
          OnQualityChanged();
        }
      }
    }

    [Storable]
    private ItemArray<RealVector> population;
    public ItemArray<RealVector> Population {
      get { return population; }
      set {
        if (population != value) {
          if (population != null) DeregisterPopulationEvents();
          population = value;
          if (population != null) RegisterPopulationEvents();
          OnPopulationChanged();
        }
      }
    }

    [Storable]
    private ISingleObjectiveTestFunctionProblemEvaluator evaluator;
    public ISingleObjectiveTestFunctionProblemEvaluator Evaluator {
      get { return evaluator; }
      set {
        if (evaluator != value) {
          evaluator = value;
          OnEvaluatorChanged();
        }
      }
    }

    private Image fitnessLandscape;
    public Image FitnessLandscape {
      get { return fitnessLandscape; }
      set { fitnessLandscape = value; }
    }

    public SingleObjectiveTestFunctionSolution() : base() { }
    public SingleObjectiveTestFunctionSolution(RealVector realVector, DoubleValue quality, ISingleObjectiveTestFunctionProblemEvaluator evaluator)
      : base() {
      this.bestRealVector = realVector;
      this.bestQuality = quality;
      this.evaluator = evaluator;
      Initialize();
    }
    [StorableConstructor]
    private SingleObjectiveTestFunctionSolution(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (bestKnownRealVector != null) RegisterBestKnownRealVectorEvents();
      if (bestRealVector != null) RegisterRealVectorEvents();
      if (bestQuality != null) RegisterQualityEvents();
      if (population != null) RegisterPopulationEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SingleObjectiveTestFunctionSolution clone = new SingleObjectiveTestFunctionSolution();
      cloner.RegisterClonedObject(this, clone);
      clone.bestKnownRealVector = (RealVector)cloner.Clone(bestKnownRealVector);
      clone.bestRealVector = (RealVector)cloner.Clone(bestRealVector);
      clone.bestQuality = (DoubleValue)cloner.Clone(bestQuality);
      clone.population = (ItemArray<RealVector>)cloner.Clone(population);
      clone.evaluator = (ISingleObjectiveTestFunctionProblemEvaluator)cloner.Clone(evaluator);
      clone.fitnessLandscape = null;
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler BestKnownRealVectorChanged;
    private void OnBestKnownRealVectorChanged() {
      var changed = BestKnownRealVectorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler RealVectorChanged;
    private void OnRealVectorChanged() {
      var changed = RealVectorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler PopulationChanged;
    private void OnPopulationChanged() {
      var changed = PopulationChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      var changed = EvaluatorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterBestKnownRealVectorEvents() {
      BestKnownRealVector.ItemChanged += new EventHandler<EventArgs<int>>(BestKnownRealVector_ItemChanged);
      BestKnownRealVector.Reset += new EventHandler(BestKnownRealVector_Reset);
    }
    private void DeregisterBestKnownRealVectorEvents() {
      BestKnownRealVector.ItemChanged -= new EventHandler<EventArgs<int>>(BestKnownRealVector_ItemChanged);
      BestKnownRealVector.Reset -= new EventHandler(BestKnownRealVector_Reset);
    }
    private void RegisterRealVectorEvents() {
      BestRealVector.ItemChanged += new EventHandler<EventArgs<int>>(RealVector_ItemChanged);
      BestRealVector.Reset += new EventHandler(RealVector_Reset);
    }
    private void DeregisterRealVectorEvents() {
      BestRealVector.ItemChanged -= new EventHandler<EventArgs<int>>(RealVector_ItemChanged);
      BestRealVector.Reset -= new EventHandler(RealVector_Reset);
    }
    private void RegisterQualityEvents() {
      BestQuality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      BestQuality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }
    private void RegisterPopulationEvents() {
      Population.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<RealVector>>(Population_CollectionReset);
      Population.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<RealVector>>(Population_ItemsMoved);
      Population.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<RealVector>>(Population_ItemsReplaced);
    }
    private void DeregisterPopulationEvents() {
      Population.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<RealVector>>(Population_CollectionReset);
      Population.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<RealVector>>(Population_ItemsMoved);
      Population.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<RealVector>>(Population_ItemsReplaced);
    }

    private void BestKnownRealVector_ItemChanged(object sender, EventArgs<int> e) {
      OnBestKnownRealVectorChanged();
    }
    private void BestKnownRealVector_Reset(object sender, EventArgs e) {
      OnBestKnownRealVectorChanged();
    }
    private void RealVector_ItemChanged(object sender, EventArgs<int> e) {
      OnRealVectorChanged();
    }
    private void RealVector_Reset(object sender, EventArgs e) {
      OnRealVectorChanged();
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    private void Population_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<RealVector>> e) {
      OnPopulationChanged();
    }
    private void Population_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<RealVector>> e) {
      OnPopulationChanged();
    }
    private void Population_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<RealVector>> e) {
      OnPopulationChanged();
    }
    #endregion
  }
}
