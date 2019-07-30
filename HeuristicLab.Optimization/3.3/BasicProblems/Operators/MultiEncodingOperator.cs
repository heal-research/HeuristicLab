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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [StorableType("43619638-9D00-4951-8138-8CCD0786E784")]
  public abstract class MultiEncodingOperator<T> : Operator, IMultiEncodingOperator where T : class,IOperator {
    private List<IEncoding> encodings = new List<IEncoding>();
    [Storable(Name = "Encodings")]
    private IEnumerable<IEncoding> StorableEncodings {
      get { return encodings; }
      set { encodings = new List<IEncoding>(value); }
    }

    public abstract string OperatorPrefix { get; }

    [StorableConstructor]
    protected MultiEncodingOperator(StorableConstructorFlag _) : base(_) { }
    protected MultiEncodingOperator(MultiEncodingOperator<T> original, Cloner cloner)
      : base(original, cloner) {
      encodings = new List<IEncoding>(original.encodings.Select(cloner.Clone));
      foreach (var encoding in encodings)
        encoding.OperatorsChanged += Encoding_OperatorsChanged;
    }
    protected MultiEncodingOperator() : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (var encoding in encodings) {
        // BackwardsCompatibility3.3
        #region Backwards compatible code, remove with 3.4
        if (Parameters.ContainsKey(encoding.Name) && !Parameters.ContainsKey(OperatorPrefix + "." + encoding.Name)) {
          var oldParam = (IConstrainedValueParameter<T>)Parameters[encoding.Name];
          var selected = oldParam.Value;
          Parameters.Remove(oldParam);
          var newParam = new ConstrainedValueParameter<T>(OperatorPrefix + "." + encoding.Name, new ItemSet<T>(oldParam.ValidValues));
          newParam.Value = selected;
          Parameters.Add(newParam);
          oldParam.ValidValues.Clear();
        }
        #endregion
        encoding.OperatorsChanged += Encoding_OperatorsChanged;
      }
    }

    public override IOperation Apply() {
      var operations = Parameters.Select(p => p.ActualValue).OfType<IOperator>().Select(op => ExecutionContext.CreateChildOperation(op));
      return new OperationCollection(operations);
    }

    public virtual void AddEncoding(IEncoding encoding) {
      if (Parameters.ContainsKey(OperatorPrefix + "." + encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was already added.", encoding.Name));

      encodings.Add(encoding);
      encoding.OperatorsChanged += Encoding_OperatorsChanged;

      var param = new ConstrainedValueParameter<T>(OperatorPrefix + "." + encoding.Name, new ItemSet<T>(encoding.Operators.OfType<T>()));
      param.Value = param.ValidValues.First();
      Parameters.Add(param);
    }

    public virtual bool RemoveEncoding(IEncoding encoding) {
      if (!encodings.Remove(encoding)) throw new ArgumentException(string.Format("Encoding {0} was not added to the MultiEncoding.", encoding.Name));
      encoding.OperatorsChanged -= Encoding_OperatorsChanged;
      return Parameters.Remove(OperatorPrefix + "." + encoding.Name);
    }

    protected IConstrainedValueParameter<T> GetParameter(IEncoding encoding) {
      if (!Parameters.ContainsKey(OperatorPrefix + "." + encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was not added to the MultiEncoding.", encoding.Name));

      return (IConstrainedValueParameter<T>)Parameters[OperatorPrefix + "." + encoding.Name];
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
