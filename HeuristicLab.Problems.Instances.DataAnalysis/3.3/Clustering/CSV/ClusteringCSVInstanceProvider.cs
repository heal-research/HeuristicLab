#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class ClusteringCSVInstanceProvider : ClusteringInstanceProvider {
    public override string Name {
      get { return "CSV File"; }
    }
    public override string Description {
      get {
        return "";
      }
    }
    public override Uri WebLink {
      get { return new Uri("http://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/FAQ#DataAnalysisImportFileFormat"); }
    }
    public override string ReferencePublication {
      get { return ""; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new List<IDataDescriptor>();
    }

    public override IClusteringProblemData LoadData(IDataDescriptor descriptor) {
      throw new NotImplementedException();
    }

    public override bool CanImportData {
      get { return true; }
    }
    public override IClusteringProblemData ImportData(string path) {
      var csvFileParser = new TableFileParser();
      csvFileParser.Parse(path, csvFileParser.AreColumnNamesInFirstLine(path));

      Dataset dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);

      // turn of input variables that are constant in the training partition
      var allowedInputVars = new List<string>();
      var trainingIndizes = Enumerable.Range(0, (csvFileParser.Rows * 2) / 3);
      if (trainingIndizes.Count() >= 2) {
        foreach (var variableName in dataset.DoubleVariables) {
          if (dataset.GetDoubleValues(variableName, trainingIndizes).Range() > 0)
            allowedInputVars.Add(variableName);
        }
      } else {
        allowedInputVars.AddRange(dataset.DoubleVariables);
      }

      ClusteringProblemData clusteringData = new ClusteringProblemData(dataset, allowedInputVars);

      int trainingPartEnd = trainingIndizes.Last();
      clusteringData.TrainingPartition.Start = trainingIndizes.First();
      clusteringData.TrainingPartition.End = trainingPartEnd;
      clusteringData.TestPartition.Start = trainingPartEnd;
      clusteringData.TestPartition.End = csvFileParser.Rows;

      clusteringData.Name = Path.GetFileName(path);

      return clusteringData;
    }

    protected override IClusteringProblemData ImportData(string path, DataAnalysisImportType type, TableFileParser csvFileParser) {
      List<IList> values = csvFileParser.Values;
      if (type.Shuffle) {
        values = Shuffle(values);
      }

      Dataset dataset = new Dataset(csvFileParser.VariableNames, values);

      // turn of input variables that are constant in the training partition
      var allowedInputVars = new List<string>();
      int trainingPartEnd = (csvFileParser.Rows * type.TrainingPercentage) / 100;
      var trainingIndizes = Enumerable.Range(0, trainingPartEnd);
      if (trainingIndizes.Count() >= 2) {
        foreach (var variableName in dataset.DoubleVariables) {
          if (dataset.GetDoubleValues(variableName, trainingIndizes).Range() > 0)
            allowedInputVars.Add(variableName);
        }
      } else {
        allowedInputVars.AddRange(dataset.DoubleVariables);
      }

      ClusteringProblemData clusteringData = new ClusteringProblemData(dataset, allowedInputVars);

      clusteringData.TrainingPartition.Start = 0;
      clusteringData.TrainingPartition.End = trainingPartEnd;
      clusteringData.TestPartition.Start = trainingPartEnd;
      clusteringData.TestPartition.End = csvFileParser.Rows;

      clusteringData.Name = Path.GetFileName(path);

      return clusteringData;
    }
  }
}
