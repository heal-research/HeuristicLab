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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// An algorithm which can be defined by the user.
  /// </summary>
  [Item("User-Defined Algorithm", "An algorithm which can be defined by the user.")]
  [Creatable(CreatableAttribute.Categories.Algorithms, Priority = 100)]
  [StorableType("8D1C0ED9-F732-4DE3-8F6C-125DFF9E8D5E")]
  public sealed class UserDefinedAlgorithm : EngineAlgorithm, IParameterizedItem, IStorableContent {
    public string Filename { get; set; }

    public new ParameterCollection Parameters {
      get { return base.Parameters; }
    }
    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get { return Parameters; }
    }

    public new OperatorGraph OperatorGraph {
      get { return base.OperatorGraph; }
      set { base.OperatorGraph = value; }
    }

    public new IScope GlobalScope {
      get { return base.GlobalScope; }
    }

    private IValueParameter AnalyzerParameter {
      get {
        IParameter param;
        if (Parameters.TryGetValue("Analyzer", out param))
          return param as IValueParameter;
        return null;
      }
    }
    private IMultiAnalyzer Analyzer {
      get {
        if (AnalyzerParameter != null)
          return AnalyzerParameter.Value as IMultiAnalyzer;
        return null;
      }
    }

    public UserDefinedAlgorithm() : base() { }
    public UserDefinedAlgorithm(string name) : base(name) { }
    public UserDefinedAlgorithm(string name, string description) : base(name, description) { }
    [StorableConstructor]
    private UserDefinedAlgorithm(StorableConstructorFlag _) : base(_) { }
    private UserDefinedAlgorithm(UserDefinedAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }
    internal UserDefinedAlgorithm(EngineAlgorithm original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserDefinedAlgorithm(this, cloner);
    }

    #region update of problem specific analyzers
    protected override void OnProblemChanged() {
      AddProblemAnalyzer();
      base.OnProblemChanged();
    }
    protected override void Problem_OperatorsChanged(object sender, System.EventArgs e) {
      RemoveProblemAnalyzers();
      AddProblemAnalyzer();
      base.Problem_OperatorsChanged(sender, e);
    }

    protected override void DeregisterProblemEvents() {
      RemoveProblemAnalyzers();
      base.DeregisterProblemEvents();
    }

    private void RemoveProblemAnalyzers() {
      if (Analyzer != null) {
        foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>())
          Analyzer.Operators.Remove(analyzer);
      }
    }
    private void AddProblemAnalyzer() {
      if (Analyzer != null && Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          Analyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
        }
      }
    }
    #endregion
  }
}
