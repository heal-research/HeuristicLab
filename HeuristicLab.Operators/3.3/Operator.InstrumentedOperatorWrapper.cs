#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using ExecutionContext = HeuristicLab.Core.ExecutionContext;

namespace HeuristicLab.Operators {
  public abstract partial class Operator {
    internal static IOperation CreateInstrumentedOperation(InstrumentedOperator instrumentedOperator) {
      var wrapper = new InstrumentedOperatorWrapper(instrumentedOperator);
      return instrumentedOperator.ExecutionContext.CreateOperation(wrapper);
    }

    [StorableClass]
    [NonDiscoverableType]
    private class InstrumentedOperatorWrapper : Operator {
      [Storable]
      private readonly InstrumentedOperator instrumentedOperator;

      [StorableConstructor]
      private InstrumentedOperatorWrapper(bool deserializing) : base(deserializing) { }

      private InstrumentedOperatorWrapper(InstrumentedOperatorWrapper original, Cloner cloner)
        : base(original, cloner) {
        instrumentedOperator = cloner.Clone(original.instrumentedOperator);
      }

      public override IDeepCloneable Clone(Cloner cloner) {
        return new InstrumentedOperatorWrapper(this, cloner);
      }

      public InstrumentedOperatorWrapper(InstrumentedOperator instrumentedOperator)
        : base() {
        this.instrumentedOperator = instrumentedOperator;
      }

      public override IOperation Apply() {
        throw new NotSupportedException("InstrumentedOperatorWrapper is not executeable.");
      }

      public override IOperation Execute(IExecutionContext context, CancellationToken cancellationToken) {
        try {
          //create new executionContext for the instrumented operation to account for parameter lookup and name translation
          var executionContext = new ExecutionContext(context.Parent, instrumentedOperator, context.Scope);
          instrumentedOperator.ExecutionContext = executionContext;
          instrumentedOperator.cancellationToken = cancellationToken;
          foreach (ILookupParameter param in instrumentedOperator.Parameters.OfType<ILookupParameter>())
            param.ExecutionContext = executionContext;
          IOperation next = instrumentedOperator.InstrumentedApply();
          return next;
        }
        finally {
          foreach (ILookupParameter param in instrumentedOperator.Parameters.OfType<ILookupParameter>())
            param.ExecutionContext = null;
          instrumentedOperator.ExecutionContext = null;
        }
      }
    }
  }
}
