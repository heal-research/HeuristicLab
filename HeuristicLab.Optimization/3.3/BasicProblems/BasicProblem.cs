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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [StorableType("D877082E-9E77-4CB1-ABDB-35F63878E116")]
  public abstract class BasicProblem<TEncoding, TEvaluator> : HeuristicOptimizationProblem<TEvaluator, ISolutionCreator>, IProblemDefinition, IStorableContent
    where TEncoding : class, IEncoding
    where TEvaluator : class, IEvaluator {

    public string Filename { get; set; }

    protected IValueParameter<TEncoding> EncodingParameter {
      get { return (IValueParameter<TEncoding>)Parameters["Encoding"]; }
    }

    //mkommend necessary for reuse of operators if the encoding changes
    private TEncoding oldEncoding;

    IEncoding IProblemDefinition.Encoding { get { return Encoding; } }
    public TEncoding Encoding {
      get { return EncodingParameter.Value; }
      protected set {
        if (value == null) throw new ArgumentNullException("Encoding must not be null.");
        EncodingParameter.Value = value;
      }
    }

    protected override IEnumerable<IItem> GetOperators() {
      if (Encoding == null) return base.GetOperators();
      return base.GetOperators().Concat(Encoding.Operators);
    }
    public override IEnumerable<IParameterizedItem> ExecutionContextItems {
      get {
        if (Encoding == null) return base.ExecutionContextItems;
        return base.ExecutionContextItems.Concat(new[] { Encoding });
      }
    }

    protected BasicProblem()
      : base() {
      Parameters.Add(new ValueParameter<TEncoding>("Encoding", "Describes the configuration of the encoding, what the variables are called, what type they are and their bounds if any."));
      oldEncoding = Encoding;
      if(Encoding != null) Parameterize();
      RegisterEvents();
    }

    protected BasicProblem(BasicProblem<TEncoding, TEvaluator> original, Cloner cloner)
      : base(original, cloner) {
      oldEncoding = cloner.Clone(original.oldEncoding);
      RegisterEvents();
    }

    [StorableConstructor]
    protected BasicProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      oldEncoding = Encoding;
      RegisterEvents();
    }

    private void RegisterEvents() {
      EncodingParameter.ValueChanged += (o, e) => OnEncodingChanged();
      var multiEncoding = Encoding as MultiEncoding;
      if (multiEncoding != null) multiEncoding.EncodingsChanged += MultiEncodingOnEncodingsChanged;
    }

    protected virtual void OnEncodingChanged() {
      Parameterize();

      OnOperatorsChanged();
      OnReset();
    }

    private void Parameterize() {
      if (oldEncoding != null) {
        AdaptEncodingOperators(oldEncoding, Encoding);
        var oldMultiEncoding = oldEncoding as MultiEncoding;
        if (oldMultiEncoding != null)
          oldMultiEncoding.EncodingsChanged -= MultiEncodingOnEncodingsChanged;
      }
      oldEncoding = Encoding;

      foreach (var op in Operators.OfType<IEncodingOperator>())
        op.EncodingParameter.ActualName = EncodingParameter.Name;

      var solutionCreatorType = Encoding.GetType().BaseType.GetGenericArguments();
      var paramType = typeof(ValueParameter<>).MakeGenericType(solutionCreatorType);
      var solutionCreatorParam = (IParameter)Activator.CreateInstance(paramType, SolutionCreatorParameter.Name, SolutionCreatorParameter.Description,
        Encoding.SolutionCreator);
      Parameters.Remove(SolutionCreatorParameter);
      Parameters.Add(solutionCreatorParam);
      ((IValueParameter)solutionCreatorParam).ValueChanged += SolutionCreatorParameter_ValueChanged;

      var multiEncoding = Encoding as MultiEncoding;
      if (multiEncoding != null) multiEncoding.EncodingsChanged += MultiEncodingOnEncodingsChanged;
    }

    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      Encoding.SolutionCreator = SolutionCreator;
    }

    private static void AdaptEncodingOperators(IEncoding oldEncoding, IEncoding newEncoding) {
      if (oldEncoding.GetType() != newEncoding.GetType()) return;

      if (oldEncoding.GetType() == typeof(MultiEncoding)) {
        var oldMultiEncoding = (MultiEncoding)oldEncoding;
        var newMultiEncoding = (MultiEncoding)newEncoding;
        if (!oldMultiEncoding.Encodings.SequenceEqual(newMultiEncoding.Encodings, new TypeEqualityComparer<IEncoding>())) return;

        var nestedEncodings = oldMultiEncoding.Encodings.Zip(newMultiEncoding.Encodings, (o, n) => new { oldEnc = o, newEnc = n });
        foreach (var multi in nestedEncodings)
          AdaptEncodingOperators(multi.oldEnc, multi.newEnc);
      }

      var comparer = new TypeEqualityComparer<IOperator>();
      var cloner = new Cloner();
      var oldOperators = oldEncoding.Operators;
      var newOperators = newEncoding.Operators;

      cloner.RegisterClonedObject(oldEncoding, newEncoding);
      var operators = oldOperators.Intersect(newOperators, comparer)
                                  .Select(cloner.Clone)
                                  .Union(newOperators, comparer).ToList();

      newEncoding.ConfigureOperators(operators);
      newEncoding.Operators = operators;
    }

    protected virtual void MultiEncodingOnEncodingsChanged(object sender, EventArgs e) {
      OnOperatorsChanged();
    }

    protected override IEnumerable<KeyValuePair<string, IItem>> GetCollectedValues(IValueParameter param) {
      if (param.Value == null) yield break;
      if (param.GetsCollected) {
        if (param == EncodingParameter) // store only the name of the encoding
          yield return new KeyValuePair<string, IItem>(String.Empty, new StringValue(EncodingParameter.Value.Name));
        else yield return new KeyValuePair<string, IItem>(String.Empty, param.Value);
      }
      var parameterizedItem = param.Value as IParameterizedItem;
      if (parameterizedItem != null) {
        var children = new Dictionary<string, IItem>();
        parameterizedItem.CollectParameterValues(children);
        foreach (var child in children) yield return child;
      }
    }
  }
}
