#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ArtificialAnt {
  /// <summary>
  /// Represents a trail of an artificial ant which can be visualized in the GUI.
  /// </summary>
  [Item("AntTrail", "Represents a trail of an artificial ant which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class AntTrail : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }
    [Storable]
    private SymbolicExpressionTree expression;
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return expression; }
      set {
        if (expression != value) {
          //if (expression != null) DeregisterSymbolicExpressionTreeEvents();
          expression = value;
          //if (expression != null) RegisterSymbolicExpressionTreeEvents();
          OnSymbolicExpressionTreeChanged();
        }
      }
    }

    [Storable]
    private BoolMatrix world;
    public BoolMatrix World {
      get { return world; }
      set {
        if (world != value) {
          if (world != null) DeregisterWorldEvents();
          world = value;
          if (world != null) RegisterWorldEvents();
          OnWorldChanged();
        }
      }
    }
    [Storable]
    private IntValue maxTimeSteps;
    public IntValue MaxTimeSteps {
      get { return maxTimeSteps; }
      set {
        if (maxTimeSteps != value) {
          if (maxTimeSteps != value) {
            if (maxTimeSteps != null) DeregisterMaxTimeStepsEvents();
            maxTimeSteps = value;
            if (maxTimeSteps != null) RegisterMaxTimeStepsEvents();
            OnWorldChanged();
          }
        }
      }
    }

    public AntTrail() : base() { }
    public AntTrail(BoolMatrix world, SymbolicExpressionTree expression, IntValue maxTimeSteps)
      : this() {
      this.world = world;
      this.expression = expression;
      this.maxTimeSteps = maxTimeSteps;
      Initialize();
    }

    [StorableConstructor]
    private AntTrail(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private AntTrail(AntTrail original, Cloner cloner)
      : base(original, cloner) {
      expression = cloner.Clone(original.expression);
      world = cloner.Clone(original.world);
      maxTimeSteps = cloner.Clone(original.maxTimeSteps);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AntTrail(this, cloner);
    }

    private void Initialize() {
      //if (expression != null) RegisterSymbolicExpressionTreeEvents();
      if (world != null) RegisterWorldEvents();
      if (maxTimeSteps != null) RegisterMaxTimeStepsEvents();
    }

    #region Events
    public event EventHandler SymbolicExpressionTreeChanged;
    private void OnSymbolicExpressionTreeChanged() {
      var changed = SymbolicExpressionTreeChanged;
      if (changed != null) changed(this, EventArgs.Empty);
    }
    public event EventHandler WorldChanged;
    private void OnWorldChanged() {
      var changed = WorldChanged;
      if (changed != null) changed(this, EventArgs.Empty);
    }
    public event EventHandler MaxTimeStepsChanged;
    private void OnMaxTimeStepsChanged() {
      var changed = MaxTimeStepsChanged;
      if (changed != null) changed(this, EventArgs.Empty);
    }

    //private void RegisterSymbolicExpressionTreeEvents() {
    //  SymbolicExpressionTree.ItemChanged += new EventHandler<EventArgs<int>>(SymbolicExpressionTree_ItemChanged);
    //  SymbolicExpressionTree.Reset += new EventHandler(SymbolicExpressionTree_Reset);
    //}
    //private void DeregisterSymbolicExpressionTreeEvents() {
    //  SymbolicExpressionTree.ItemChanged -= new EventHandler<EventArgs<int>>(SymbolicExpressionTree_ItemChanged);
    //  SymbolicExpressionTree.Reset -= new EventHandler(SymbolicExpressionTree_Reset);
    //}

    private void RegisterWorldEvents() {
      World.ItemChanged += new EventHandler<EventArgs<int, int>>(World_ItemChanged);
      World.Reset += new EventHandler(World_Reset);
    }
    private void DeregisterWorldEvents() {
      World.ItemChanged -= new EventHandler<EventArgs<int, int>>(World_ItemChanged);
      World.Reset -= new EventHandler(World_Reset);
    }
    private void RegisterMaxTimeStepsEvents() {
      MaxTimeSteps.ValueChanged += new EventHandler(MaxTimeSteps_ValueChanged);
    }
    private void DeregisterMaxTimeStepsEvents() {
      MaxTimeSteps.ValueChanged -= new EventHandler(MaxTimeSteps_ValueChanged);
    }

    void MaxTimeSteps_ValueChanged(object sender, EventArgs e) {
      OnMaxTimeStepsChanged();
    }
    private void World_ItemChanged(object sender, EventArgs<int, int> e) {
      OnWorldChanged();
    }
    private void World_Reset(object sender, EventArgs e) {
      OnWorldChanged();
    }
    #endregion
  }
}
