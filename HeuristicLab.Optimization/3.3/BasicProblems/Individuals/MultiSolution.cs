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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("MultiSolution", "A solution that consists of other solutions.")]
  [StorableClass]
  public sealed class MultiSolution : Item, ISolution {

    private MultiEncoding Encoding { get; set; }
    protected IScope Scope { get; private set; }

    private readonly Dictionary<string, ISolution> solutions;

    [StorableConstructor]
    private MultiSolution(bool deserializing) : base(deserializing) { }

    private MultiSolution(MultiSolution original, Cloner cloner)
      : base(original, cloner) {
      Encoding = cloner.Clone(original.Encoding);
      Scope = cloner.Clone(original.Scope);
      solutions = original.solutions.ToDictionary(x => x.Key, x => cloner.Clone(x.Value));
    }
    public MultiSolution(MultiEncoding encoding, IScope scope) {
      Encoding = encoding;
      Scope = scope;
      solutions = encoding.Encodings.Select(e => new { Name = e.Name, Solution = ScopeUtil.GetSolution(scope, e) })
                                    .ToDictionary(x => x.Name, x => x.Solution);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiSolution(this, cloner);
    }

    public ISolution this[string name] {
      get {
        ISolution result;
        if (!solutions.TryGetValue(name, out result)) throw new ArgumentException(string.Format("{0} is not part of the specified encoding.", name));
        return result;
      }
      set {
        if (!solutions.ContainsKey(name)) throw new ArgumentException(string.Format("{0} is not part of the specified encoding.", name));
        solutions[name] = value;
      }
    }

    public TEncoding GetEncoding<TEncoding>() where TEncoding : IEncoding {
      TEncoding encoding;
      try {
        encoding = (TEncoding)Encoding.Encodings.SingleOrDefault(e => e is TEncoding);
      } catch (InvalidOperationException) {
        throw new InvalidOperationException(string.Format("The solution uses multiple {0} .", typeof(TEncoding).GetPrettyName()));
      }
      if (encoding == null) throw new InvalidOperationException(string.Format("The solution does not use a {0}.", typeof(TEncoding).GetPrettyName()));
      return encoding;
    }

    public TSolution GetSolution<TSolution>() where TSolution : class, ISolution {
      TSolution solution;
      try {
        solution = (TSolution)solutions.SingleOrDefault(s => s.Value is TSolution).Value;
      } catch (InvalidOperationException) {
        throw new InvalidOperationException(string.Format("The solution uses multiple {0} .", typeof(TSolution).GetPrettyName()));
      }
      if (solution == null) throw new InvalidOperationException(string.Format("The solution does not use a {0}.", typeof(TSolution).GetPrettyName()));
      return solution;
    }
  }
}
