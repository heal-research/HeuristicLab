package com.heuristiclab.problems.externalevaluation.test;

import java.io.IOException;
import java.util.*;
import com.heuristiclab.problems.externalevaluation.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;

public class RandomSocketPollEvaluator {

	public static void main(String[] args) {
		ServerSocketListenerFactory factory = new ServerSocketListenerFactory(8842);
		PollService service = new PollService(factory, 1);
		service.start();
		System.out.println("service started");
		
		Random random = new Random();
		while (true) {
			SolutionMessage msg = service.getSolution();
			double quality = 0;
			double[] qualities = new double[3];
			
			// single-objective
			quality = random.nextDouble();
			// multi-objective
			//for (int i = 0; i < qualities.length; i++)
			//	qualities[i] = random.nextDouble();
			
			try {
				// single-objective
				service.sendQuality(msg, quality);
				// multi-objective
				//service.sendQualities(msg, qualities);
				System.out.println("Quality sended");
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}
}
