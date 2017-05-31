package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import com.google.protobuf.*;

public class StreamChannel extends Channel {
	private InputStream input;
	private OutputStream output;
	
	public StreamChannel(InputStream input, OutputStream output) {
		super();
		this.input = input;
		this.output = output;
	}

	@Override
	public void open() throws IOException {
		super.open();
	}
	
	@Override
	public void send(Message msg)
		throws IOException {
		synchronized (output) {
			msg.writeDelimitedTo(output);
	        output.flush(); // very important!
	    }
	}
	
	@Override
	public Message receive(Message.Builder builder)
		throws IOException {
		synchronized(input) {
			builder.mergeDelimitedFrom(input);
			if (builder.isInitialized())
				return builder.build();
			else throw new EOFException("EOF reached, but message is incomplete.");
		}
	}
	
	@Override
	public void close() throws IOException {
		super.close();
		input.close();
		output.close();
	}
}

