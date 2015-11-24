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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class MultiEncodingOperator<T> : InstrumentedOperator, IMultiEncodingOperator where T : class, IOperator {
    [Storable]
    private MultiEncoding encoding;
    public MultiEncoding Encoding {
      get { return encoding; }
      set {
        if (value == null) throw new ArgumentNullException("Encoding must not be null.");
        if (value == encoding) return;
        if (encoding != null) DeregisterEventHandlers();
        encoding = value;
        CombinedSolutionParameter.ActualName = encoding.Name;
        RegisterEventHandlers();
      }
    }

    public ILookupParameter<CombinedSolution> CombinedSolutionParameter {
      get { return (ILookupParameter<CombinedSolution>)Parameters["CombinedSolution"]; }
    }

    [StorableConstructor]
    protected MultiEncodingOperator(bool deserializing) : base(deserializing) { }
    protected MultiEncodingOperator(MultiEncodingOperator<T> original, Cloner cloner)
      : base(original, cloner) {
      encoding = cloner.Clone(original.encoding);
      RegisterEventHandlers();
    }
    protected MultiEncodingOperator()
      : base() {
      Parameters.Add(new LookupParameter<CombinedSolution>("CombinedSolution", "The combined solution that gets created."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      encoding.Encodings.ItemsAdded += EncodingsOnItemsChanged;
      encoding.Encodings.CollectionReset += EncodingsOnItemsChanged;
      encoding.Encodings.ItemsRemoved += EncodingsOnItemsRemoved;
      foreach (var enc in encoding.Encodings)
        enc.OperatorsChanged += Encoding_OperatorsChanged;
    }

    private void DeregisterEventHandlers() {
      encoding.Encodings.ItemsAdded -= EncodingsOnItemsChanged;
      encoding.Encodings.CollectionReset -= EncodingsOnItemsChanged;
      encoding.Encodings.ItemsRemoved -= EncodingsOnItemsRemoved;
      foreach (var enc in encoding.Encodings)
        enc.OperatorsChanged -= Encoding_OperatorsChanged;
    }

    private void EncodingsOnItemsChanged(object sender, CollectionItemsChangedEventArgs<IEncoding> e) {
      foreach (var enc in e.Items)
        AddEncoding(enc);
      foreach (var enc in e.OldItems)
        RemoveEncoding(enc);
    }

    private void EncodingsOnItemsRemoved(object sender, CollectionItemsChangedEventArgs<IEncoding> e) {
      foreach (var enc in e.Items)
        RemoveEncoding(enc);
    }

    public override IOperation InstrumentedApply() {
      var operations = Parameters.Select(p => p.ActualValue).OfType<IOperator>().Select(op => ExecutionContext.CreateOperation(op));
      return new OperationCollection(operations);
    }

    protected virtual void AddEncoding(IEncoding encoding) {
      if (Parameters.ContainsKey(encoding.Name)) throw new ArgumentException(string.Format("Encoding {0} was already added.", encoding.Name));

      encoding.OperatorsChanged += Encoding_OperatorsChanged;

      var param = new ConstrainedValueParameter<T>(encoding.Name, new ItemSet<T>(encoding.Operators.OfType<T>()));
      param.Value = param.ValidValues.First();
      Parameters.Add(param);
    }

    protected virtual bool RemoveEncoding(IEncoding encoding) {
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
