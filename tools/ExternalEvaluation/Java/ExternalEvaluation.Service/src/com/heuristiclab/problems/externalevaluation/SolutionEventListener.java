package com.heuristiclab.problems.externalevaluation;

import java.util.*;

public interface SolutionEventListener extends EventListener {
	void solutionReceived(EventObject evt);
}
