#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Collections;

namespace HeuristicLab.Problems.BinPacking {
  [StorableClass]
  public abstract class PackingPlan<D, B, I> : Item
    where D : class, IPackingPosition
    where B : PackingShape<D>
    where I : PackingShape<D>, IPackingItem {

    #region Properties
    public int NrOfBins {
      get {
        if (BinPackings != null)
          return BinPackings.Count;
        else return 0;
      }
    }
    [Storable]
    protected bool StackingConstraints { get; set; }
    [Storable]
    protected bool UseExtremePoints { get; set; }

    [Storable]
    public B BinMeasures { get; private set; }

    [Storable]
    public ObservableList<BinPacking<D, B, I>> BinPackings { get; set; }

    [Storable]
    private DoubleValue quality;
    public DoubleValue Quality {
      get { return quality; }
      set {
        if (quality != value) {
          if (quality != null) DeregisterQualityEvents();
          quality = value;
          if (quality != null) RegisterQualityEvents();
          OnQualityChanged();
        }
      }
    }
    #endregion

    protected PackingPlan(B binMeasures, bool useExtremePoints, bool stackingConstraints)
      : base() {
      BinMeasures = (B)binMeasures.Clone();
      StackingConstraints = stackingConstraints;
      UseExtremePoints = useExtremePoints;
      BinPackings = new ObservableList<BinPacking<D, B, I>>();
    }

    [StorableConstructor]
    protected PackingPlan(bool deserializing) : base(deserializing) { }
    protected PackingPlan(PackingPlan<D, B, I> original, Cloner cloner)
      : base(original, cloner) {
      this.BinPackings = new ObservableList<BinPacking<D, B, I>>(original.BinPackings.Select(p => cloner.Clone(p)));
      UseExtremePoints = original.UseExtremePoints;
      StackingConstraints = original.StackingConstraints;
      BinMeasures = cloner.Clone(original.BinMeasures);
      Quality = cloner.Clone(original.Quality);
    }


    public void UpdateBinPackings() {
      BinPackings.RemoveAll(x => x.Positions.Count == 0);
      BinPackings = new ObservableList<BinPacking<D, B, I>>(BinPackings.OrderByDescending(bp => bp.PackingDensity));
    }

    #region Events
    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }

    public event EventHandler BinPackingsChanged;
    #endregion
  }
}
