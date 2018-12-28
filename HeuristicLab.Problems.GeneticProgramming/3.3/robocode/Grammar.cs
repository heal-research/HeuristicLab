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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableType("DC5FF90A-AD5A-4A56-8A9B-F117D2B842AD")]
  [Item("Robocode Grammar", "The grammar for the Robocode GP problem.")]
  public class Grammar : SymbolicExpressionGrammar {
    private const string EventsName = "Events";
    private const string ExpressionsName = "Expressions";
    private const string ControlStatementsName = "Control Statements";
    private const string RobocodeFunctionsName = "Robocode Functions";
    private const string RobocodeActionsName = "Robocode Actions";
    private const string RelationalOperatorsName = "Relational Operators";
    private const string LogicalOperators = "Logical Operators";
    private const string NumericalOperatorsName = "Numerical Operators";

    [StorableConstructor]
    protected Grammar(StorableConstructorFlag _) : base(_) { }
    protected Grammar(Grammar original, Cloner cloner) : base(original, cloner) { }

    public Grammar()
      : base("Robocode Grammar", "The grammar for the Robocode GP problem.") {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Grammar(this, cloner);
    }

    // initialize set of allowed symbols and define 
    // the allowed combinations of symbols
    private void Initialize() {
      #region Symbols
      var block = new SimpleSymbol("Block", "A group of statements", 1, byte.MaxValue);
      var stat = new SimpleSymbol("Statement", "A statement.", 1, 1);
      var ifThenElseStat = new SimpleSymbol("IfThenElseStat", "An if statement.", 2, 3);
      var whileStat = new SimpleSymbol("WhileStat", "A while statement.", 2, 2);
      whileStat.Enabled = false;

      var boolExpr = new SimpleSymbol("BooleanExpression", "A Boolean expression.", 1, 1);
      var numericalExpr = new SimpleSymbol("NumericalExpression", "A numerical expression.", 1, 1);

      var equal = new SimpleSymbol("Equal", "Equal comparator.", 2, 2);
      var lessThan = new SimpleSymbol("LessThan", "LessThan comparator.", 2, 2);
      var lessThanOrEqual = new SimpleSymbol("LessThanOrEqual", "LessThanOrEqual comparator.", 2, 2);
      var greaterThan = new SimpleSymbol("GreaterThan", "GreaterThan comparator.", 2, 2);
      var greaterThanOrEqual = new SimpleSymbol("GreaterThanOrEqual", "GreaterThanOrEqual comparator.", 2, 2);

      var conjunction = new SimpleSymbol("ConditionalAnd", "Conjunction comparator.", 2, byte.MaxValue);
      var disjunction = new SimpleSymbol("ConditionalOr", "Disjunction comparator.", 2, byte.MaxValue);
      var negation = new SimpleSymbol("Negation", "A negation.", 1, 1);

      var addition = new SimpleSymbol("Addition", "Addition operator.", 2, byte.MaxValue);
      var subtraction = new SimpleSymbol("Subtraction", "Subtraction operator.", 2, byte.MaxValue);
      var multiplication = new SimpleSymbol("Multiplication", "Multiplication operator.", 2, byte.MaxValue);
      var division = new SimpleSymbol("Division", "Division operator.", 2, byte.MaxValue);
      var modulus = new SimpleSymbol("Modulus", "Modulus operator.", 2, byte.MaxValue);

      var number = new Number();
      var logicalVal = new BooleanValue();

      var ahead = new SimpleSymbol("Ahead", "Immediately moves your robot ahead (forward) by distance measured in pixels.", 1, 1);
      var back = new SimpleSymbol("Back", "Immediately moves your robot backward by distance measured in pixels.", 1, 1);
      var fire = new SimpleSymbol("Fire", "Immediately fires a bullet.", 1, 1);
      var shotPower = new ShotPower();

      var getEnergy = new SimpleSymbol("GetEnergy", "Returns the robot's current energy.", 0, 0);
      var getHeading = new SimpleSymbol("GetHeading", "Returns the direction that the robot's body is facing, in degrees.", 0, 0);
      var getGunHeading = new SimpleSymbol("GetGunHeading", "Returns the direction that the robot's gun is facing, in degrees.", 0, 0);
      var getRadarHeading = new SimpleSymbol("GetRadarHeading", "Returns the direction that the robot's radar is facing, in degrees.", 0, 0);
      var getX = new SimpleSymbol("GetX", "Returns the X position of the robot. (0,0) is at the bottom left of the battlefield.", 0, 0);
      var getY = new SimpleSymbol("GetY", "Returns the Y position of the robot. (0,0) is at the bottom left of the battlefield.", 0, 0);

      var turnLeft = new SimpleSymbol("TurnLeft", "Immediately turns the robot's body to the left by degrees.", 1, 1);
      var turnRight = new SimpleSymbol("TurnRight", "Immediately turns the robot's body to the right by degrees.", 1, 1);
      var turnGunLeft = new SimpleSymbol("TurnGunLeft", "Immediately turns the robot's gun to the left by degrees.", 1, 1);
      var turnGunRight = new SimpleSymbol("TurnGunRight", "Immediately turns the robot's gun to the right by degrees.", 1, 1);
      var turnRadarLeft = new SimpleSymbol("TurnRadarLeft", "Immediately turns the robot's radar to the left by degrees.", 1, 1);
      var turnRadarRight = new SimpleSymbol("TurnRadarRight", "Immediately turns the robot's radar to the right by degrees.", 1, 1);

      var onBulletHit = new SimpleSymbol("OnBulletHit", "This method is called when one of your bullets hits another robot.", 2, 10);
      var onBulletMissed = new SimpleSymbol("OnBulletMissed", "This method is called when one of your bullets misses, i.e. hits a wall.", 2, 10);
      var onHitByBullet = new SimpleSymbol("OnHitByBullet", "This method is called when your robot is hit by a bullet.", 2, 10);
      var onHitRobot = new SimpleSymbol("OnHitRobot", "This method is called when your robot collides with another robot.", 2, 10);
      var onHitWall = new SimpleSymbol("OnHitWall", "This method is called when your robot collides with a wall.", 2, 10);
      var onScannedRobot = new SimpleSymbol("OnScannedRobot", "This method is called when your robot sees another robot, i.e. when the robot's radar scan \"hits\" another robot.", 2, 10);

      var run = new SimpleSymbol("Run", "The main method in every robot.", 1, 10);
      var tank = new SimpleSymbol("Tank", "The root of a Robocode Tank program.", 2, 7);

      var doNothing = new SimpleSymbol("DoNothing", "Do nothing this turn, meaning that the robot will skip its turn.", 0, 0);
      var emptyEvent = new SimpleSymbol("EmptyEvent", "This is a placeholder for an empty event.", 0, 0);
      #endregion

      #region Symbol Collections
      var controlSymbols = new ISymbol[] { ifThenElseStat, whileStat };
      var actionSymbols = new ISymbol[] {
        ahead, back, fire, turnGunLeft, turnGunRight, turnLeft, turnRadarLeft, turnRadarRight, turnRight
      };
      var functionSymbols = new ISymbol[] {
        getEnergy, getGunHeading, getHeading, getRadarHeading, getX, getY
      };

      var events = new GroupSymbol(EventsName, new ISymbol[] { run, onScannedRobot, onBulletHit, onBulletMissed, onHitByBullet, onHitRobot, onHitWall });
      var controlStatements = new GroupSymbol(ControlStatementsName, controlSymbols);
      var expressions = new GroupSymbol(ExpressionsName, new ISymbol[] { boolExpr, numericalExpr });
      var robocodeFunctions = new GroupSymbol(RobocodeFunctionsName, functionSymbols);
      var robocodeActions = new GroupSymbol(RobocodeActionsName, actionSymbols);
      var relationalOperators = new GroupSymbol(RelationalOperatorsName, new ISymbol[] { equal, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual });
      var logicalOperators = new GroupSymbol(LogicalOperators, new ISymbol[] { conjunction, disjunction, negation });
      var numericalOperators = new GroupSymbol(NumericalOperatorsName, new ISymbol[] { addition, subtraction, multiplication, division, modulus });
      #endregion

      #region Adding Symbols
      AddSymbol(tank);
      AddSymbol(stat);
      AddSymbol(block);
      AddSymbol(shotPower);
      AddSymbol(number);
      AddSymbol(logicalVal);
      AddSymbol(events);
      AddSymbol(expressions);
      AddSymbol(controlStatements);
      AddSymbol(robocodeFunctions);
      AddSymbol(robocodeActions);
      AddSymbol(relationalOperators);
      AddSymbol(logicalOperators);
      AddSymbol(numericalOperators);
      AddSymbol(emptyEvent);
      AddSymbol(doNothing);
      #endregion

      #region Grammar Definition
      // StartSymbol
      AddAllowedChildSymbol(StartSymbol, tank);

      // Tank
      AddAllowedChildSymbol(tank, run, 0);
      AddAllowedChildSymbol(tank, onScannedRobot, 1);
      AddAllowedChildSymbol(tank, onBulletHit, 2);
      AddAllowedChildSymbol(tank, onBulletMissed, 3);
      AddAllowedChildSymbol(tank, onHitByBullet, 4);
      AddAllowedChildSymbol(tank, onHitRobot, 5);
      AddAllowedChildSymbol(tank, onHitWall, 6);

      // Events
      AddAllowedChildSymbol(events, stat);

      // Block
      AddAllowedChildSymbol(block, stat);

      // Stat
      AddAllowedChildSymbol(stat, stat);
      AddAllowedChildSymbol(stat, block);
      AddAllowedChildSymbol(stat, controlStatements);
      AddAllowedChildSymbol(stat, robocodeFunctions);
      AddAllowedChildSymbol(stat, robocodeActions);
      AddAllowedChildSymbol(stat, emptyEvent);
      AddAllowedChildSymbol(stat, doNothing);

      // IfStat
      AddAllowedChildSymbol(ifThenElseStat, boolExpr, 0);
      AddAllowedChildSymbol(ifThenElseStat, stat, 1);
      AddAllowedChildSymbol(ifThenElseStat, emptyEvent, 1);
      AddAllowedChildSymbol(ifThenElseStat, doNothing, 1);
      AddAllowedChildSymbol(ifThenElseStat, stat, 2);
      AddAllowedChildSymbol(ifThenElseStat, emptyEvent, 2);
      AddAllowedChildSymbol(ifThenElseStat, doNothing, 2);

      // WhileStat
      AddAllowedChildSymbol(whileStat, boolExpr, 0);
      AddAllowedChildSymbol(whileStat, stat, 1);
      AddAllowedChildSymbol(whileStat, emptyEvent, 1);
      AddAllowedChildSymbol(whileStat, doNothing, 1);

      // Numerical Expressions
      AddAllowedChildSymbol(numericalExpr, number);
      AddAllowedChildSymbol(numericalExpr, robocodeFunctions);
      AddAllowedChildSymbol(numericalExpr, numericalOperators);
      AddAllowedChildSymbol(numericalOperators, number);
      AddAllowedChildSymbol(numericalOperators, robocodeFunctions);
      AddAllowedChildSymbol(numericalOperators, numericalOperators);

      // Logical Expressions
      AddAllowedChildSymbol(boolExpr, logicalVal);
      AddAllowedChildSymbol(boolExpr, logicalOperators);
      AddAllowedChildSymbol(logicalOperators, logicalVal);
      AddAllowedChildSymbol(logicalOperators, logicalOperators);
      AddAllowedChildSymbol(logicalOperators, relationalOperators);
      AddAllowedChildSymbol(relationalOperators, numericalExpr);

      // Functions and Actions
      AddAllowedChildSymbol(robocodeFunctions, numericalExpr);
      foreach (var a in robocodeActions.Symbols) {
        if (a.Name == fire.Name) AddAllowedChildSymbol(a, shotPower);
        else AddAllowedChildSymbol(a, numericalExpr);
      }
      #endregion
    }
  }
}