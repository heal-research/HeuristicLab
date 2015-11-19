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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  public static class Interpreter {

    // necessary for synchronization to guarantee that only one robocode program is executed at the same time
    // NOTE: this does not guarantee OS-wide mutual exclusion, but we ignore that for now
    private static readonly object syncRoot = new object();

    // TODO performance: it would probably be useful to implement the BattleRunner in such a way that we don't have to restart the java process each time, e.g. using console IO to load & run robots 
    public static double EvaluateTankProgram(ISymbolicExpressionTree tree, string path, EnemyCollection enemies, string robotName = null, bool showUI = false, int nrOfRounds = 3) {
      if (robotName == null)
        robotName = GenerateRobotName();

      string interpretedProgram = InterpretProgramTree(tree.Root, robotName);
      string battleRunnerPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "robocode");
      string roboCodeLibPath = Path.Combine(path, "libs");
      string robocodeJar = Path.Combine(roboCodeLibPath, "robocode.jar");
      string robocodeCoreJar = GetFileName(roboCodeLibPath, "robocode.core*");
      string picocontainerJar = GetFileName(roboCodeLibPath, "picocontainer*");
      string robotsPath = Path.Combine(path, "robots", "Evaluation");
      string srcRobotPath = Path.Combine(robotsPath, robotName + ".java");

      File.WriteAllText(srcRobotPath, interpretedProgram, System.Text.Encoding.Default);

      // compile java source to class file
      ProcessStartInfo javaCompileInfo = new ProcessStartInfo();
      javaCompileInfo.FileName = "cmd.exe";
      javaCompileInfo.Arguments = "/C javac -cp " + robocodeJar + "; " + srcRobotPath;
      javaCompileInfo.RedirectStandardOutput = true;
      javaCompileInfo.RedirectStandardError = true;
      javaCompileInfo.UseShellExecute = false;
      javaCompileInfo.CreateNoWindow = true;

      // it's ok to compile multiple robocode programs concurrently
      using (Process javaCompile = new Process()) {
        javaCompile.StartInfo = javaCompileInfo;
        javaCompile.Start();

        string cmdOutput = javaCompile.StandardOutput.ReadToEnd();
        cmdOutput += javaCompile.StandardError.ReadToEnd();

        javaCompile.WaitForExit();
        if (javaCompile.ExitCode != 0) {
          DeleteRobotFiles(path, robotName);
          throw new Exception("Compile Error: " + cmdOutput);
        }
      }

      ProcessStartInfo evaluateCodeInfo = new ProcessStartInfo();

      // execute a battle with numberOfRounds against a number of enemies
      // TODO: seems there is a bug when selecting multiple enemies
      evaluateCodeInfo.FileName = "cmd.exe";
      var classpath = string.Join(";", new[] { battleRunnerPath, robocodeCoreJar, robocodeJar, picocontainerJar });
      var enemyRobotNames = string.Join(" ", enemies.CheckedItems.Select(i => i.Value));
      evaluateCodeInfo.Arguments = string.Format("/C java -cp {0} BattleRunner Evaluation.{1} {2} {3} {4} {5}", classpath, robotName, path, showUI, nrOfRounds, enemyRobotNames);

      evaluateCodeInfo.RedirectStandardOutput = true;
      evaluateCodeInfo.RedirectStandardError = true;
      evaluateCodeInfo.UseShellExecute = false;
      evaluateCodeInfo.CreateNoWindow = true;

      // the robocode framework writes state to a file therefore parallel evaluation of multiple robocode programs is not possible yet.
      double evaluation;
      lock (syncRoot) {
        using (Process evaluateCode = new Process()) {
          evaluateCode.StartInfo = evaluateCodeInfo;
          evaluateCode.Start();
          evaluateCode.WaitForExit();

          if (evaluateCode.ExitCode != 0) {
            DeleteRobotFiles(path, robotName);
            throw new Exception("Error running Robocode: " + evaluateCode.StandardError.ReadToEnd() +
                                Environment.NewLine +
                                evaluateCode.StandardOutput.ReadToEnd());
          }

          try {
            string scoreString =
              evaluateCode.StandardOutput.ReadToEnd()
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Last();
            evaluation = Double.Parse(scoreString, CultureInfo.InvariantCulture);
          }
          catch (Exception ex) {
            throw new Exception("Error parsing score string: " + ex);
          }
          finally {
            DeleteRobotFiles(path, robotName);
          }
        }
      }

      return evaluation;
    }

    private static void DeleteRobotFiles(string path, string outputname) {
      File.Delete(path + @"\robots\Evaluation\" + outputname + ".java");
      File.Delete(path + @"\robots\Evaluation\" + outputname + ".class");
    }

    private static string GetFileName(string path, string pattern) {
      string fileName = string.Empty;
      try {
        fileName = Directory.GetFiles(path, pattern).First();
      }
      catch {
        throw new Exception("Error finding required Robocode files.");
      }
      return fileName;
    }

    private static string GenerateRobotName() {
      // Robocode class names are 32 char max and 
      // Java class names have to start with a letter
      string outputname = Guid.NewGuid().ToString();
      outputname = outputname.Remove(8, 1);
      outputname = outputname.Remove(12, 1);
      outputname = outputname.Remove(16, 1);
      outputname = outputname.Remove(20, 1);
      outputname = outputname.Remove(0, 1);
      outputname = outputname.Insert(0, "R");
      return outputname;
    }

    public static string InterpretProgramTree(ISymbolicExpressionTreeNode node, string robotName) {
      var tankNode = node;
      while (tankNode.Symbol.Name != "Tank")
        tankNode = tankNode.GetSubtree(0);

      string result = Interpret(tankNode);
      result = result.Replace("class output", "class " + robotName);
      return result;
    }

    private static string Interpret(ISymbolicExpressionTreeNode node) {
      switch (node.Symbol.Name) {
        case "Block": return InterpretBlock(node);
        case "Statement": return InterpretStat(node);
        case "DoNothing": return string.Empty;
        case "EmptyEvent": return string.Empty;

        case "GetEnergy": return "getEnergy()";
        case "GetHeading": return "getHeading()";
        case "GetGunHeading": return "getGunHeading()";
        case "GetRadarHeading": return "getRadarHeading()";
        case "GetX": return "getX()";
        case "GetY": return "getY()";

        case "Addition": return InterpretBinaryOperator(" + ", node);
        case "Subtraction": return InterpretBinaryOperator(" - ", node);
        case "Multiplication": return InterpretBinaryOperator(" * ", node);
        case "Division": return InterpretBinaryOperator(" / ", node);
        case "Modulus": return InterpretBinaryOperator(" % ", node);

        case "Equal": return InterpretComparison(" == ", node);
        case "LessThan": return InterpretComparison(" < ", node);
        case "LessThanOrEqual": return InterpretComparison(" <= ", node);
        case "GreaterThan": return InterpretComparison("  >", node);
        case "GreaterThanOrEqual": return InterpretComparison(" >= ", node);
        case "ConditionalAnd": return InterpretBinaryOperator(" && ", node);
        case "ConditionalOr": return InterpretBinaryOperator(" || ", node);
        case "Negation": return InterpretFunc1("!", node);

        case "IfThenElseStat": return InterpretIf(node);
        case "WhileStat": return InterpretWhile(node);
        case "BooleanExpression": return InterpretChild(node);
        case "NumericalExpression": return InterpretChild(node);
        case "Number": return InterpretNumber(node);
        case "BooleanValue": return InterpretBoolValue(node);


        case "Ahead": return InterpretFunc1("setAhead", node);
        case "Back": return InterpretFunc1("setBack", node);
        case "Fire": return InterpretFunc1("setFire", node);
        case "TurnLeft": return InterpretFunc1("setTurnLeft", node);
        case "TurnRight": return InterpretFunc1("setTurnRight", node);
        case "TurnGunLeft": return InterpretFunc1("setTurnGunLeft", node);
        case "TurnGunRight": return InterpretFunc1("setTurnGunRight", node);
        case "TurnRadarLeft": return InterpretFunc1("setTurnRadarLeft", node);
        case "TurnRadarRight": return InterpretFunc1("setTurnRadarRight", node);
        case "ShotPower": return InterpetShotPower(node);

        case "OnBulletHit": return InterpretOnBulletHit(node);
        case "OnBulletMissed": return InterpretOnBulletMissed(node);
        case "OnHitByBullet": return InterpretOnHitByBullet(node);
        case "OnHitRobot": return InterpretOnHitRobot(node);
        case "OnHitWall": return InterpretOnHitWall(node);
        case "OnScannedRobot": return InterpretOnScannedRobot(node);

        case "Run": return InterpretRun(node);
        case "Tank": return InterpretTank(node);
        case "CodeSymbol": return InterpretCodeSymbol(node);

        default: throw new ArgumentException(string.Format("Found an unknown symbol {0} in a robocode solution", node.Symbol.Name));
      }
    }

    private static string InterpretCodeSymbol(ISymbolicExpressionTreeNode node) {
      var sy = node.Symbol as CodeSymbol;
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"{0}
{1}
{2}", sy.Prefix, code, sy.Suffix);
    }

    private static string InterpretBoolValue(ISymbolicExpressionTreeNode node) {
      var boolNode = node as BooleanTreeNode;
      return string.Format(NumberFormatInfo.InvariantInfo, "{0}", boolNode.Value).ToLower();
    }

    private static string InterpretNumber(ISymbolicExpressionTreeNode node) {
      var numberNode = node as NumberTreeNode;
      return string.Format(NumberFormatInfo.InvariantInfo, "{0}", numberNode.Value);
    }

    private static string InterpetShotPower(ISymbolicExpressionTreeNode node) {
      var shotPowerNode = node as ShotPowerTreeNode;
      return string.Format(NumberFormatInfo.InvariantInfo, "{0:E}", shotPowerNode.Value);
    }


    internal static string InterpretBlock(ISymbolicExpressionTreeNode node) {
      string result = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format("{{ {0} }}", result + Environment.NewLine);
    }

    internal static string InterpretStat(ISymbolicExpressionTreeNode node) {
      // must only have one sub-tree
      Contract.Assert(node.SubtreeCount == 1);
      return Interpret(node.GetSubtree(0)) + " ;" + Environment.NewLine;
    }

    internal static string InterpretIf(ISymbolicExpressionTreeNode node) {
      ISymbolicExpressionTreeNode condition = null, truePart = null, falsePart = null;
      string[] parts = new string[3];

      if (node.SubtreeCount < 2 || node.SubtreeCount > 3)
        throw new Exception("Unexpected number of children. Expected 2 or 3 children.");

      condition = node.GetSubtree(0);
      truePart = node.GetSubtree(1);
      if (node.SubtreeCount == 3)
        falsePart = node.GetSubtree(2);

      parts[0] = Interpret(condition);
      parts[1] = Interpret(truePart);
      if (falsePart != null) parts[2] = Interpret(falsePart);

      return string.Format("if ({0}) {{ {1} }} else {{ {2} }}", Interpret(condition), Interpret(truePart),
        falsePart == null ? string.Empty : Interpret(falsePart));
    }

    internal static string InterpretWhile(ISymbolicExpressionTreeNode node) {
      var cond = Interpret(node.GetSubtree(0));
      var body = Interpret(node.GetSubtree(1));
      return string.Format("while ({0}) {{ {2} {1} {2} }} {2}", cond, body, Environment.NewLine);
    }

    public static string InterpretBinaryOperator(string opSy, ISymbolicExpressionTreeNode node) {
      if (node.SubtreeCount < 2)
        throw new ArgumentException(string.Format("Expected at least two children in {0}.", node.Symbol), "node");

      string result = string.Join(opSy, node.Subtrees.Select(Interpret));
      return "(" + result + ")";
    }

    public static string InterpretChild(ISymbolicExpressionTreeNode node) {
      if (node.SubtreeCount != 1)
        throw new ArgumentException(string.Format("Expected exactly one child in {0}.", node.Symbol), "node");

      return Interpret(node.GetSubtree(0));
    }

    public static string InterpretComparison(string compSy, ISymbolicExpressionTreeNode node) {
      ISymbolicExpressionTreeNode lhs = null, rhs = null;
      if (node.SubtreeCount != 2)
        throw new ArgumentException(string.Format("Expected exactly two children in {0}.", node.Symbol), "node");

      lhs = node.GetSubtree(0);
      rhs = node.GetSubtree(1);

      return Interpret(lhs) + " == " + Interpret(rhs);
    }

    public static string InterpretFunc1(string functionId, ISymbolicExpressionTreeNode node) {
      if (node.SubtreeCount != 1)
        throw new ArgumentException(string.Format("Expected 1 child in {0}.", node.Symbol.Name), "node");

      return string.Format("{0}({1})", functionId, Interpret(node.GetSubtree(0)));
    }

    public static string InterpretOnScannedRobot(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"public void onScannedRobot(ScannedRobotEvent e) {{
  double absoluteBearing = getHeading() + e.getBearing();
  double bearingFromGun = normalRelativeAngleDegrees(absoluteBearing - getGunHeading());
  setTurnGunRight(bearingFromGun);
{0}
  execute();
}}", code);
    }


    public static string InterpretOnHitWall(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"public void onHitWall(HitWallEvent e) {{
{0}
execute();
}}", code);
    }

    public static string InterpretOnHitRobot(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"public void onHitRobot(HitRobotEvent e) {{
{0}
execute();
}}", code);
    }

    public static string InterpretOnHitByBullet(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"public void onHitByBullet(HitByBulletEvent e) {{
{0}
execute();
}}", code);
    }

    public static string InterpretOnBulletMissed(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"public void onBulletMissed(BulletMissedEvent e) {{
{0}
execute();
}}", code);
    }

    public static string InterpretOnBulletHit(ISymbolicExpressionTreeNode node) {
      var Prefix = "public void onBulletHit(BulletHitEvent e) {";
      var Suffix =
@"execute();
}";
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return Prefix + code + Environment.NewLine + Suffix;
    }

    public static string InterpretRun(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"public void run() {{
  setAdjustGunForRobotTurn(true);
  turnRadarRightRadians(Double.POSITIVE_INFINITY);
{0}
  execute();
}}", code);
    }

    public static string InterpretTank(ISymbolicExpressionTreeNode node) {
      string code = string.Join(Environment.NewLine, node.Subtrees.Select(Interpret));
      return string.Format(
@"package Evaluation;
import robocode.*;
import robocode.Robot;
import robocode.util.*;
import static robocode.util.Utils.normalRelativeAngleDegrees;
import java.awt.*;

public class output extends AdvancedRobot {{ 
{0}
}}", code);
    }
  }
}