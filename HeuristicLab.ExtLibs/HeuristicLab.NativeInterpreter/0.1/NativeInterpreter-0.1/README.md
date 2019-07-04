# Build instructions

1) Create a folder to hold the build 

`mkdir build` 

`cd build`

2) Run CMake 

- Visual Studio

    `cmake -G "Visual Studio 15 2017 [arch]" <path-to-repository>`. Leave `arch` empty for x86 or `Win64` for x64. 
    
    `cmake --build . --config Release`

- MinGW

    `cmake -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=Release|Debug`. CMake supports different build types: `Release`, `Debug`, `MinSizeRel` and `RelWithDebInfo`. 
    
    `cmake --build .` or `make` or `make VERBOSE=1`
    
The build system generates two targets, one using the standard math library functions (`std::exp`, `std::log`, etc.) and the other using functions from the [vdt](https://github.com/dpiparo/vdt) library (described in more detail in [this talk from CERN](https://indico.cern.ch/event/202688/contributions/1487957/attachments/305612/426820/OpenLabFPWorkshop_9_2012.pdf)). A performance comparison with existing HL interpreters is given below. MinGW and MSVC builds of this library were tested. The non-vdt MinGW binary suffers significant peformance loss due to trancendental functions `log` and `exp`. All MinGW binaries also exhibit performance loss with the `pow` and `root` functions. Batched versions take advantage of auto-vectorization. Overall the `Native-MinGW-Vdt[BatchSize=64]` performs the best on the selected grammars. Units in the table correspond to billion node evaluations per second.
    
    
# Performance

## Arithmetic Grammar 

|  Rows | StandardInterpreter | LinerInterpreter | ILEmittingInterpreter | CompiledTreeInterpreter | Native-MSVC-Std | Native-MSVC-Vdt | Native-MinGW-Std | Native-MinGW-Vdt | Native-MSVC-Std[BatchSize=64] | Native-MSVC-Vdt[BatchSize=64] | Native-MinGW-Std[BatchSize=64] | Native-MinGW-Vdt[BatchSize=64] |
|:-----:|:-------------------:|:----------------:|:---------------------:|:-----------------------:|:---------------:|:---------------:|:----------------:|:----------------:|:-----------------------------:|:-----------------------------:|:------------------------------:|:------------------------------:|
|  1000 |        0.0430       |      0.2115      |         0.0922        |          0.0817         |      0.5487     |      0.5118     |      0.5512      |      0.5665      |             1.2304            |             1.1942            |             1.1942             |             1.2688             |
|  2000 |        0.0488       |      0.2048      |         0.1584        |          0.1501         |      0.5845     |      0.5803     |      0.5817      |      0.5668      |             1.3247            |             1.1497            |             1.3693             |             1.3617             |
|  3000 |        0.0487       |      0.2087      |         0.2142        |          0.2049         |      0.5832     |      0.5678     |      0.5851      |      0.5722      |             1.4745            |             1.2876            |             1.4986             |             1.4986             |
|  4000 |        0.0488       |      0.2113      |         0.2492        |          0.2423         |      0.4828     |      0.5254     |      0.5472      |      0.5696      |             1.5237            |             1.5190            |             1.4555             |             1.4341             |
|  5000 |        0.0489       |      0.2072      |         0.2645        |          0.2576         |      0.5394     |      0.4646     |      0.5788      |      0.5618      |             1.3790            |             1.0923            |             1.5276             |             1.5509             |
|  6000 |        0.0486       |      0.2065      |         0.3038        |          0.3248         |      0.5875     |      0.5861     |      0.5020      |      0.4713      |             1.5239            |             1.0601            |             1.4897             |             1.5399             |
|  7000 |        0.0489       |      0.2082      |         0.3261        |          0.3518         |      0.5751     |      0.5869     |      0.5805      |      0.5993      |             1.1952            |             1.0627            |             1.2332             |             1.2191             |
|  8000 |        0.0487       |      0.2086      |         0.3467        |          0.3760         |      0.5890     |      0.5737     |      0.5781      |      0.6024      |             1.3660            |             1.5192            |             1.3622             |             1.4428             |
|  9000 |        0.0485       |      0.2033      |         0.3653        |          0.3924         |      0.5915     |      0.5918     |      0.5778      |      0.5855      |             1.4990            |             1.5476            |             1.5742             |             1.5325             |
| 10000 |        0.0486       |      0.2048      |         0.3749        |          0.4080         |      0.5825     |      0.6015     |      0.5797      |      0.6069      |             1.5221            |             1.5671            |             1.4601             |             1.5958             |

## TypeCoherent Grammar

|  Rows | StandardInterpreter | LinerInterpreter | ILEmittingInterpreter | CompiledTreeInterpreter | Native-MSVC-Std | Native-MSVC-Vdt | Native-MinGW-Std | Native-MinGW-Vdt | Native-MSVC-Std[BatchSize=64] | Native-MSVC-Vdt[BatchSize=64] | Native-MinGW-Std[BatchSize=64] | Native-MinGW-Vdt[BatchSize=64] |
|:-----:|:-------------------:|:----------------:|:---------------------:|:-----------------------:|:---------------:|:---------------:|:----------------:|:----------------:|:-----------------------------:|:-----------------------------:|:------------------------------:|:------------------------------:|
|  1000 |        0.0435       |      0.1522      |         0.0813        |          0.0732         |      0.3049     |      0.3074     |      0.2419      |      0.3098      |             0.7542            |             0.7928            |             0.3049             |             1.1368             |
|  2000 |        0.0441       |      0.1569      |         0.1254        |          0.1165         |      0.3066     |      0.3138     |      0.2407      |      0.3170      |             0.7932            |             0.8429            |             0.3199             |             1.2424             |
|  3000 |        0.0441       |      0.1536      |         0.1521        |          0.1476         |      0.3115     |      0.3088     |      0.2397      |      0.3136      |             0.8595            |             0.8675            |             0.3268             |             1.3074             |
|  4000 |        0.0440       |      0.1559      |         0.1708        |          0.1692         |      0.3122     |      0.3166     |      0.2420      |      0.3172      |             0.8443            |             0.8918            |             0.3247             |             1.3084             |
|  5000 |        0.0436       |      0.1543      |         0.1829        |          0.1852         |      0.3119     |      0.3138     |      0.2355      |      0.3175      |             0.8634            |             0.8872            |             0.3165             |             1.3024             |
|  6000 |        0.0439       |      0.1539      |         0.1950        |          0.1939         |      0.3091     |      0.3060     |      0.2389      |      0.3127      |             0.8758            |             0.9049            |             0.3284             |             1.3058             |
|  7000 |        0.0437       |      0.1553      |         0.2013        |          0.2055         |      0.3131     |      0.2835     |      0.2403      |      0.3141      |             0.8542            |             0.8989            |             0.3292             |             1.2941             |
|  8000 |        0.0442       |      0.1530      |         0.2084        |          0.2095         |      0.3109     |      0.3096     |      0.2381      |      0.3132      |             0.8804            |             0.9049            |             0.3286             |             1.3383             |
|  9000 |        0.0440       |      0.1552      |         0.2127        |          0.2222         |      0.3107     |      0.2944     |      0.2385      |      0.3035      |             0.8359            |             0.9049            |             0.3171             |             1.2707             |
| 10000 |        0.0439       |      0.1554      |         0.2133        |          0.2274         |      0.3118     |      0.2870     |      0.2385      |      0.3089      |             0.8424            |             0.9023            |             0.3187             |             1.3271             |










