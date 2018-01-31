#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// Represents a VRP solution which can be visualized in the GUI.
  /// </summary>
  [Item("VRPSolution", "Represents a VRP solution which can be visualized in the GUI.")]
  [StorableClass]
  public sealed class VRPSolution : Item {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private IVRPProblemInstance problemInstance;
    public IVRPProblemInstance ProblemInstance {
      get { return problemInstance; }
      set {
        if (problemInstance != value) {
          if (problemInstance != null) DeregisterProblemInstanceEvents();
          problemInstance = value;
          if (problemInstance != null) RegisterProblemInstanceEvents();
          OnProblemInstanceChanged();
        }
      }
    }
    [Storable]
    private IVRPEncoding solution;
    public IVRPEncoding Solution {
      get { return solution; }
      set {
        if (solution != value) {
          if (solution != null) DeregisterSolutionEvents();
          solution = value;
          if (solution != null) RegisterSolutionEvents();
          OnSolutionChanged();
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

    public VRPSolution() : base() { }

    public VRPSolution(IVRPProblemInstance problemInstance, IVRPEncoding solution, DoubleValue quality)
      : base() {
      this.problemInstance = problemInstance;
      this.solution = solution;
      this.quality = quality;

      Initialize();
    }
    [StorableConstructor]
    private VRPSolution(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (problemInstance != null) RegisterProblemInstanceEvents();
      if (solution != null) RegisterSolutionEvents();
      if (quality != null) RegisterQualityEvents();
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPSolution(this, cloner);
    }

    private VRPSolution(VRPSolution original, Cloner cloner)
      : base(original, cloner) {
      this.solution = (IVRPEncoding)cloner.Clone(original.solution);
      this.quality = (DoubleValue)cloner.Clone(original.quality);

      if (original.ProblemInstance != null && cloner.ClonedObjectRegistered(original.ProblemInstance))
        this.ProblemInstance = (IVRPProblemInstance)cloner.Clone(original.ProblemInstance);
      else
        this.ProblemInstance = original.ProblemInstance;

      this.Initialize();
    }

    #region Events
    public event EventHandler ProblemInstanceChanged;
    private void OnProblemInstanceChanged() {
      var changed = ProblemInstanceChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler SolutionChanged;
    private void OnSolutionChanged() {
      var changed = SolutionChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler QualityChanged;
    private void OnQualityChanged() {
      var changed = QualityChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void RegisterProblemInstanceEvents() {
      ProblemInstance.ToStringChanged += new EventHandler(ProblemInstance_ToStringChanged);
    }
    private void DeregisterProblemInstanceEvents() {
      ProblemInstance.ToStringChanged -= new EventHandler(ProblemInstance_ToStringChanged);
    }
    private void RegisterSolutionEvents() {
      Solution.ToStringChanged += new EventHandler(Solution_ToStringChanged);
    }
    private void DeregisterSolutionEvents() {
      Solution.ToStringChanged -= new EventHandler(Solution_ToStringChanged);
    }
    private void RegisterQualityEvents() {
      Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    }
    private void DeregisterQualityEvents() {
      Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    }

    private void ProblemInstance_ToStringChanged(object sender, EventArgs e) {
      OnProblemInstanceChanged();
    }
    private void Solution_ToStringChanged(object sender, EventArgs e) {
      OnSolutionChanged();
    }
    private void Quality_ValueChanged(object sender, EventArgs e) {
      OnQualityChanged();
    }
    #endregion
  }
}
