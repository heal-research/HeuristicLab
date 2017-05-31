#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.ProtocolBuffers;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ExternalEvaluation.RepetitionsExtension;

namespace HeuristicLab.Problems.ExternalEvaluation.MyExtension {

  [Item("ExtendedExternalEvaluationProblem", "Creates a solution message, and communicates it via the driver to receive a quality message.")]
  [StorableClass]
  public class ExtendedExternalEvaluationProblem : ExternalEvaluationProblem, IStatefulItem {

    private IDictionary<SolutionMessage, QualityMessage> messages;

    [Storable(Name = "Messages")]
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private IEnumerable<KeyValuePair<byte[], byte[]>> Messages_Persistence {
      get {
        return messages.ToDictionary(
          kvp => kvp.Key.ToByteArray(),
          kvp => kvp.Value.ToByteArray());
      }
      set {
        messages = value.ToDictionary(
          kvp => SolutionMessage.ParseFrom(ByteString.CopyFrom(kvp.Key)),
          kvp => QualityMessage.ParseFrom(ByteString.CopyFrom(kvp.Value), GetQualityMessageExtensions()));
      }
    }

    #region Construction & Cloning
    [StorableConstructor]
    private ExtendedExternalEvaluationProblem(bool deserializing) : base(deserializing) { }

    private ExtendedExternalEvaluationProblem(ExtendedExternalEvaluationProblem original, Cloner cloner)
      : base(original, cloner) {
      Messages_Persistence = original.Messages_Persistence;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtendedExternalEvaluationProblem(this, cloner);
    }
    public ExtendedExternalEvaluationProblem()
      : base() {
      messages = new Dictionary<SolutionMessage, QualityMessage>();

      Encoding = new RealVectorEncoding("r", length: 40, min: 0, max: 10);
    }
    #endregion

    public override QualityMessage Evaluate(SolutionMessage solutionMessage) {
      var qualityMessage = base.Evaluate(solutionMessage);
      lock (messages) {
        messages.Add(solutionMessage, qualityMessage);
      }
      return qualityMessage;
    }
    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);

      lock (messages) {
        ExtendedAnalyze(messages.Keys.ToArray(), messages.Values.ToArray(), results, Encoding, random);
        messages.Clear();
      }
    }

    public virtual void ExtendedAnalyze(SolutionMessage[] solutionMessages, QualityMessage[] qualityMessages, ResultCollection results, IEncoding encoding, IRandom random) {
      if (!results.ContainsKey("SolutionsHistory")) {
        var dataTable = new DataTable("SolutionsHistory");
        int solutionLength = ((RealVectorEncoding)encoding).Length;
        for (int i = 0; i < solutionLength; i++)
          dataTable.Rows.Add(new DataRow("a" + i, "Parameter " + i));
        dataTable.Rows.Add(new DataRow("Quality"));
        dataTable.Rows.Add(new DataRow("Repetitions"));
        dataTable.Rows.Add(new DataRow("Variance"));
        dataTable.Rows.Add(new DataRow("CanceledRuns"));
        results.Add(new Result("SolutionsHistory", dataTable));
      }

      var solutions =
        from result in solutionMessages.Zip(qualityMessages, (sm, qm) => new { sm, qm })
        select new {
          inputs = result.sm.GetDoubleArrayVars(0).DataList,
          quality = result.qm.GetExtension(SingleObjectiveQualityMessage.QualityMessage_).Quality,
          repititions = GetExtension(result.qm, RepetitionsResponse.Repetitions, 1),
          variance = GetExtension(result.qm, RepetitionsResponse.Variance, 0),
          canceledRuns = GetExtension(result.qm, RepetitionsResponse.NumFailed, 0)
        };

      var solutionsHistory = (DataTable)results["SolutionsHistory"].Value;

      foreach (var solution in solutions.OrderBy(s => s.quality)) {
        for (int i = 0; i < solution.inputs.Count; i++)
          solutionsHistory.Rows["a" + i].Values.Add(solution.inputs[i]);
        solutionsHistory.Rows["Quality"].Values.Add(solution.quality);
        solutionsHistory.Rows["Repetitions"].Values.Add(solution.repititions);
        solutionsHistory.Rows["Variance"].Values.Add(solution.variance);
        solutionsHistory.Rows["CanceledRuns"].Values.Add(solution.canceledRuns);
      }
    }
    private T GetExtension<T>(QualityMessage msg, GeneratedExtensionBase<T> extension, T @default = default(T)) {
      return msg.HasExtension(extension) ? msg.GetExtension(extension) : @default;
    }

    public override ExtensionRegistry GetQualityMessageExtensions() {
      var extensions = base.GetQualityMessageExtensions();
      RepetitionsQualityMessage.RegisterAllExtensions(extensions);
      return extensions;
    }

    public void InitializeState() {
    }
    public void ClearState() {
      lock (messages)
        messages.Clear();
    }
  }
}
