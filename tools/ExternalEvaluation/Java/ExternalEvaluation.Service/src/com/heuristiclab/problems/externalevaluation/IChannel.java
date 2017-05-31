package com.heuristiclab.problems.externalevaluation;

import java.io.*;
import com.google.protobuf.*;

public interface IChannel {
	boolean isInitialized();
	void open() throws IOException;
	void send(Message msg) throws IOException;
	Message receive(Message.Builder builder) throws IOException;
	void close() throws IOException;
}
