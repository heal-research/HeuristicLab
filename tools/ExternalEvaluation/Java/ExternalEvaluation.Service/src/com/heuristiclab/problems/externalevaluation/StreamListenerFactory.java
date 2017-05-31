package com.heuristiclab.problems.externalevaluation;

import java.io.*;

public class StreamListenerFactory implements IListenerFactory {

	InputStream input;
	OutputStream output;
	
	public StreamListenerFactory(InputStream input, OutputStream output) {
		this.input = input;
		this.output = output;
	}
	
	@Override
	public IListener createListener() {
		return new StreamListener(input, output);
	}

}
