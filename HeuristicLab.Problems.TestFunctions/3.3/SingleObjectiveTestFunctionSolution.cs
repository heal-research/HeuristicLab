#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// Represents a SingleObjectiveTestFunctionSolution solution.
  /// </summary>
  [Item("SingleObjectiveTestFunctionSolution", "Represents a SingleObjectiveTestFunction solution.")]
  [StorableType("F1CC46D2-10F1-4D14-AEF0-A575B67A0955")]
  public class SingleObjectiveTestFunctionSolution : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
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
          if (bestRealVector != null) DeregisterBestRealVectorEvents();
          bestRealVector = value;
          if (bestRealVector != null) RegisterBestRealVectorEvents();
          OnBestRealVectorChanged();
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
    private ISingleObjectiveTestFunction testFunction;
    public ISingleObjectiveTestFunction TestFunction {
      get { return testFunction; }
      set {
        if (testFunction != value) {
          testFunction = value;
          OnEvaluatorChanged();
        }
      }
    }

    [Storable]
    private DoubleMatrix bounds;
    public DoubleMatrix Bounds {
      get { return bounds; }
      set {
        if (bounds != value) {
          if (bounds != null) DeregisterBoundsEvents();
          bounds = value;
          if (bounds != null) RegisterBoundsEvents();
          OnBoundsChanged();
        }
      }
    }

    [StorableConstructor]
    protected SingleObjectiveTestFunctionSolution(StorableConstructorFlag _) : base(_) { }
    protected SingleObjectiveTestFunctionSolution(SingleObjectiveTestFunctionSolution original, Cloner cloner)
      : base(original, cloner) {
      bestKnownRealVector = cloner.Clone(original.bestKnownRealVector);
      bestRealVector = cloner.Clone(original.bestRealVector);
      bestQuality = cloner.Clone(original.bestQuality);
      population = cloner.Clone(original.population);
      testFunction = cloner.Clone(original.testFunction);
      bounds = cloner.Clone(original.bounds);
      Initialize();
    }
    public SingleObjectiveTestFunctionSolution() : base() { }
    public SingleObjectiveTestFunctionSolution(RealVector realVector, DoubleValue quality, ISingleObjectiveTestFunction testFunction)
      : base() {
      this.bestRealVector = realVector;
      this.bestQuality = quality;
      this.testFunction = testFunction;
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      if (bestKnownRealVector != null) RegisterBestKnownRealVectorEvents();
      if (bestRealVector != null) RegisterBestRealVectorEvents();
      if (bestQuality != null) RegisterQualityEvents();
      if (population != null) RegisterPopulationEvents();
      if (bounds != null) RegisterBoundsEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveTestFunctionSolution(this, cloner);
    }

    #region Events
    public event EventHandler BestKnownRealVectorChanged;
    private void OnBestKnownRealVectorChanged() {
      var changed = BestKnownRealVectorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler BestRealVectorChanged;
    private void OnBestRealVectorChanged() {
      var changed = BestRealVectorChanged;
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

    public event EventHandler BoundsChanged;
    private void OnBoundsChanged() {
      var changed = BoundsChanged;
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
    private void RegisterBestRealVectorEvents() {
      BestRealVector.ItemChanged += new EventHandler<EventArgs<int>>(BestRealVector_ItemChanged);
      BestRealVector.Reset += new EventHandler(BestRealVector_Reset);
    }
    private void DeregisterBestRealVectorEvents() {
      BestRealVector.ItemChanged -= new EventHandler<EventArgs<int>>(BestRealVector_ItemChanged);
      BestRealVector.Reset -= new EventHandler(BestRealVector_Reset);
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
    private void RegisterBoundsEvents() {
      Bounds.ItemChanged += new EventHandler<EventArgs<int, int>>(Bounds_ItemChanged);
      Bounds.Reset += new EventHandler(Bounds_Reset);
    }
    private void DeregisterBoundsEvents() {
      Bounds.ItemChanged -= new EventHandler<EventArgs<int, int>>(Bounds_ItemChanged);
      Bounds.Reset -= new EventHandler(Bounds_Reset);
    }

    private void BestKnownRealVector_ItemChanged(object sender, EventArgs<int> e) {
      OnBestKnownRealVectorChanged();
    }
    private void BestKnownRealVector_Reset(object sender, EventArgs e) {
      OnBestKnownRealVectorChanged();
    }
    private void BestRealVector_ItemChanged(object sender, EventArgs<int> e) {
      OnBestRealVectorChanged();
    }
    private void BestRealVector_Reset(object sender, EventArgs e) {
      OnBestRealVectorChanged();
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
    private void Bounds_ItemChanged(object sender, EventArgs<int, int> e) {
      OnBoundsChanged();
    }
    private void Bounds_Reset(object sender, EventArgs e) {
      OnBoundsChanged();
    }
    #endregion
  }
}
