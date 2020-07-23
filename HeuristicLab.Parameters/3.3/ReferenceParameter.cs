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
using System.Drawing;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Parameters {
  [Item("ReferenceParameter", "A base class for reference parameters that forward to other (referenced) parameters.")]
  [StorableType("39CE4123-E41C-4935-90FE-91F6A629178A")]
  public abstract class ReferenceParameter : Parameter, IValueParameter {
    public override Image ItemImage {
      get {
        if (Value != null) return Value.ItemImage;
        else return base.ItemImage;
      }
    }

    public IItem Value {
      get => GetActualValue();
      set => SetActualValue(value);
    }

    [Storable] public IValueParameter ReferencedParameter { get; private set; }

    [Storable(DefaultValue = true)]
    private bool readOnly;
    public bool ReadOnly {
      get { return readOnly; }
      set {
        if (value != readOnly) {
          readOnly = value;
          OnReadOnlyChanged();
        }
      }
    }


    [Storable(DefaultValue = true)]
    private bool getsCollected;
    public bool GetsCollected {
      get { return getsCollected; }
      set {
        if (value != getsCollected) {
          getsCollected = value;
          OnGetsCollectedChanged();
        }
      }
    }

    protected ReferenceParameter(IValueParameter referencedParameter) : this(referencedParameter.Name, referencedParameter) { }
    protected ReferenceParameter(string name, IValueParameter referencedParameter) : this(name, referencedParameter.Description, referencedParameter) { }
    protected ReferenceParameter(string name, string description, IValueParameter referencedParameter) : this(name, description, referencedParameter, referencedParameter.DataType) { }
    protected ReferenceParameter(string name, string description, IValueParameter referencedParameter, Type dataType) : base(name, description, dataType) {
      ReferencedParameter = referencedParameter ?? throw new ArgumentNullException("referencedParameter");
      RegisterEvents();
    }

    [StorableConstructor]
    protected ReferenceParameter(StorableConstructorFlag _) : base(_) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    protected ReferenceParameter(ReferenceParameter original, Cloner cloner) : base(original, cloner) {
      ReferencedParameter = cloner.Clone(original.ReferencedParameter);
      ReadOnly = original.ReadOnly;
      GetsCollected = original.GetsCollected;

      RegisterEvents();
    }

    private void RegisterEvents() {
      ReferencedParameter.ToStringChanged += (o, e) => OnToStringChanged();
      ReferencedParameter.ItemImageChanged += (o, e) => OnItemImageChanged();
    }

    protected override IItem GetActualValue() {
      return ReferencedParameter.ActualValue;
    }
    protected override void SetActualValue(IItem value) {
      ReferencedParameter.ActualValue = value;
    }

    public override string ToString() {
      return Name + ": " + (Value != null ? Value.ToString() : "null");
    }

    // TODO: Remove
    //protected void ChangeReference(IValueParameter newReference, bool retainData) {
    //  IItem oldValue = Value;
    //  var oldRef = ReferencedParameter;
    //  oldRef.ToStringChanged -= (o, e) => OnToStringChanged();
    //  oldRef.ItemImageChanged -= (o, e) => OnItemImageChanged();
    //  if (valueChanged != null) oldRef.ValueChanged -= OnReferencedParameterValueChanged;
    //  newReference.ToStringChanged += (o, e) => OnToStringChanged();
    //  newReference.ItemImageChanged += (o, e) => OnItemImageChanged();
    //  if (valueChanged != null) newReference.ValueChanged += OnReferencedParameterValueChanged;
    //  ReferencedParameter = newReference;
    //  oldRef.Value = (IItem)oldValue.Clone(); // This is problematic, e.g. FixedValueParameter !!! But necessary, otherwise the value holds reference to the old parameter
    //  if (retainData) Value = oldValue;
    //}


    #region event handlers
    // code for forwarding of events adapted from https://stackoverflow.com/questions/1065355/forwarding-events-in-c-sharp
    private EventHandler valueChanged;
    public event EventHandler ValueChanged {
      add { // only subscribe when we have a subscriber ourselves
        bool firstSubscription = valueChanged == null;
        valueChanged += value;
        if (firstSubscription && valueChanged != null) //only subscribe once
          ReferencedParameter.ValueChanged += OnReferencedParameterValueChanged;
      }
      remove { // unsubscribe if we have no more subscribers
        valueChanged -= value;
        if (valueChanged == null) ReferencedParameter.ValueChanged -= OnReferencedParameterValueChanged;
      }
    }
    private void OnReferencedParameterValueChanged(object sender, EventArgs args) {
      valueChanged?.Invoke(this, args); // note "this", not "sender" as sender would be the referenced parameter
      OnItemImageChanged();
      OnToStringChanged();
    }

    public event EventHandler ReadOnlyChanged;
    private void OnReadOnlyChanged() {
      ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler GetsCollectedChanged;
    private void OnGetsCollectedChanged() {
      GetsCollectedChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion
  }


  [Item("ReferenceParameter", "ValueParameter<T> that forwards to another (referenced) ValueParameter<T>).")]
  [StorableType("6DD59BE5-C618-4AD4-90FE-0FAAF15650C3")]
  public sealed class ReferenceParameter<T> : ReferenceParameter, IValueParameter<T>
    where T : class, IItem {

    public new T Value {
      get => ReferencedParameter.Value;
      set => ReferencedParameter.Value = value;
    }

    public new IValueParameter<T> ReferencedParameter { get => (IValueParameter<T>)base.ReferencedParameter; }

    public ReferenceParameter(IValueParameter<T> referencedParameter) : this(referencedParameter.Name, referencedParameter) { }
    public ReferenceParameter(string name, IValueParameter<T> referencedParameter) : this(name, referencedParameter.Description, referencedParameter) { }
    public ReferenceParameter(string name, string description, IValueParameter<T> referencedParameter) : base(name, description, referencedParameter) { }

    [StorableConstructor]
    private ReferenceParameter(StorableConstructorFlag _) : base(_) { }
    private ReferenceParameter(ReferenceParameter<T> original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReferenceParameter<T>(this, cloner);
    }

    // TODO: Remove
    //public void ChangeReference(IValueParameter<T> newParameter, bool retainData) {
    //  base.ChangeReference(newParameter, retainData);
    //}
  }


  [Item("ReferenceParameter", "ValueParameter<T> that forwards to another (referenced) ValueParameter<U>).")]
  [StorableType("83FEA704-6AED-4D76-B25A-B469E0E9187A")]
  public sealed class ReferenceParameter<T, U> : ReferenceParameter, IValueParameter<T>
    where T : class, U
    where U : class, IItem {

    public new T Value {
      get => (T)ReferencedParameter.Value;
      set => ReferencedParameter.Value = value;
    }

    public new IValueParameter<U> ReferencedParameter { get => (IValueParameter<U>)base.ReferencedParameter; }

    public ReferenceParameter(IValueParameter<U> referencedParameter) : this(referencedParameter.Name, referencedParameter) { }
    public ReferenceParameter(string name, IValueParameter<U> referencedParameter) : this(name, referencedParameter.Description, referencedParameter) { }
    public ReferenceParameter(string name, string description, IValueParameter<U> referencedParameter) : base(name, description, referencedParameter, typeof(T)) { }

    [StorableConstructor]
    private ReferenceParameter(StorableConstructorFlag _) : base(_) { }
    private ReferenceParameter(ReferenceParameter<T, U> original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReferenceParameter<T, U>(this, cloner);
    }

    // TODO: Remove
    //public void ChangeReference(IValueParameter<U> newParameter, bool retainData) {
    //  base.ChangeReference(newParameter, retainData);
    //}
  }
}
