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

using System;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class TransformationToSymbolicTreeMapper : ITransformationMapper<ISymbolicExpressionTreeNode> {
    private ITransformation transformation;
    private string column;

    #region ITransformationMapper<ISymbolicExpressionTree> Members

    public ISymbolicExpressionTreeNode GenerateModel(ITransformation transformation) {
      InitComponents(transformation);

      if (transformation is LinearTransformation) {
        return GenerateModelForLinearTransformation();
      } else if (transformation is ExponentialTransformation) {
        return GenerateModelForExponentialTransformation();
      } else if (transformation is LogarithmicTransformation) {
        return GenerateModelForLogarithmicTransformation();
      } else if (transformation is PowerTransformation) {
        return GenerateModelForPowerTransformation();
      } else if (transformation is ReciprocalTransformation) {
        return GenerateModelForReciprocalTransformation();
      } else if (transformation is ShiftStandardDistributionTransformation) {
        return GenerateModelForShiftStandardDistributionTransformation();
      } else if (transformation is CopyColumnTransformation) {
        return GenerateTreeNodeForCopyColumnTransformation();
      }
      throw new NotImplementedException();
    }

    public ISymbolicExpressionTreeNode GenerateInverseModel(ITransformation transformation) {
      InitComponents(transformation);

      if (transformation is LinearTransformation) {
        return GenerateInverseModelForLinearTransformation();
      } else if (transformation is ExponentialTransformation) {
        return GenerateInverseModelForExponentialTransformation();
      } else if (transformation is LogarithmicTransformation) {
        return GenerateInverseModelForLogarithmicTransformation();
      } else if (transformation is PowerTransformation) {
        return GenerateInverseModelForPowerTransformation();
      } else if (transformation is ReciprocalTransformation) {
        return GenerateInverseModelForReciprocalTransformation();
      } else if (transformation is ShiftStandardDistributionTransformation) {
        GenerateInverseModelForShiftStandardDistributionTransformation();
      } else if (transformation is CopyColumnTransformation) {
        return GenerateTreeNodeForCopyColumnTransformation();
      }

      throw new NotImplementedException();
    }

    #endregion

    // helper

    private ISymbolicExpressionTreeNode GenerateModelForLinearTransformation() {
      var linearTransformation = (LinearTransformation)transformation;
      var kValue = linearTransformation.Multiplier;
      var dValue = linearTransformation.Addend;

      // k * x
      var multiplicationNode = new Multiplication().CreateTreeNode();
      var kNode = CreateConstantTreeNode("k", kValue);
      var xNode = CreateVariableTreeNode(column, "x");
      multiplicationNode.AddSubtree(kNode);
      multiplicationNode.AddSubtree(xNode);

      // ( k * x ) + d
      var additionNode = new Addition().CreateTreeNode();
      var dNode = CreateConstantTreeNode("d", dValue);
      additionNode.AddSubtree(multiplicationNode);
      additionNode.AddSubtree(dNode);

      return additionNode;
    }

    private ISymbolicExpressionTreeNode GenerateInverseModelForLinearTransformation() {
      var linearTransformation = (LinearTransformation)transformation;
      var kValue = linearTransformation.Multiplier;
      var dValue = linearTransformation.Addend;

      // x - d
      var substractionNode = new Subtraction().CreateTreeNode();
      var dNode = CreateConstantTreeNode("d", dValue);
      var xNode = CreateVariableTreeNode(column, "x");
      substractionNode.AddSubtree(xNode);
      substractionNode.AddSubtree(dNode);

      // ( x - d ) / k
      var divisionNode = new Division().CreateTreeNode();
      var kNode = CreateConstantTreeNode("k", kValue);
      divisionNode.AddSubtree(substractionNode);
      divisionNode.AddSubtree(kNode);

      return divisionNode;
    }


    private ISymbolicExpressionTreeNode GenerateModelForExponentialTransformation() {
      var exponentialTransformation = (ExponentialTransformation)transformation;
      var bValue = exponentialTransformation.Base;

      return GenTreePow_b_x(bValue);
    }

    private ISymbolicExpressionTreeNode GenerateInverseModelForExponentialTransformation() {
      var exponentialTransformation = (ExponentialTransformation)transformation;
      var bValue = exponentialTransformation.Base;

      return GenTreeLog_x_b(bValue);
    }


    private ISymbolicExpressionTreeNode GenerateModelForLogarithmicTransformation() {
      var logarithmicTransformation = (LogarithmicTransformation)transformation;
      var bValue = logarithmicTransformation.Base;

      return GenTreeLog_x_b(bValue);
    }

    private ISymbolicExpressionTreeNode GenerateInverseModelForLogarithmicTransformation() {
      var logarithmicTransformation = (LogarithmicTransformation)transformation;
      var bValue = logarithmicTransformation.Base;

      return GenTreePow_b_x(bValue);
    }


    private ISymbolicExpressionTreeNode GenerateModelForPowerTransformation() {
      var powerTransformation = (PowerTransformation)transformation;
      var expValue = powerTransformation.Exponent;

      // x ^ exp
      var powerNode = new Power().CreateTreeNode();
      var xNode = CreateVariableTreeNode(column, "x");
      var expNode = CreateConstantTreeNode("exp", expValue);
      powerNode.AddSubtree(xNode);
      powerNode.AddSubtree(expNode);

      return powerNode;
    }

    private ISymbolicExpressionTreeNode GenerateInverseModelForPowerTransformation() {
      var powerTransformation = (PowerTransformation)transformation;
      var expValue = powerTransformation.Exponent;

      // rt(x, b)
      var rootNode = new Root().CreateTreeNode();
      var xNode = CreateVariableTreeNode(column, "x");
      var bNode = CreateConstantTreeNode("b", expValue);
      rootNode.AddSubtree(xNode);
      rootNode.AddSubtree(bNode);

      return rootNode;
    }


    private ISymbolicExpressionTreeNode GenerateModelForReciprocalTransformation() {
      return GenTreeDiv_1_x();
    }

    private ISymbolicExpressionTreeNode GenerateInverseModelForReciprocalTransformation() {
      return GenTreeDiv_1_x();
    }


    private ISymbolicExpressionTreeNode GenerateModelForShiftStandardDistributionTransformation() {
      var shiftStandardDistributionTransformation = (ShiftStandardDistributionTransformation)transformation;
      var m_orgValue = shiftStandardDistributionTransformation.OriginalMean;
      var s_orgValue = shiftStandardDistributionTransformation.OriginalStandardDeviation;
      var m_tarValue = shiftStandardDistributionTransformation.Mean;
      var s_tarValue = shiftStandardDistributionTransformation.StandardDeviation;

      return GenTreeShiftStdDist(column, m_orgValue, s_orgValue, m_tarValue, s_tarValue);
    }

    private ISymbolicExpressionTreeNode GenerateInverseModelForShiftStandardDistributionTransformation() {
      var shiftStandardDistributionTransformation = (ShiftStandardDistributionTransformation)transformation;
      var m_orgValue = shiftStandardDistributionTransformation.OriginalMean;
      var s_orgValue = shiftStandardDistributionTransformation.OriginalStandardDeviation;
      var m_tarValue = shiftStandardDistributionTransformation.Mean;
      var s_tarValue = shiftStandardDistributionTransformation.StandardDeviation;

      return GenTreeShiftStdDist(column, m_tarValue, s_tarValue, m_orgValue, s_orgValue);
    }

    private ISymbolicExpressionTreeNode GenerateTreeNodeForCopyColumnTransformation() {
      var copyColumnTransformation = (CopyColumnTransformation)transformation;
      var copiedColumnName = copyColumnTransformation.CopiedColumnName;

      return CreateVariableTreeNode(copiedColumnName, copiedColumnName + "(original)");
    }

    // helper's helper:

    private ISymbolicExpressionTreeNode GenTreeLog_x_b(double b) {
      // log(x, b)
      var logNode = new Logarithm().CreateTreeNode();
      var bNode = CreateConstantTreeNode("b", b);
      var xNode = CreateVariableTreeNode(column, "x");
      logNode.AddSubtree(xNode);
      logNode.AddSubtree(bNode);

      return logNode;
    }

    private ISymbolicExpressionTreeNode GenTreePow_b_x(double b) {
      // b ^ x
      var powerNode = new Power().CreateTreeNode();
      var bNode = CreateConstantTreeNode("b", b);
      var xNode = CreateVariableTreeNode(column, "x");
      powerNode.AddSubtree(bNode);
      powerNode.AddSubtree(xNode);

      return powerNode;
    }

    private ISymbolicExpressionTreeNode GenTreeDiv_1_x() {
      // 1 / x
      var divNode = new Division().CreateTreeNode();
      var oneNode = CreateConstantTreeNode("1", 1.0);
      var xNode = CreateVariableTreeNode(column, "x");
      divNode.AddSubtree(oneNode);
      divNode.AddSubtree(xNode);

      return divNode;
    }

    private ISymbolicExpressionTreeNode GenTreeShiftStdDist(string variable, double m_org, double s_org, double m_tar, double s_tar) {
      // x - m_org
      var substractionNode = new Subtraction().CreateTreeNode();
      var xNode = CreateVariableTreeNode(variable, "x");
      var m_orgNode = CreateConstantTreeNode("m_org", m_org);
      substractionNode.AddSubtree(xNode);
      substractionNode.AddSubtree(m_orgNode);

      // (x - m_org) / s_org
      var divisionNode = new Division().CreateTreeNode();
      var s_orgNode = CreateConstantTreeNode("s_org", s_org);
      divisionNode.AddSubtree(substractionNode);
      divisionNode.AddSubtree(s_orgNode);

      // ((x - m_org) / s_org ) * s_tar
      var multiplicationNode = new Multiplication().CreateTreeNode();
      var s_tarNode = CreateConstantTreeNode("s_tar", s_tar);
      multiplicationNode.AddSubtree(divisionNode);
      multiplicationNode.AddSubtree(s_tarNode);

      // ((x - m_org) / s_org ) * s_tar + m_tar
      var additionNode = new Addition().CreateTreeNode();
      var m_tarNode = CreateConstantTreeNode("m_tar", m_tar);
      additionNode.AddSubtree(multiplicationNode);
      additionNode.AddSubtree(m_tarNode);

      return additionNode;
    }

    private ConstantTreeNode CreateConstantTreeNode(string description, double value) {
      return new ConstantTreeNode(new Constant()) { Value = value };
    }

    private VariableTreeNode CreateVariableTreeNode(string name, string description) {
      return new VariableTreeNode(new Variable(name, description)) { VariableName = name, Weight = 1.0 };
    }

    private void InitComponents(ITransformation transformation) {
      this.transformation = transformation;
      column = transformation.Column;
    }
  }
}
