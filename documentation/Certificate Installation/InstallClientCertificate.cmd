@echo off
REM Note: Installation of Certificates requires Admin-Rights!
REM       Be sure the certhash is the same as on the generated certificate (fingerprint)!

REM --install public key on windows xp clients:
httpcfg set ssl -i 0.0.0.0:9000 -h 280aea192b55742cb44b171326a15383afa2819a

REM --install public key on vista clients:
REM netsh http add sslcert ipport=0.0.0.0:9000 certhash=280aea192b55742cb44b171326a15383afa2819a appid={00112233-4455-6677-8899-AABBCCDDEEFF}

pause