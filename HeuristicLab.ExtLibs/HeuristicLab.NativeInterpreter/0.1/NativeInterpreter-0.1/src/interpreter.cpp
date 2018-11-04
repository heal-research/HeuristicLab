#include "interpreter.h" 

#ifdef __cplusplus
extern "C" {
#endif

constexpr size_t BUFSIZE = BATCHSIZE * sizeof(double);

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
void __cdecl GetValuesVectorized(instruction* code, int codeLength, int* rows, int totalRows, double* __restrict result) noexcept
{
    double* buffer = static_cast<double*>(_aligned_malloc(codeLength * BUFSIZE, 16));
    for (int i = 0; i < codeLength; ++i)
    {
        instruction& in = code[i];
        in.buf = buffer + (i * BATCHSIZE);

        if (in.opcode == OpCodes::Const)
        {
            load(in.buf, in.value);
        }
    }

    int remainingRows = totalRows % BATCHSIZE;
    int total = totalRows - remainingRows;

    for (int rowIndex = 0; rowIndex < total; rowIndex += BATCHSIZE)
    {
        evaluate(code, codeLength, rows, rowIndex, BATCHSIZE);
        std::memcpy(result + rowIndex, code[0].buf, BUFSIZE);
    }

    // are there any rows left?
    if (remainingRows > 0) 
    {
        for (int rowIndex = total; rowIndex < totalRows; rowIndex += remainingRows)
        {
            evaluate(code, codeLength, rows, rowIndex, remainingRows);
            std::memcpy(result + rowIndex, code[0].buf, remainingRows * sizeof(double));
        }
    }
    _aligned_free(buffer);
}

#ifdef __cplusplus
}
#endif
