#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class MultiEncodingOperator<T> : Operator, IMultiEncodingOperator where T : class,IOperator {
    private List<IEncoding> encodings = new List<IEncoding>();
    [Storable(Name = "Encodings")]
    private IEnumerable<IEncoding> StorableEncodings {
      get { return encodings; }
      set { encodings = new List<IEncoding>(value); }
    }

    [StorableConstructor]
    protected MultiEncodingOperator(bool deserializing)
      : base(deserializing) {
    }

    protected MultiEncodingOperator(MultiEncodingOperator<T> original, Cloner cloner)
      : base(original, cloner) {
      encodings = new List<IEncoding>(original.encodings.Select(cloner.Clone));
      foreach (var encoding in encodings)
        encoding.OperatorsChanged += Encoding_OperatorsChanged;
    }

    protected MultiEncodingOperator() : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (var encoding in encodings)
        encoding.OperatorsChanged += Encoding_OperatorsChanged;
    }


    public override IOperation Apply() {
      var operations = Parameters.Select(p => p.ActualValue).OfType<IOperator>().Select(op => ExecutionContext.CreateOperation(op));
      return new OperationCollection(operations);
    }

    public virtual void AddEncoding(IEncoding encoding) {
      if (Parameters.ContainsKey(encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was already added.", encoding.Name));

      encodings.Add(encoding);
      encoding.OperatorsChanged += Encoding_OperatorsChanged;

      var param = new ConstrainedValueParameter<T>(encoding.Name, new ItemSet<T>(encoding.Operators.OfType<T>()));
      param.Value = param.ValidValues.First();
      Parameters.Add(param);
    }

    public virtual bool RemoveEncoding(IEncoding encoding) {
      if (!encodings.Remove(encoding)) throw new ArgumentException(string.Format("Encoding {0} was not added to the MultiEncoding.", encoding.Name));
      encoding.OperatorsChanged -= Encoding_OperatorsChanged;
      return Parameters.Remove(encoding.Name);
    }

    protected IConstrainedValueParameter<T> GetParameter(IEncoding encoding) {
      if (!Parameters.ContainsKey(encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was not added to the MultiEncoding.", encoding.Name));

      return (IConstrainedValueParameter<T>)Parameters[encoding.Name];
    }

    private void Encoding_OperatorsChanged(object sender, EventArgs e) {
      var encoding = (IEncoding)sender;
      var param = GetParameter(encoding);

      var oldParameterValue = param.Value;
      param.ValidValues.Clear();
      foreach (var op in encoding.Operators.OfType<T>())
        param.ValidValues.Add(op);

      var newValue = param.ValidValues.FirstOrDefault(op => op.GetType() == oldParameterValue.GetType());
      if (newValue == null) newValue = param.ValidValues.First();
      param.Value = newValue;
    }
  }
}
