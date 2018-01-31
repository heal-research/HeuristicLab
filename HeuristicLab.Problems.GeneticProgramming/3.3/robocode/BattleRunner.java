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

import robocode.control.*;
import robocode.control.events.*;
import java.util.*;

//
// Add this file to the HL bin folder. Compile it with the following command:
// javac -cp c:\robocode\libs\robocode.jar BattleRunner.java
//
public class BattleRunner {
  public static String player = "Evaluation.output*";    
  public static List<Double> score = new ArrayList<Double>();

  // This program requires 5 arguments: 
  // The first argument is the name of the robot.
  // The second argument is the path to the robocode installation. If not give, c:\robocode is assumed. 
  // The third argument: true if Robocode should be shown, otherwise it is hidden. 
  // The fourth argument is the number of rounds. 
  // The remaining arguments are the names of the opponents. At least 1 must be provided. 
  public static void main(String[] args) {
    if (args.length < 5)
      System.exit(-1);  

    String roboCodePath = "C:\\robocode";
    Boolean visible = false;
    String bots = "";
    int numberOfRounds = 3;
    String[] robots = new String[1 + args.length - 4];

    player = robots[0] = args[0];
    roboCodePath = args[1];
    visible = Boolean.valueOf(args[2]);
    numberOfRounds = Integer.valueOf(args[3]);
    for (int i = 4; i < args.length; i++) {
      robots[i - 3] = args[i];
    }

    RobocodeEngine.setLogMessagesEnabled(false);
    RobocodeEngine engine = new RobocodeEngine(new java.io.File(roboCodePath));
    engine.setVisible(visible);
    engine.addBattleListener(new BattleObserver());

    BattlefieldSpecification battlefield = new BattlefieldSpecification(800, 600);
    RobotSpecification[] all = engine.getLocalRepository();
    List<RobotSpecification> selectedRobots = new ArrayList<RobotSpecification>();
    
    for(RobotSpecification rs : all) {
      for(String r : robots) {
        if(r.equals(rs.getClassName())) {
          selectedRobots.add(rs);
        }
      }
    }

    for (int i = 1; i < selectedRobots.size(); i++) {
      BattleSpecification battleSpec = new BattleSpecification(numberOfRounds, battlefield, new RobotSpecification[] { selectedRobots.get(0), selectedRobots.get(i) });

      // run our specified battle and wait till the battle finishes
      engine.runBattle(battleSpec, true);
    }
    engine.close();

    // print out result which is then parsed by HeuristicLab
    System.out.println(avg(score));
    System.exit(0);
  }

  private static double avg(List <Double> lst) {
    double sum = 0;
    for (double val : lst) {
      sum += val;
    }
    return sum / lst.size();
  }
}

//
// The battle listener for handling the battle event we are interested in.
//
class BattleObserver extends BattleAdaptor {
  public void onBattleCompleted(BattleCompletedEvent e) {
    double robotScore = -1.0;
    double opponentScore = -1.0;
    
        for (robocode.BattleResults result : e.getSortedResults()) {
            if (result.getTeamLeaderName().contains(BattleRunner.player)) 
                robotScore = result.getScore();
            else
                opponentScore = result.getScore();
        }
    
    //prevent div / 0 which can happen if both robots do not score
    if((robotScore + opponentScore) == 0) {
      BattleRunner.score.add(0.0);
    } else {
      BattleRunner.score.add(robotScore / (robotScore + opponentScore));
    }
  }
}