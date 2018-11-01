#include <memory>

#include "interpreter.h" 

extern "C" {

// slow (ish?)
__declspec(dllexport) 
double __cdecl GetValue(instruction* code, int codeLength, int row) noexcept
{
    return evaluate(code, codeLength, row);
}

__declspec(dllexport) 
void __cdecl GetValues(instruction* code, int codeLength, int* rows, int totalRows, double* result) noexcept
{
    for (int i = 0; i < totalRows; ++i)
    {
        result[i] = evaluate(code, codeLength, rows[i]);
    }
}

__declspec(dllexport)
void __cdecl GetValuesVectorized(instruction* code, int codeLength, int* rows, int totalRows, double* result) noexcept
{
    std::vector<double[BUFSIZE]> buffers(codeLength);
    // initialize instruction buffers
    for (int i = 0; i < codeLength; ++i)
    {
        instruction& in = code[i];
        in.buf = buffers[i];

        if (in.opcode == OpCodes::Const)
        {
            load(in.buf, in.value);
        }
    }

    int remainingRows = totalRows % BUFSIZE;
    int total = totalRows - remainingRows;

    for (int rowIndex = 0; rowIndex < total; rowIndex += BUFSIZE)
    {
        evaluate(code, codeLength, rows, rowIndex, BUFSIZE);
        std::memcpy(result + rowIndex, code[0].buf, BUFSIZE * sizeof(double));
    }

    // are there any rows left?
    if (remainingRows > 0) {
        for (int rowIndex = total; rowIndex < totalRows; rowIndex += remainingRows)
        {
            evaluate(code, codeLength, rows, rowIndex, remainingRows);
            std::memcpy(result + rowIndex, code[0].buf, remainingRows * sizeof(double));
        }
    }
}

#ifdef __cplusplus
}
#endif
