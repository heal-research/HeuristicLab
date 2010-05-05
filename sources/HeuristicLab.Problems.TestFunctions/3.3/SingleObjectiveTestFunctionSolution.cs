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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Drawing;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Common;

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
    private RealVector realVector;
    public RealVector RealVector {
      get { return realVector; }
      set {
        if (realVector != value) {
          if (realVector != null) DeregisterRealVectorEvents();
          realVector = value;
          if (realVector != null) RegisterRealVectorEvents();
          OnRealVectorChanged();
        }
      }
    }

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

    public SingleObjectiveTestFunctionSolution() : base() { }
    public SingleObjectiveTestFunctionSolution(RealVector realVector, DoubleValue quality)
      : base() {
      this.realVector = realVector;
      this.quality = quality;
      Initialize();
    }
    [StorableConstructor]
    private SingleObjectiveTestFunctionSolution(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (realVector != null) RegisterRealVectorEvents();
      if (quality != null) RegisterQualityEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SingleObjectiveTestFunctionSolution clone = new SingleObjectiveTestFunctionSolution();
      cloner.RegisterClonedObject(this, clone);
      clone.realVector = (RealVector)cloner.Clone(realVector);
      clone.quality = (DoubleValue)cloner.Clone(quality);
      clone.Initialize();
      return clone;
    }

    #region Events
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

    private void RegisterRealVectorEvents() {
      RealVector.ItemChanged += new EventHandler<EventArgs<int>>(RealVector_ItemChanged);
      RealVector.Reset += new EventHandler(RealVector_Reset);
    }

    private void DeregisterRealVectorEvents() {
      RealVector.ItemChanged -= new EventHandler<EventArgs<int>>(RealVector_ItemChanged);
      RealVector.Reset -= new EventHandler(RealVector_Reset);
    }
    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
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
    #endregion
  }
}
