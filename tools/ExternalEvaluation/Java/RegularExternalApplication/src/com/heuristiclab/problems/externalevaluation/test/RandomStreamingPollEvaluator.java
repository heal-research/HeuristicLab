package com.heuristiclab.problems.externalevaluation.test;

import java.io.IOException;
import java.util.Random;
import com.heuristiclab.problems.externalevaluation.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;

public class RandomStreamingPollEvaluator {

	public static void main(String[] args) {
		StreamListenerFactory factory = new StreamListenerFactory(System.in, System.out);
		PollService service = new PollService(factory, 1);
		service.start();
		
		Random random = new Random();
		while (true) {
			SolutionMessage msg = service.getSolution();
			// parse the message and retrieve the variables there
			try {
				service.sendQuality(msg, random.nextDouble());
			} catch (IOException e) {
				break;
			}
		}
		
		service.stop();
	}
}
