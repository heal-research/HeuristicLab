FROM mono:latest

COPY ./bin /hl/

ENTRYPOINT [ "mono", "/hl/HeuristicLab 3.3.exe", "/start:JsonInterface" ]