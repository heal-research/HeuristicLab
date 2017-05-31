package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import com.google.protobuf.*;

public abstract class Channel implements IChannel {
    protected boolean initialized;
    
    protected Channel() {
    }
    
	@Override
	public boolean isInitialized() {
		return initialized;
	}
	
	@Override
	public void open() throws IOException {
		initialized = true;
	}

	@Override
	public abstract void send(Message msg) throws IOException;

	@Override
	public abstract Message receive(Message.Builder builder) throws IOException;

	@Override
	public void close() throws IOException {
		initialized = false;
	}
}
