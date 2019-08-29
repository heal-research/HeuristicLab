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
  public abstract class Encoding<TEncodedSolution> : ParameterizedNamedItem, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
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
        if (!value.OfType<ISolutionCreator<TEncodedSolution>>().Any())
          throw new ArgumentException("The provided operators contain no suitable solution creator");
        encodingOperators.Clear();
        foreach (var op in value) encodingOperators.Add(op);

        ISolutionCreator<TEncodedSolution> newSolutionCreator = (ISolutionCreator<TEncodedSolution>)encodingOperators.FirstOrDefault(o => o.GetType() == SolutionCreator.GetType()) ??
                               encodingOperators.OfType<ISolutionCreator<TEncodedSolution>>().First();
        SolutionCreator = newSolutionCreator;
        OnOperatorsChanged();
      }
    }

    public IValueParameter SolutionCreatorParameter {
      get { return (IValueParameter)Parameters[Name + ".SolutionCreator"]; }
    }

    ISolutionCreator IEncoding.SolutionCreator {
      get { return SolutionCreator; }
    }
    public ISolutionCreator<TEncodedSolution> SolutionCreator {
      get { return (ISolutionCreator<TEncodedSolution>)SolutionCreatorParameter.Value; }
      set {
        if (value == null) throw new ArgumentNullException("SolutionCreator must not be null.");
        encodingOperators.Remove(SolutionCreator);
        encodingOperators.Add(value);
        SolutionCreatorParameter.Value = value;
        OnSolutionCreatorChanged();
      }
    }


    [StorableConstructor]
    protected Encoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected Encoding(Encoding<TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      encodingOperators = cloner.Clone(original.encodingOperators);

      RegisterEventHandlers();
    }

    protected Encoding(string name)
      : base(name) {
      Parameters.Add(new ValueParameter<ISolutionCreator<TEncodedSolution>>(name + ".SolutionCreator", "The operator to create a solution."));
      Parameters.Add(new FixedValueParameter<ReadOnlyItemSet<IOperator>>(name + ".Operators", "The operators that the encoding specifies.", encodingOperators.AsReadOnly()) {
        GetsCollected = false
      });

      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      SolutionCreatorParameter.ValueChanged += (o, e) => OnSolutionCreatorChanged();
    }

    protected bool AddOperator(IOperator @operator) {
      return encodingOperators.Add(@operator);
    }

    protected bool RemoveOperator(IOperator @operator) {
      return encodingOperators.Remove(@operator);
    }

    public void ConfigureOperator(IItem @operator) { ConfigureOperators(new[] { @operator }); }
    public abstract void ConfigureOperators(IEnumerable<IItem> operators);

    public event EventHandler SolutionCreatorChanged;
    protected virtual void OnSolutionCreatorChanged() {
      ConfigureOperator(SolutionCreator);
      var handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler OperatorsChanged;
    protected virtual void OnOperatorsChanged() {
      ConfigureOperators(Operators);
      var handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
