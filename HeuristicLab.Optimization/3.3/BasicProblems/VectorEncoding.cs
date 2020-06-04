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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("VectorEncoding", "Represents a vector-based encoding.")]
  [StorableType("fed1e058-6e47-4e0f-af0a-ff646a9a778b")]
  public abstract class VectorEncoding<TEncodedSolution> : Encoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    [Storable] public IFixedValueParameter<IntValue> LengthParameter { get; private set; }

    public int Length {
      get { return LengthParameter.Value.Value; }
      set {
        if (Length == value) return;
        if (Length <= 0) throw new ArgumentException("Length must be > 0", "value");
        LengthParameter.Value.Value = value;
      }
    }

    [StorableConstructor]
    protected VectorEncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }
    protected VectorEncoding(VectorEncoding<TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      LengthParameter = cloner.Clone(original.LengthParameter);
      RegisterEventHandlers();
    }

    protected VectorEncoding() : this("Vector", 10) { }
    protected VectorEncoding(string name) : this(name, 10) { }
    protected VectorEncoding(int length) : this("Vector", length) { }
    protected VectorEncoding(string name, int length)
      : base(name) {
      Parameters.Add(LengthParameter = new FixedValueParameter<IntValue>(Name + ".Length", new IntValue(length)));
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      IntValueParameterChangeHandler.Create(LengthParameter, OnLengthChanged);
    }

    public event EventHandler LengthChanged;
    protected virtual void OnLengthChanged() {
      LengthChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
