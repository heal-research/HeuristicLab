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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("ThresholdTerminator", "Base class for all termination criteria which specifies some threshold.")]
  [StorableType("E5D99104-54B2-471D-B27A-07CC737804A6")]
  public abstract class ThresholdTerminator<T> : Terminator where T : class, IItem, IStringConvertibleValue, new() {
    [Storable]
    private IFixedValueParameter<T> thresholdParameter;
    public IFixedValueParameter<T> ThresholdParameter {
      get { return thresholdParameter; }
      set {
        if (value == null) throw new ArgumentNullException("Threshold parameter must not be null.");
        if (value.Value == null) throw new ArgumentNullException("Threshold parameter value must not be null.");
        if (thresholdParameter == value) return;

        if (thresholdParameter != null) Parameters.Remove(thresholdParameter);
        thresholdParameter = value;
        Parameters.Add(thresholdParameter);
        OnThresholdParameterChanged();
      }
    }

    public T Threshold {
      get { return ThresholdParameter.Value; }
    }

    [StorableConstructor]
    protected ThresholdTerminator(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    protected ThresholdTerminator(ThresholdTerminator<T> original, Cloner cloner)
      : base(original, cloner) {
      thresholdParameter = cloner.Clone(original.thresholdParameter);
      Initialize();
    }
    protected ThresholdTerminator()
      : this(new T()) { }
    protected ThresholdTerminator(T threshold)
      : base() {
      if (threshold == null) throw new ArgumentNullException("threshold");
      thresholdParameter = new FixedValueParameter<T>("Threshold", "The limit of the termiation criterion.", threshold);
      Parameters.Add(thresholdParameter);
      Initialize();
    }

    private void Initialize() {
      RegisterThresholdParameterEvents();
    }
    private void RegisterThresholdParameterEvents() {
      Threshold.ValueChanged += new EventHandler(Threshold_ValueChanged);
    }
    private void OnThresholdParameterChanged() {
      RegisterThresholdParameterEvents();
    }

    private void Threshold_ValueChanged(object sender, EventArgs e) {
      OnThresholdChanged();
    }
    protected virtual void OnThresholdChanged() {
      OnToStringChanged();
    }

    public override string ToString() {
      if (ThresholdParameter.Value == null) return Name;
      else return Name + ": " + ThresholdParameter.Value;
    }
  }
}