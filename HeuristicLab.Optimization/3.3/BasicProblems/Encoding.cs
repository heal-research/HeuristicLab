#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public abstract class Encoding<TSolution> : ParameterizedNamedItem, IEncoding<TSolution>
    where TSolution : class,ISolution {
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
        if (!value.OfType<ISolutionCreator<TSolution>>().Any())
          throw new ArgumentException("The provided operators contain no suitable solution creator");
        encodingOperators.Clear();
        foreach (var op in value) encodingOperators.Add(op);

        ISolutionCreator<TSolution> newSolutionCreator = (ISolutionCreator<TSolution>)encodingOperators.FirstOrDefault(o => o.GetType() == solutionCreator.GetType()) ??
                               encodingOperators.OfType<ISolutionCreator<TSolution>>().First();
        SolutionCreator = newSolutionCreator;
        OnOperatorsChanged();
      }
    }

    ISolutionCreator IEncoding.SolutionCreator {
      get { return solutionCreator; }
    }

    [Storable]
    private ISolutionCreator<TSolution> solutionCreator;
    public ISolutionCreator<TSolution> SolutionCreator {
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
    protected Encoding(Encoding<TSolution> original, Cloner cloner)
      : base(original, cloner) {
      encodingOperators = cloner.Clone(original.encodingOperators);
      solutionCreator = cloner.Clone(original.solutionCreator);
    }
    protected Encoding(string name)
      : base(name) {
      Parameters.Add(new FixedValueParameter<ReadOnlyItemSet<IOperator>>(name + ".Operators", "The operators that the encoding specifies.", encodingOperators.AsReadOnly()));
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
