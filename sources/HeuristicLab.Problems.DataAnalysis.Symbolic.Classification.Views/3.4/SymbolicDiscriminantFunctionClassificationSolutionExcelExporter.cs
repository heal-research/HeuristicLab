using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Views;
using OfficeOpenXml;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification.Views {
  public class SymbolicDiscriminantFunctionClassificationSolutionExcelExporter : SymbolicSolutionExcelExporter {
    protected override void ExportChart(string fileName, ISymbolicDataAnalysisSolution solution, string formula) {
      ExportChart(fileName, solution as ISymbolicClassificationSolution, formula);
    }

    private void ExportChart(string fileName, ISymbolicClassificationSolution solution, string formula) {
      FileInfo newFile = new FileInfo(fileName);
      if (newFile.Exists) {
        newFile.Delete();
        newFile = new FileInfo(fileName);
      }
      var formulaParts = formula.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

      using (ExcelPackage package = new ExcelPackage(newFile)) {
        ExcelWorksheet modelWorksheet = package.Workbook.Worksheets.Add("Model");
        FormatModelSheet(modelWorksheet, solution, formulaParts);

        ExcelWorksheet datasetWorksheet = package.Workbook.Worksheets.Add("Dataset");
        WriteDatasetToExcel(datasetWorksheet, solution.ProblemData);

        ExcelWorksheet inputsWorksheet = package.Workbook.Worksheets.Add("Inputs");
        WriteInputSheet(inputsWorksheet, datasetWorksheet, formulaParts.Skip(2), solution.ProblemData.Dataset);

        ExcelWorksheet estimatedWorksheet = package.Workbook.Worksheets.Add("Estimated Values");
        WriteEstimatedWorksheet(estimatedWorksheet, datasetWorksheet, formulaParts, solution);

        ExcelWorksheet chartsWorksheet = package.Workbook.Worksheets.Add("Charts");
        AddCharts(chartsWorksheet);
        package.Workbook.Properties.Title = "Excel Export";
        package.Workbook.Properties.Author = "HEAL";
        package.Workbook.Properties.Comments = "Excel export of a symbolic data analysis solution from HeuristicLab";

        package.Save();
      }
    }

    protected void WriteEstimatedWorksheet(ExcelWorksheet estimatedWorksheet, ExcelWorksheet datasetWorksheet, string[] formulaParts, ISymbolicDataAnalysisSolution solution) {
      WriteEstimatedWorksheet(estimatedWorksheet, datasetWorksheet, formulaParts, solution as SymbolicDiscriminantFunctionClassificationSolution);
    }

    protected override void FormatModelSheet(ExcelWorksheet modelWorksheet, ISymbolicDataAnalysisSolution solution, IEnumerable<string> formulaParts) {
      FormatModelSheet(modelWorksheet, solution as SymbolicDiscriminantFunctionClassificationSolution, formulaParts);
    }

    private void FormatModelSheet(ExcelWorksheet modelWorksheet, SymbolicDiscriminantFunctionClassificationSolution solution, IEnumerable<string> formulaParts) {
      int row = 1;
      modelWorksheet.Cells[row, 1].Value = "Model";
      modelWorksheet.Cells[row, 2].Value = solution.Name;

      foreach (var part in formulaParts) {
        modelWorksheet.Cells[row, 4].Value = part;
        row++;
      }

      row = 2;
      modelWorksheet.Cells[row, 1].Value = "Model Depth";
      modelWorksheet.Cells[row, 2].Value = solution.Model.SymbolicExpressionTree.Depth;
      row++;

      modelWorksheet.Cells[row, 1].Value = "Model Length";
      modelWorksheet.Cells[row, 2].Value = solution.Model.SymbolicExpressionTree.Length;
      row += 2;

      var thresholds = solution.Model.Thresholds.ToList();
      // skip first (-inf) and last (+inf) thresholds
      for (int i = 0; i < thresholds.Count; ++i) {
        if (double.IsInfinity(thresholds[i]) || double.IsNaN(thresholds[i]))
          continue;
        modelWorksheet.Cells[row, 1].Value = "Threshold " + i;
        modelWorksheet.Cells[row, 2].Value = thresholds[i];
        ++row;
      }
      row++;

      modelWorksheet.Cells[row, 1].Value = "Estimation Limits Lower";
      modelWorksheet.Cells[row, 2].Value = Math.Max(solution.Model.LowerEstimationLimit, -9.99999999999999E+307); // minimal value supported by excel
      modelWorksheet.Names.Add("EstimationLimitLower", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Estimation Limits Upper";
      modelWorksheet.Cells[row, 2].Value = Math.Min(solution.Model.UpperEstimationLimit, 9.99999999999999E+307);  // maximal value supported by excel
      modelWorksheet.Names.Add("EstimationLimitUpper", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row += 2;

      modelWorksheet.Cells[row, 1].Value = "Trainings Partition Start";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TrainingPartition.Start;
      modelWorksheet.Names.Add(TRAININGSTART, modelWorksheet.Cells[row, 2]);
      row++;

      modelWorksheet.Cells[row, 1].Value = "Trainings Partition End";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TrainingPartition.End;
      modelWorksheet.Names.Add(TRAININGEND, modelWorksheet.Cells[row, 2]);
      row++;

      modelWorksheet.Cells[row, 1].Value = "Test Partition Start";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TestPartition.Start;
      modelWorksheet.Names.Add(TESTSTART, modelWorksheet.Cells[row, 2]);
      row++;

      modelWorksheet.Cells[row, 1].Value = "Test Partition End";
      modelWorksheet.Cells[row, 2].Value = solution.ProblemData.TestPartition.End;
      modelWorksheet.Names.Add(TESTEND, modelWorksheet.Cells[row, 2]);
      row += 2;

      string excelTrainingTarget = Indirect("B", true);
      string excelTrainingEstimated = Indirect("C", true);
      string excelTrainingClassValues = Indirect("D", true);
      string excelTrainingMeanError = Indirect("F", true);
      string excelTrainingMSE = Indirect("G", true);

      string excelTestTarget = Indirect("B", false);
      string excelTestEstimated = Indirect("C", false);
      string excelTestClassValues = Indirect("D", false);
      string excelTestMeanError = Indirect("F", false);
      string excelTestMSE = Indirect("G", false);

      modelWorksheet.Cells[row, 1].Value = "Accuracy (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("SUMPRODUCT(({0}={1})*1)/COUNT({0})", excelTrainingClassValues, excelTrainingTarget);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Accuracy (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("SUMPRODUCT(({0}={1})*1)/COUNT({0})", excelTestClassValues, excelTestTarget);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Pearson's R² (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("POWER(PEARSON({0},{1}),2)", excelTrainingTarget, excelTrainingEstimated);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Pearson's R² (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("POWER(PEARSON({0},{1}),2)", excelTestTarget, excelTestEstimated);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean Squared Error (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTrainingMSE);
      modelWorksheet.Names.Add("TrainingMSE", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Mean Squared Error (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTestMSE);
      modelWorksheet.Names.Add("TestMSE", modelWorksheet.Cells[row, 2]);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Normalized Gini Coefficient (training)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTrainingMeanError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells[row, 1].Value = "Normalized Gini Coefficient (test)";
      modelWorksheet.Cells[row, 2].Formula = string.Format("AVERAGE({0})", excelTestMeanError);
      modelWorksheet.Cells[row, 2].Style.Numberformat.Format = "0.000E+00";
      row++;

      modelWorksheet.Cells["A1:B" + row].AutoFitColumns();

      AddModelTreePicture(modelWorksheet, solution.Model);
    }

    private void WriteEstimatedWorksheet(ExcelWorksheet estimatedWorksheet, ExcelWorksheet datasetWorksheet, string[] formulaParts, SymbolicDiscriminantFunctionClassificationSolution solution) {
      string preparedFormula = PrepareFormula(formulaParts);
      int rows = solution.ProblemData.Dataset.Rows;
      estimatedWorksheet.Cells[1, 1].Value = "Id"; // A
      estimatedWorksheet.Cells[1, 2].Value = "Target Variable"; // B
      estimatedWorksheet.Cells[1, 3].Value = "Estimated Values"; // C
      estimatedWorksheet.Cells[1, 4].Value = "Estimated Class Values"; // D
      estimatedWorksheet.Cells[1, 6].Value = "Error"; // E
      estimatedWorksheet.Cells[1, 7].Value = "Squared Error"; // G
      estimatedWorksheet.Cells[1, 9].Value = "Unbounded Estimated Values"; // I
      estimatedWorksheet.Cells[1, 10].Value = "Bounded Estimated Values"; // J

      estimatedWorksheet.Cells[1, 1, 1, 10].AutoFitColumns();

      var thresholds = solution.Model.Thresholds.Where(x => !double.IsInfinity(x)).ToList();
      var thresholdsFormula = GenerateThresholdsFormula(thresholds);

      int targetIndex = solution.ProblemData.Dataset.VariableNames.ToList().FindIndex(x => x.Equals(solution.ProblemData.TargetVariable)) + 1;
      for (int i = 0; i < rows; i++) {
        estimatedWorksheet.Cells[i + 2, 1].Value = i;
        estimatedWorksheet.Cells[i + 2, 2].Formula = datasetWorksheet.Cells[i + 2, targetIndex].FullAddress; // target values
        estimatedWorksheet.Cells[i + 2, 9].Formula = string.Format(preparedFormula, i + 2); // formula (estimated) values
      }
      estimatedWorksheet.Cells["B2:B" + (rows + 1)].Style.Numberformat.Format = "0.000";

      estimatedWorksheet.Cells["C2:C" + (rows + 1)].Formula = "J2";
      estimatedWorksheet.Cells["C2:C" + (rows + 1)].Style.Numberformat.Format = "0.000";

      estimatedWorksheet.Cells["D2:D" + (rows + 1)].Formula = thresholdsFormula;
      estimatedWorksheet.Cells["D2:D" + (rows + 1)].Style.Numberformat.Format = "0.0";

      estimatedWorksheet.Cells["F2:F" + (rows + 1)].Formula = "B2 - D2";
      estimatedWorksheet.Cells["F2:F" + (rows + 1)].Style.Numberformat.Format = "0.0";

      estimatedWorksheet.Cells["G2:G" + (rows + 1)].Formula = "POWER(F2, 2)";
      estimatedWorksheet.Cells["G2:G" + (rows + 1)].Style.Numberformat.Format = "0.000";

      estimatedWorksheet.Cells["I2:I" + (rows + 1)].Style.Numberformat.Format = "0.000";
      estimatedWorksheet.Cells["J2:J" + (rows + 1)].Formula = "IFERROR(IF(I2 > Model!EstimationLimitUpper, Model!EstimationLimitUpper, IF(I2 < Model!EstimationLimitLower, Model!EstimationLimitLower, I2)), AVERAGE(Model!EstimationLimitLower, Model!EstimationLimitUpper))";
      estimatedWorksheet.Cells["J2:J" + (rows + 1)].Style.Numberformat.Format = "0.000";
    }

    // this method assumes that the thresholds list is sorted in ascending order
    private string GenerateThresholdsFormula(List<double> thresholds) {
      if (thresholds.Count == 1) {
        return String.Format("IF(J2 < {0}, 0, 1)", thresholds[0]);
      }
      var sb = new StringBuilder();
      sb.Append(String.Format("IF(J2 < {0}, 0,", thresholds[0]));
      for (int i = 1; i < thresholds.Count; ++i) {
        double v = thresholds[i];
        sb.Append(String.Format("IF(J2 < {0}, {1},", v, i));
      }
      for (int i = 1; i < thresholds.Count; ++i)
        sb.Append(")");
      sb.Append(")");
      return sb.ToString();
    }
  }
}
