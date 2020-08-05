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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("Encoding", "Base class for describing different encodings.")]
  [StorableType("395B1372-FA54-4649-9EBE-5402A0AA9494")]
  public abstract class Encoding : ParameterizedNamedItem, IEncoding {
    public sealed override bool CanChangeName {
      get { return false; }
    }

    private ItemSet<IOperator> encodingOperators = new ItemSet<IOperator>(new TypeEqualityComparer<IOperator>());

    [Storable(Name = "Operators")]
    private IEnumerable<IOperator> StorableOperators {
      get { return encodingOperators; }
      set { encodingOperators = new ItemSet<IOperator>(value, new TypeEqualityComparer<IOperator>()); ; }
    }

    public IEnumerable<IOperator> Operators {
      get { return encodingOperators; }
      set {
        // SolutionCreator is now a parameter of the algorithm, we don't care!
        //if (!value.OfType<ISolutionCreator<TEncodedSolution>>().Any())
        //  throw new ArgumentException("The provided operators contain no suitable solution creator");
        encodingOperators.Clear();
        foreach (var op in value) encodingOperators.Add(op);
        OnOperatorsChanged();
      }
    }


    [StorableConstructor]
    protected Encoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    protected Encoding(Encoding original, Cloner cloner)
      : base(original, cloner) {
      encodingOperators = cloner.Clone(original.encodingOperators);
    }

    protected Encoding(string name)
      : base(name) {
      Parameters.Add(new FixedValueParameter<ReadOnlyItemSet<IOperator>>(name + ".Operators", "The operators that the encoding specifies.", encodingOperators.AsReadOnly()) {
        GetsCollected = false, ReadOnly = true
      });
    }

    protected bool AddOperator(IOperator @operator) {
      return encodingOperators.Add(@operator);
    }

    protected bool RemoveOperator(IOperator @operator) {
      return encodingOperators.Remove(@operator);
    }

    protected void ReplaceOperators(IEnumerable<IOperator> operators) {
      encodingOperators.Replace(operators);
    }

    public void ConfigureOperator(IItem @operator) { ConfigureOperators(new[] { @operator }); }
    public virtual void ConfigureOperators(IEnumerable<IItem> operators) {
      ConfigureSingleObjectiveImprovementOperators(operators.OfType<ISingleObjectiveImprovementOperator>());
      ConfigureSingleObjectivePathRelinker(operators.OfType<ISingleObjectivePathRelinker>());
    }

    protected virtual void ConfigureSingleObjectiveImprovementOperators(IEnumerable<ISingleObjectiveImprovementOperator> operators) {
      foreach (var op in operators) {
        op.SolutionParameter.ActualName = Name;
        op.SolutionParameter.Hidden = true;
      }
    }

    protected virtual void ConfigureSingleObjectivePathRelinker(IEnumerable<ISingleObjectivePathRelinker> operators) {
      foreach (var op in operators.OfType<ISingleObjectivePathRelinker>()) {
        op.ParentsParameter.ActualName = Name;
        op.ParentsParameter.Hidden = true;
      }
    }

    public event EventHandler OperatorsChanged;
    protected virtual void OnOperatorsChanged() {
      ConfigureOperators(Operators);
      var handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
