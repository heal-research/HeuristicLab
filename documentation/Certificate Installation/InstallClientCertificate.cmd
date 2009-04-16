@echo off
REM Note: Installation of Certificates requires Admin-Rights!
REM       Be sure the certhash is the same as on the generated certificate (fingerprint)!

REM --install public key on windows xp clients:
httpcfg set ssl -i 0.0.0.0:9000 -h 3ccf80ac7d3fac977f76517e576084f2a58382bd

REM --install public key on vista clients:
REM netsh http add sslcert ipport=0.0.0.0:9000 certhash=3ccf80ac7d3fac977f76517e576084f2a58382bd appid={00112233-4455-6677-8899-AABBCCDDEEFF}

pause