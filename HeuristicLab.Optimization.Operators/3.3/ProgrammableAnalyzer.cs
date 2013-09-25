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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {

  [Item("ProgrammableAnalyzer", "An analyzer that can be programmed for arbitrary needs.")]
  [StorableClass]
  public class ProgrammableAnalyzer : ProgrammableSingleSuccessorOperator, IAnalyzer {
    public virtual bool EnabledByDefault {
      get { return false; }
    }

    [StorableConstructor]
    protected ProgrammableAnalyzer(bool deserializing) : base(deserializing) { }
    protected ProgrammableAnalyzer(ProgrammableAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public ProgrammableAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the solution should be stored."));
      Parameters.ForEach(x => x.Hidden = false);

      SelectNamespace("HeuristicLab.Optimization");
      SelectNamespace("HeuristicLab.Optimization.Operators");
      Assemblies[typeof(HeuristicLab.Optimization.ResultCollection).Assembly] = true;
      Assemblies[typeof(HeuristicLab.Optimization.Operators.ProgrammableAnalyzer).Assembly] = true;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProgrammableAnalyzer(this, cloner);
    }
  }
}
