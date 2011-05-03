#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  [Item("ClassificationProblemData", "Represents an item containing all data defining a classification problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public class ClassificationProblemData : DataAnalysisProblemData {
    #region default data
    private static string[] defaultInputs = new string[] { "sample", "clump thickness", "cell size", "cell shape", "marginal adhesion", "epithelial cell size", "bare nuclei", "chromatin", "nucleoli", "mitoses", "class" };
    private static double[,] defaultData = new double[,]{
     {1000025,5,1,1,1,2,1,3,1,1,2      },
     {1002945,5,4,4,5,7,10,3,2,1,2     },
     {1015425,3,1,1,1,2,2,3,1,1,2      },
     {1016277,6,8,8,1,3,4,3,7,1,2      },
     {1017023,4,1,1,3,2,1,3,1,1,2      },
     {1017122,8,10,10,8,7,10,9,7,1,4   },
     {1018099,1,1,1,1,2,10,3,1,1,2     },
     {1018561,2,1,2,1,2,1,3,1,1,2      },
     {1033078,2,1,1,1,2,1,1,1,5,2      },
     {1033078,4,2,1,1,2,1,2,1,1,2      },
     {1035283,1,1,1,1,1,1,3,1,1,2      },
     {1036172,2,1,1,1,2,1,2,1,1,2      },
     {1041801,5,3,3,3,2,3,4,4,1,4      },
     {1043999,1,1,1,1,2,3,3,1,1,2      },
     {1044572,8,7,5,10,7,9,5,5,4,4     },
     {1047630,7,4,6,4,6,1,4,3,1,4      },
     {1048672,4,1,1,1,2,1,2,1,1,2      },
     {1049815,4,1,1,1,2,1,3,1,1,2      },
     {1050670,10,7,7,6,4,10,4,1,2,4    },
     {1050718,6,1,1,1,2,1,3,1,1,2      },
     {1054590,7,3,2,10,5,10,5,4,4,4    },
     {1054593,10,5,5,3,6,7,7,10,1,4    },
     {1056784,3,1,1,1,2,1,2,1,1,2      },
     {1057013,8,4,5,1,2,2,7,3,1,4      },
     {1059552,1,1,1,1,2,1,3,1,1,2      },
     {1065726,5,2,3,4,2,7,3,6,1,4      },
     {1066373,3,2,1,1,1,1,2,1,1,2      },
     {1066979,5,1,1,1,2,1,2,1,1,2      },
     {1067444,2,1,1,1,2,1,2,1,1,2      },
     {1070935,1,1,3,1,2,1,1,1,1,2      },
     {1070935,3,1,1,1,1,1,2,1,1,2      },
     {1071760,2,1,1,1,2,1,3,1,1,2      },
     {1072179,10,7,7,3,8,5,7,4,3,4     },
     {1074610,2,1,1,2,2,1,3,1,1,2      },
     {1075123,3,1,2,1,2,1,2,1,1,2      },
     {1079304,2,1,1,1,2,1,2,1,1,2      },
     {1080185,10,10,10,8,6,1,8,9,1,4   },
     {1081791,6,2,1,1,1,1,7,1,1,2      },
     {1084584,5,4,4,9,2,10,5,6,1,4     },
     {1091262,2,5,3,3,6,7,7,5,1,4      },
     {1096800,6,6,6,9,6,4,7,8,1,2      },
     {1099510,10,4,3,1,3,3,6,5,2,4     },
     {1100524,6,10,10,2,8,10,7,3,3,4   },
     {1102573,5,6,5,6,10,1,3,1,1,4     },
     {1103608,10,10,10,4,8,1,8,10,1,4  },
     {1103722,1,1,1,1,2,1,2,1,2,2      },
     {1105257,3,7,7,4,4,9,4,8,1,4      },
     {1105524,1,1,1,1,2,1,2,1,1,2      },
     {1106095,4,1,1,3,2,1,3,1,1,2      },
     {1106829,7,8,7,2,4,8,3,8,2,4      },
     {1108370,9,5,8,1,2,3,2,1,5,4      },
     {1108449,5,3,3,4,2,4,3,4,1,4      },
     {1110102,10,3,6,2,3,5,4,10,2,4    },
     {1110503,5,5,5,8,10,8,7,3,7,4     },
     {1110524,10,5,5,6,8,8,7,1,1,4     },
     {1111249,10,6,6,3,4,5,3,6,1,4     },
     {1112209,8,10,10,1,3,6,3,9,1,4    },
     {1113038,8,2,4,1,5,1,5,4,4,4      },
     {1113483,5,2,3,1,6,10,5,1,1,4     },
     {1113906,9,5,5,2,2,2,5,1,1,4      },
     {1115282,5,3,5,5,3,3,4,10,1,4     },
     {1115293,1,1,1,1,2,2,2,1,1,2      },
     {1116116,9,10,10,1,10,8,3,3,1,4   },
     {1116132,6,3,4,1,5,2,3,9,1,4      },
     {1116192,1,1,1,1,2,1,2,1,1,2      },
     {1116998,10,4,2,1,3,2,4,3,10,4    },
     {1117152,4,1,1,1,2,1,3,1,1,2      },
     {1118039,5,3,4,1,8,10,4,9,1,4     },
     {1120559,8,3,8,3,4,9,8,9,8,4      },
     {1121732,1,1,1,1,2,1,3,2,1,2      },
     {1121919,5,1,3,1,2,1,2,1,1,2      },
     {1123061,6,10,2,8,10,2,7,8,10,4   },
     {1124651,1,3,3,2,2,1,7,2,1,2      },
     {1125035,9,4,5,10,6,10,4,8,1,4    },
     {1126417,10,6,4,1,3,4,3,2,3,4     },
     {1131294,1,1,2,1,2,2,4,2,1,2      },
     {1132347,1,1,4,1,2,1,2,1,1,2      },
     {1133041,5,3,1,2,2,1,2,1,1,2      },
     {1133136,3,1,1,1,2,3,3,1,1,2      },
     {1136142,2,1,1,1,3,1,2,1,1,2      },
     {1137156,2,2,2,1,1,1,7,1,1,2      },
     {1143978,4,1,1,2,2,1,2,1,1,2      },
     {1143978,5,2,1,1,2,1,3,1,1,2      },
     {1147044,3,1,1,1,2,2,7,1,1,2      },
     {1147699,3,5,7,8,8,9,7,10,7,4     },
     {1147748,5,10,6,1,10,4,4,10,10,4  },
     {1148278,3,3,6,4,5,8,4,4,1,4      },
     {1148873,3,6,6,6,5,10,6,8,3,4     },
     {1152331,4,1,1,1,2,1,3,1,1,2      },
     {1155546,2,1,1,2,3,1,2,1,1,2      },
     {1156272,1,1,1,1,2,1,3,1,1,2      },
     {1156948,3,1,1,2,2,1,1,1,1,2      },
     {1157734,4,1,1,1,2,1,3,1,1,2      },
     {1158247,1,1,1,1,2,1,2,1,1,2      },
     {1160476,2,1,1,1,2,1,3,1,1,2      },
     {1164066,1,1,1,1,2,1,3,1,1,2      },
     {1165297,2,1,1,2,2,1,1,1,1,2      },
     {1165790,5,1,1,1,2,1,3,1,1,2      },
     {1165926,9,6,9,2,10,6,2,9,10,4    },
     {1166630,7,5,6,10,5,10,7,9,4,4    },
     {1166654,10,3,5,1,10,5,3,10,2,4   },
     {1167439,2,3,4,4,2,5,2,5,1,4      },
     {1167471,4,1,2,1,2,1,3,1,1,2      },
     {1168359,8,2,3,1,6,3,7,1,1,4      },
     {1168736,10,10,10,10,10,1,8,8,8,4 },
     {1169049,7,3,4,4,3,3,3,2,7,4      },
     {1170419,10,10,10,8,2,10,4,1,1,4  },
     {1170420,1,6,8,10,8,10,5,7,1,4    },
     {1171710,1,1,1,1,2,1,2,3,1,2      },
     {1171710,6,5,4,4,3,9,7,8,3,4      },
     {1171795,1,3,1,2,2,2,5,3,2,2      },
     {1171845,8,6,4,3,5,9,3,1,1,4      },
     {1172152,10,3,3,10,2,10,7,3,3,4   },
     {1173216,10,10,10,3,10,8,8,1,1,4  },
     {1173235,3,3,2,1,2,3,3,1,1,2      },
     {1173347,1,1,1,1,2,5,1,1,1,2      },
     {1173347,8,3,3,1,2,2,3,2,1,2      },
     {1173509,4,5,5,10,4,10,7,5,8,4    },
     {1173514,1,1,1,1,4,3,1,1,1,2      },
     {1173681,3,2,1,1,2,2,3,1,1,2      },
     {1174057,1,1,2,2,2,1,3,1,1,2      },
     {1174057,4,2,1,1,2,2,3,1,1,2      },
     {1174131,10,10,10,2,10,10,5,3,3,4 },
     {1174428,5,3,5,1,8,10,5,3,1,4     },
     {1175937,5,4,6,7,9,7,8,10,1,4     },
     {1176406,1,1,1,1,2,1,2,1,1,2      },
     {1176881,7,5,3,7,4,10,7,5,5,4        }
};
    #endregion

    private const int MaximumClasses = 20;
    private const string ClassNamesParameterName = "ClassNames";
    private const string MisclassificationMatrixParameterName = "MisClassificationMatrix";

    public StringArray ClassNames {
      get { return ClassNamesParameter.Value; }
      protected set { ClassNamesParameter.Value = value; }
    }
    public IValueParameter<StringArray> ClassNamesParameter {
      get { return (IValueParameter<StringArray>)Parameters[ClassNamesParameterName]; }
    }

    public DoubleMatrix MisclassificationMatrix {
      get { return MisclassificationMatrixParameter.Value; }
      protected set { MisclassificationMatrixParameter.Value = value; }
    }
    public IValueParameter<DoubleMatrix> MisclassificationMatrixParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters[MisclassificationMatrixParameterName]; }
    }

    [Storable]
    private List<double> sortedClassValues;
    public IEnumerable<double> SortedClassValues {
      get { return sortedClassValues; }
    }
    public int NumberOfClasses {
      get { return sortedClassValues.Count; }
    }

    [StorableConstructor]
    protected ClassificationProblemData(bool deserializing) : base(deserializing) { }
    protected ClassificationProblemData(ClassificationProblemData original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
      UpdateClassValues();
    }
    public ClassificationProblemData()
      : base(new Dataset(defaultInputs, defaultData), defaultInputs, defaultInputs[defaultInputs.Length - 1], 0, 60, 60, 120) {
      Parameters.Add(new ValueParameter<StringArray>(ClassNamesParameterName, "An array of the names for all class values."));
      Parameters.Add(new ValueParameter<DoubleMatrix>(MisclassificationMatrixParameterName, "A matrix that describles the penalties for misclassifaction between the single classes."));
      sortedClassValues = new List<double>();

      InputVariables.SetItemCheckedState(InputVariables[InputVariables.Count - 1], false);
      RegisterParameterEvents();
      UpdateClassValues();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClassificationProblemData(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public override void ImportFromFile(string fileName) {
      var csvFileParser = new TableFileParser();
      csvFileParser.Parse(fileName);
      suppressEvents = true;
      Name = "Data imported from " + Path.GetFileName(fileName);
      Dataset = new Dataset(csvFileParser.VariableNames, csvFileParser.Values);
      Dataset.Name = Path.GetFileName(fileName);
      var variableNames = Dataset.VariableNames.Select(x => new StringValue(x).AsReadOnly()).ToList();

      var validTargetVariables = from v in variableNames
                                 let DistinctValues = Dataset.Rows > 50 ? Dataset.GetVariableValues(v.Value, 0, 50).Distinct().Count()
                                                                        : Dataset.GetVariableValues(v.Value).Distinct().Count()
                                 where DistinctValues < MaximumClasses
                                 select v;

      if (!validTargetVariables.Any())
        throw new ArgumentException("Import of classification problem data was not successfull, because no target variable was found." +
          " A target variable must have at most " + MaximumClasses + " distinct values to be applicable to classification.");

      ((ConstrainedValueParameter<StringValue>)TargetVariableParameter).ValidValues.Clear();
      foreach (var variableName in validTargetVariables)
        ((ConstrainedValueParameter<StringValue>)TargetVariableParameter).ValidValues.Add(variableName);
      TargetVariable = validTargetVariables.FirstOrDefault();

      InputVariables = new CheckedItemList<StringValue>(variableNames).AsReadOnly();
      if (TargetVariable != null) InputVariables.SetItemCheckedState(TargetVariable, false);
      int middle = (int)(csvFileParser.Rows * 0.5);
      TrainingSamplesEnd = new IntValue(middle);
      TrainingSamplesStart = new IntValue(0);
      TestSamplesEnd = new IntValue(csvFileParser.Rows);
      TestSamplesStart = new IntValue(middle);

      UpdateClassValues();
      suppressEvents = false;
      OnProblemDataChanged(EventArgs.Empty);
    }

    protected override void OnProblemDataChanged(EventArgs e) {
      base.OnProblemDataChanged(e);
      if (!suppressEvents)
        UpdateClassValues();
    }

    private void UpdateClassValues() {
      sortedClassValues = Dataset.GetVariableValues(TargetVariable.Value).Distinct().ToList();
      sortedClassValues.Sort();
      ResetMisclassificationMatrix();
      UpdateClassNames();
    }

    private void UpdateClassNames() {
      if (ClassNames != null) DeregisterParameterValueEvents();
      StringArray array = new StringArray(NumberOfClasses);
      int i = 0;
      foreach (double classValue in SortedClassValues) {
        array[i] = "Class " + classValue;
        i++;
      }
      ClassNames = array;
      UpdateMisclassifciationMatrixHeaders();
      RegisterParameterValueEvents();
    }

    private void RegisterParameterEvents() {
      ClassNamesParameter.ValueChanged += new EventHandler(ClassNamesChanged);
    }
    private void RegisterParameterValueEvents() {
      ClassNames.ItemChanged += new EventHandler<EventArgs<int>>(ClassNamesChanged);
    }
    private void DeregisterParameterValueEvents() {
      ClassNames.ItemChanged -= new EventHandler<EventArgs<int>>(ClassNamesChanged);
    }

    private void ClassNamesChanged(object sender, EventArgs e) {
      UpdateMisclassifciationMatrixHeaders();
    }

    private void ResetMisclassificationMatrix() {
      double[,] matrix = new double[NumberOfClasses, NumberOfClasses];
      for (int i = 0; i < NumberOfClasses; i++) {
        for (int j = 0; j < NumberOfClasses; j++)
          if (i != j) matrix[i, j] = 1;
      }

      if (MisclassificationMatrix == null)
        MisclassificationMatrix = new DoubleMatrix(matrix);
      if (MisclassificationMatrix.Rows != NumberOfClasses)
        MisclassificationMatrix = new DoubleMatrix(matrix);
      if (MisclassificationMatrix.Columns != NumberOfClasses)
        MisclassificationMatrix = new DoubleMatrix(matrix);
    }

    private void UpdateMisclassifciationMatrixHeaders() {
      MisclassificationMatrix.RowNames = ClassNames.Select(name => "Estimated " + name).ToList();
      MisclassificationMatrix.ColumnNames = ClassNames.Select(name => "Actual " + name).ToList();
    }
  }
}
