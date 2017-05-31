package com.heuristiclab.problems.externalevaluation.repetitionsextension;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Random;
import com.heuristiclab.problems.externalevaluation.*;
import com.heuristiclab.problems.externalevaluation.ExternalEvaluationMessages.*;
import com.heuristiclab.problems.externalevaluation.repetitionsextension.RepetitionsQualityMessage.RepetitionsResponse;

public class RandomSocketPushEvaluator {
	private PushService service;
	
	public static void main(String[] args) {
		RandomSocketPushEvaluator main = new RandomSocketPushEvaluator();
		main.run();
		System.out.println("Service is running, terminate by pressing <Return> or <Enter>.");
		BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
		try {
			br.readLine();
			br.close();
		} catch (IOException e) { e.printStackTrace(); }
		main.terminate();
	}
	
	private void run() {
		ServerSocketListenerFactory factory = new ServerSocketListenerFactory(8842);
		service = new PushService(factory, 1, new RandomEvaluator());
		service.start();
	}
	
	private void terminate() {
		service.stop();
	}
	
	private class RandomEvaluator implements IEvaluationService {
		Random random;
		
		public RandomEvaluator() {
			random = new Random();
		}
		
		@Override
		public double evaluate(SolutionMessage msg, QualityMessage.Builder responseBuilder) {
			responseBuilder.setExtension(RepetitionsResponse.repetitions, 1);
			return random.nextDouble();
		}
		
	}
}
