#ifndef INSTRUCTION_H
#define INSTRUCTION_H

#include <cstdint>

#include "vector_operations.h"

enum OpCodes : uint8_t
{
    // same values as in OpCodes.cs
    Add                = 1,
    Sub                = 2,
    Mul                = 3,
    Div                = 4,
    Sin                = 5,
    Cos                = 6,
    Tan                = 7,
    Log                = 8,
    Exp                = 9,
    Var                = 18,
    Const              = 20,
    Power              = 22,
    Root               = 23,
    Square             = 28,
    Sqrt               = 29,
    Absolute           = 48,
    AnalyticalQuotient = 49,
    Cube               = 50,
    CubeRoot           = 51,
    Tanh               = 52
};

struct instruction
{
    // from Instruction.cs
    uint8_t opcode;
    uint16_t narg;

    int childIndex;

    // from LinearInstruction.cs
    double value;
    double weight; // necessary for variables

    // pointer to data
    double *buf;
    double *data;
};

#endif
