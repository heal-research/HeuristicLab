package com.heuristiclab.problems.externalevaluation;

public interface IListener {
	void listen();
    void stop();
    void addChannelDiscoveredEventListener(ChannelDiscoveredEventListener l);
    void removeChannelDiscoveredEventListener(ChannelDiscoveredEventListener l);
}
