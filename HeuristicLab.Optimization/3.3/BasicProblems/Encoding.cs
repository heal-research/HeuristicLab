#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Encoding", "Base class for describing different encodings.")]
  [StorableClass]
  public abstract class Encoding<T> : ParameterizedNamedItem, IEncoding
    where T : class,ISolutionCreator {
    public override sealed bool CanChangeName {
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
        if (!value.OfType<T>().Any())
          throw new ArgumentException("The provided operators contain no suitable solution creator");
        encodingOperators.Clear();
        foreach (var op in value) encodingOperators.Add(op);

        T newSolutionCreator = (T)encodingOperators.FirstOrDefault(o => o.GetType() == solutionCreator.GetType()) ??
                               encodingOperators.OfType<T>().First();
        SolutionCreator = newSolutionCreator;
        OnOperatorsChanged();
      }
    }

    ISolutionCreator IEncoding.SolutionCreator {
      get { return SolutionCreator; }
      set {
        if (!(value is T)) throw new ArgumentException(string.Format("Cannot assign the solution creator {0} to the encoding {1}.", value.GetType().GetPrettyName(), GetType().GetPrettyName()));
        SolutionCreator = (T)value;
      }
    }
    [Storable]
    private T solutionCreator;
    public T SolutionCreator {
      get {
        return solutionCreator;
      }
      set {
        if (value == null) throw new ArgumentNullException("SolutionCreator must not be null.");
        if (solutionCreator == value) return;
        encodingOperators.Remove(solutionCreator);
        encodingOperators.Add(value);
        solutionCreator = value;
        OnSolutionCreatorChanged();
      }
    }

    [StorableConstructor]
    protected Encoding(bool deserializing) : base(deserializing) { }
    protected Encoding(Encoding<T> original, Cloner cloner)
      : base(original, cloner) {
      encodingOperators = cloner.Clone(original.encodingOperators);
      solutionCreator = cloner.Clone(original.solutionCreator);
    }
    protected Encoding(string name)
      : base(name) {
      Parameters.Add(new FixedValueParameter<ReadOnlyItemSet<IOperator>>(name + ".Operators", "The operators that the encoding specifies.", encodingOperators.AsReadOnly()));
    }

    public virtual Individual GetIndividual(IScope scope) {
      return new SingleEncodingIndividual(this, scope);
    }

    protected bool AddOperator(IOperator @operator) {
      return encodingOperators.Add(@operator);
    }

    protected bool RemoveOperator(IOperator @operator) {
      return encodingOperators.Remove(@operator);
    }

    public void ConfigureOperator(IOperator @operator) { ConfigureOperators(new[] { @operator }); }
    public abstract void ConfigureOperators(IEnumerable<IOperator> operators);

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
