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

using System.Drawing;
using System.IO;
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  [Item("File-Based Linear/Mixed Integer Programming Problem Definition", "File that defines the model for linear/mixed integer programming problem.")]
  [StorableClass]
  public sealed class FileBasedLinearProgrammingProblemDefinition : ParameterizedNamedItem, ILinearProgrammingProblemDefinition {

    [Storable]
    private readonly IFixedValueParameter<FileValue> fileNameParam;

    [Storable]
    private byte[] fileContent;

    public FileBasedLinearProgrammingProblemDefinition() {
      Parameters.Add(fileNameParam = new FixedValueParameter<FileValue>(nameof(FileName), new FileValue()));
      fileNameParam.Value.FileDialogFilter =
        "All Supported Files (*.mps;*.bin;*.prototxt)|*.mps;*.bin;*.prototxt|" +
        "Mathematical Programming System Files (*.mps)|*.mps|" +
        "Google OR-Tools Protocol Buffers Files (*.bin;*.prototxt)|*.bin;*.prototxt|" +
        "All Files (*.*)|*.*";
      fileNameParam.Value.PathChanged += (o, e) => {
        if (File.Exists(FileName)) {
          fileContent = File.ReadAllBytes(FileName);
        }
      };
    }

    private FileBasedLinearProgrammingProblemDefinition(FileBasedLinearProgrammingProblemDefinition original, Cloner cloner)
      : base(original, cloner) {
      fileNameParam = cloner.Clone(original.fileNameParam);
    }

    [StorableConstructor]
    private FileBasedLinearProgrammingProblemDefinition(bool deserializing) : base(deserializing) { }

    public new static Image StaticItemImage => VSImageLibrary.File;

    public string FileName {
      get => fileNameParam.Value.Value;
      set => fileNameParam.Value.Value = value;
    }

    public IFixedValueParameter<FileValue> FileNameParam => fileNameParam;

    public void Analyze(Solver solver, ResultCollection results) {
    }

    public void BuildModel(Solver solver) {
      var fileInfo = new FileInfo(FileName);
      var tempFileName = Path.GetTempFileName();
      File.WriteAllBytes(tempFileName, fileContent);

      var status = (SolverResponseStatus)(fileInfo.Extension == ".mps"
        ? solver.ImportModelFromMpsFormat(tempFileName)
        : solver.ImportModelFromProtoFormat(tempFileName));

      if (status == SolverResponseStatus.Abnormal)
        throw new FileFormatException($"'{FileName}' is not a valid MPS or Google OR-Tools Protocol Buffers file.");

      File.Delete(tempFileName);
    }

    public override IDeepCloneable Clone(Cloner cloner) => new FileBasedLinearProgrammingProblemDefinition(this, cloner);
  }
}
