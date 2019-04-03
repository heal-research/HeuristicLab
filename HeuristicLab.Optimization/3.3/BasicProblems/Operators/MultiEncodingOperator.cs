#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("43619638-9D00-4951-8138-8CCD0786E784")]
  internal abstract class MultiEncodingOperator<T> : InstrumentedOperator, IEncodingOperator<CombinedSolution>, IMultiEncodingOperator where T : class, IOperator {
    public ILookupParameter<CombinedSolution> SolutionParameter {
      get { return (ILookupParameter<CombinedSolution>)Parameters["Solution"]; }
    }

    public ILookupParameter<IEncoding<CombinedSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<CombinedSolution>>)Parameters["Encoding"]; }
    }

    [StorableConstructor]
    protected MultiEncodingOperator(StorableConstructorFlag _) : base(_) { }
    protected MultiEncodingOperator(MultiEncodingOperator<T> original, Cloner cloner) : base(original, cloner) { }
    protected MultiEncodingOperator()
      : base() {
      Parameters.Add(new LookupParameter<CombinedSolution>("Solution", "The solution that gets created."));
      Parameters.Add(new LookupParameter<IEncoding<CombinedSolution>>("Encoding", "The encoding."));
    }

    public override IOperation InstrumentedApply() {
      var operations = Parameters.Select(p => p.ActualValue).OfType<IOperator>().Select(op => ExecutionContext.CreateOperation(op));
      return new OperationCollection(operations);
    }

    public virtual void AddEncoding(IEncoding encoding) {
      if (Parameters.ContainsKey(encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was already added.", encoding.Name));

      encoding.OperatorsChanged += Encoding_OperatorsChanged;

      var param = new ConstrainedValueParameter<T>(encoding.Name, new ItemSet<T>(encoding.Operators.OfType<T>()));
      param.Value = param.ValidValues.First();
      Parameters.Add(param);
    }

    public virtual bool RemoveEncoding(IEncoding encoding) {
      if (!Parameters.ContainsKey(encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was not added to the MultiEncoding.", encoding.Name));
      encoding.OperatorsChanged -= Encoding_OperatorsChanged;
      return Parameters.Remove(encoding.Name);
    }

    protected IConstrainedValueParameter<T> GetParameter(IEncoding encoding) {
      if (!Parameters.ContainsKey(encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was not added to the MultiEncoding.", encoding.Name));

      return (IConstrainedValueParameter<T>)Parameters[encoding.Name];
    }

    private void Encoding_OperatorsChanged(object sender, EventArgs e) {
      var enc = (IEncoding)sender;
      var param = GetParameter(enc);

      var oldParameterValue = param.Value;
      param.ValidValues.Clear();
      foreach (var op in enc.Operators.OfType<T>())
        param.ValidValues.Add(op);

      var newValue = param.ValidValues.FirstOrDefault(op => op.GetType() == oldParameterValue.GetType());
      if (newValue == null) newValue = param.ValidValues.First();
      param.Value = newValue;
    }
  }
}
