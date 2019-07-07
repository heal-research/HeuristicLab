#ifndef NATIVE_TREE_INTERPRETER_CLANG_H
#define NATIVE_TREE_INTERPRETER_CLANG_H

#include "vector_operations.h"
#include "instruction.h"

inline double evaluate(instruction *code, int len, int row) noexcept
{
    for (int i = len - 1; i >= 0; --i)
    {
        instruction &in = code[i];
        switch (in.opcode)
        {
            case OpCodes::Const: /* nothing to do */ break;
            case OpCodes::Var:
                {
                    in.value = in.weight * in.data[row];
                    break;
                }
            case OpCodes::Add:
                {
                    in.value = code[in.childIndex].value;
                    for (int j = 1; j < in.narg; ++j)
                    {
                        in.value += code[in.childIndex + j].value;
                    }
                    break;
                }
            case OpCodes::Sub:
                {
                    in.value = code[in.childIndex].value;
                    for (int j = 1; j < in.narg; ++j)
                    {
                        in.value -= code[in.childIndex + j].value;
                    }
                    if (in.narg == 1)
                    {
                        in.value = -in.value;
                    }
                    break;
                }
            case OpCodes::Mul:
                {
                    in.value = code[in.childIndex].value;
                    for (int j = 1; j < in.narg; ++j)
                    {
                        in.value *= code[in.childIndex + j].value;
                    }
                    break;
                }
            case OpCodes::Div:
                {
                    in.value = code[in.childIndex].value;
                    for (int j = 1; j < in.narg; ++j)
                    {
                        in.value /= code[in.childIndex + j].value;
                    }
                    if (in.narg == 1)
                    {
                        in.value = 1 / in.value;
                    }
                    break;
                }
            case OpCodes::Exp:
                {
                    in.value = hl_exp(code[in.childIndex].value);
                    break;
                }
            case OpCodes::Log:
                {
                    in.value = hl_log(code[in.childIndex].value);
                    break;
                }
            case OpCodes::Sin:
                {
                    in.value = hl_sin(code[in.childIndex].value);
                    break;
                }
            case OpCodes::Cos:
                {
                    in.value = hl_cos(code[in.childIndex].value);
                    break;
                }
            case OpCodes::Tan:
                {
                    in.value = hl_tan(code[in.childIndex].value);
                    break;
                }
            case OpCodes::Tanh:
                {
                    in.value = hl_tanh(code[in.childIndex].value);
                    break;
                }
            case OpCodes::Power:
                {
                    double x = code[in.childIndex].value;
                    double y = hl_round(code[in.childIndex + 1].value);
                    in.value = hl_pow(x, y);
                    break;
                }
            case OpCodes::Root:
                {
                    double x = code[in.childIndex].value;
                    double y = hl_round(code[in.childIndex + 1].value);
                    in.value = hl_pow(x, 1 / y);
                    break;
                }
            case OpCodes::Sqrt:
                {
                    in.value = hl_pow(code[in.childIndex].value, 1./2.);
                    break;
                }
            case OpCodes::Square:
                {
                    in.value = hl_pow(code[in.childIndex].value, 2.);
                    break;
                }
            case OpCodes::CubeRoot:
                {
                    in.value = hl_pow(code[in.childIndex].value, 1./3.);
                    break;
                }
            case OpCodes::Cube:
                {
                    in.value = hl_pow(code[in.childIndex].value, 3.);
                    break;
                }
            case OpCodes::Absolute:
                {
                    in.value = std::fabs(code[in.childIndex].value);
                    break;
                }
            case OpCodes::AnalyticalQuotient:
                {
                    double x = code[in.childIndex].value;
                    double y = code[in.childIndex + 1].value;
                    in.value = x / hl_sqrt(1 + y*y);
                    break;
                }
            default: in.value = NAN;
        }
    }
    return code[0].value;
}

inline void load_data(instruction &in, int* __restrict rows, int rowIndex, int batchSize) noexcept
{
    for (int i = 0; i < batchSize; ++i)
    {
        auto row = rows[rowIndex + i];
        in.buf[i] = in.weight * in.data[row];
    }
}

inline void evaluate(instruction* code, int len, int* __restrict rows, int rowIndex, int batchSize) noexcept
{
    for (int i = len - 1; i >= 0; --i)
    {
        instruction &in = code[i];
        switch (in.opcode)
        {
            case OpCodes::Var:
                {
                    load_data(in, rows, rowIndex, batchSize); // buffer data
                    break;
                }
            case OpCodes::Const: /* nothing to do because buffers for constants are already set */ break;
            case OpCodes::Add:
                {
                    load(in.buf, code[in.childIndex].buf);
                    for (int j = 1; j < in.narg; ++j)
                    {
                        add(in.buf, code[in.childIndex + j].buf);
                    }
                    break;
                }
            case OpCodes::Sub:
                {
                    if (in.narg == 1)
                    {
                        neg(in.buf, code[in.childIndex].buf);
                        break;
                    }
                    else
                    {
                        load(in.buf, code[in.childIndex].buf);
                        for (int j = 1; j < in.narg; ++j)
                        {
                            sub(in.buf, code[in.childIndex + j].buf);
                        }
                    }
                    break;
                }
            case OpCodes::Mul:
                {
                    load(in.buf, code[in.childIndex].buf);
                    for (int j = 1; j < in.narg; ++j)
                    {
                        mul(in.buf, code[in.childIndex + j].buf);
                    }
                    break;
                }
            case OpCodes::Div:
                {
                    if (in.narg == 1)
                    {
                        inv(in.buf, code[in.childIndex].buf);
                        break;
                    }
                    else
                    {
                        load(in.buf, code[in.childIndex].buf);
                        for (int j = 1; j < in.narg; ++j)
                        {
                            div(in.buf, code[in.childIndex + j].buf);
                        }
                    }
                    break;
                }
            case OpCodes::Sin:
                {
                    sin(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::Cos:
                {
                    cos(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::Tan:
                {
                    tan(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::Tanh:
                {
                    tanh(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::Log:
                {
                    log(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::Exp:
                {
                    exp(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::Power:
                {
                    load(in.buf, code[in.childIndex].buf);
                    pow(in.buf, code[in.childIndex + 1].buf);
                    break;
                }
            case OpCodes::Root:
                {
                    load(in.buf, code[in.childIndex].buf);
                    root(in.buf, code[in.childIndex + 1].buf);
                    break;
                }
            case OpCodes::Square:
                {
                    pow(in.buf, code[in.childIndex].buf, 2.);
                    break;
                }
            case OpCodes::Sqrt:
                {
                    pow(in.buf, code[in.childIndex].buf, 1./2.);
                    break;
                }
            case OpCodes::CubeRoot:
                {
                    pow(in.buf, code[in.childIndex].buf, 1./3.);
                    break;
                }
            case OpCodes::Cube:
                {
                    pow(in.buf, code[in.childIndex].buf, 3.);
                    break;
                }
            case OpCodes::Absolute:
                {
                    abs(in.buf, code[in.childIndex].buf);
                    break;
                }
            case OpCodes::AnalyticalQuotient:
                {
                    load(in.buf, code[in.childIndex].buf);
                    analytical_quotient(in.buf, code[in.childIndex + 1].buf);
                    break;
                }
            default: load(in.buf, NAN);
            }
    }
}

#endif
