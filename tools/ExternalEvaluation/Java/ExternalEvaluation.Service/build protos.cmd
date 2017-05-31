@echo off
protoc.exe --java_out=./src -I ./protos ./protos/ExternalEvaluationMessages.proto