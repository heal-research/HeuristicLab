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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// Represents a VRP solution which can be visualized in the GUI.
  /// </summary>
  [Item("VRPSolution", "Represents a VRP solution which can be visualized in the GUI.")]
  [StorableType("74CBBEDB-DE6E-4122-AC38-F49DC2B85730")]
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
    private IVRPEncodedSolution solution;
    public IVRPEncodedSolution Solution {
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
    private VRPEvaluation evaluation;
    public VRPEvaluation Evaluation {
      get { return evaluation; }
      set {
        if (evaluation != value) {
          //if (evaluation != null) DeregisterQualityEvents();
          evaluation = value;
          //if (evaluation != null) RegisterQualityEvents();
          OnEvaluationChanged();
        }
      }
    }

    public VRPSolution() : base() { }

    public VRPSolution(IVRPProblemInstance problemInstance, IVRPEncodedSolution solution, VRPEvaluation evaluation)
      : base() {
      this.problemInstance = problemInstance;
      this.solution = solution;
      this.evaluation = evaluation;

      Initialize();
    }
    [StorableConstructor]
    private VRPSolution(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (problemInstance != null) RegisterProblemInstanceEvents();
      if (solution != null) RegisterSolutionEvents();
      // TODO if (evaluation != null) RegisterQualityEvents();
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new VRPSolution(this, cloner);
    }

    private VRPSolution(VRPSolution original, Cloner cloner)
      : base(original, cloner) {
      this.solution = cloner.Clone(original.solution);
      this.evaluation = cloner.Clone(original.evaluation);

      // TODO: this seems very strange
      if (original.ProblemInstance != null && cloner.ClonedObjectRegistered(original.ProblemInstance))
        this.ProblemInstance = cloner.Clone(original.ProblemInstance);
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
    public event EventHandler EvaluationChanged;
    private void OnEvaluationChanged() {
      var changed = EvaluationChanged;
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
    //private void RegisterQualityEvents() {
    //  Quality.ValueChanged += new EventHandler(Quality_ValueChanged);
    //}
    //private void DeregisterQualityEvents() {
    //  Quality.ValueChanged -= new EventHandler(Quality_ValueChanged);
    //}

    private void ProblemInstance_ToStringChanged(object sender, EventArgs e) {
      OnProblemInstanceChanged();
    }
    private void Solution_ToStringChanged(object sender, EventArgs e) {
      OnSolutionChanged();
    }
    //private void Quality_ValueChanged(object sender, EventArgs e) {
    //  OnQualityChanged();
    //}
    #endregion
  }
}
