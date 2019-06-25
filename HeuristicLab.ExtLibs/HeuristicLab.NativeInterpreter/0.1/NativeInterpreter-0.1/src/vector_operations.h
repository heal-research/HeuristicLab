#ifndef VECTOR_OPERATIONS_H
#define VECTOR_OPERATIONS_H

#define _USE_MATH_DEFINES
#include <cmath>
#include <cstring>

#ifdef USE_VDT
#include "vdt/vdtMath.h"
#include "vdt/stdwrap.h"
#define hl_exp vdt::fast_exp
#define hl_log vdt::fast_log
#define hl_sin vdt::fast_sin
#define hl_cos vdt::fast_cos
#define hl_tan vdt::fast_tan
#define hl_tanh vdt::fast_tanh
#define hl_sqrt vdt::fast_sqrt
#define hl_pow vdt::fast_pow
#define hl_cbrt std::cbrt
#define hl_round vdt::fast_round
#define hl_inv vdt::fast_inv
#else
#define hl_exp std::exp
#define hl_log std::log
#define hl_sin std::sin
#define hl_cos std::cos
#define hl_tan std::tan
#define hl_tanh std::tanh
#define hl_sqrt std::sqrt
#define hl_pow std::pow
#define hl_cbrt std::cbrt
#define hl_round std::round
#define hl_inv(x) 1. / x;

#endif

constexpr int BATCHSIZE = 64;

#define FOR(i) for(int i = 0; i < BATCHSIZE; ++i)

// When auto-vectorizing without __restrict,
// gcc and clang check for overlap (with a bunch of integer code)
// before running the vectorized loop

// vector - vector operations
inline void load(double* __restrict a, double const * __restrict b) noexcept { std::memcpy(a, b, BATCHSIZE * sizeof(double)); }
inline void add(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] += b[i]; }
inline void sub(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] -= b[i]; }
inline void mul(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] *= b[i]; }
inline void div(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] /= b[i]; }
inline void exp(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_exp(b[i]); }
inline void log(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_log(b[i]); }
inline void sin(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_sin(b[i]); }
inline void cos(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_cos(b[i]); }
inline void tan(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_tan(b[i]); }
inline void tanh(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_tanh(b[i]); }
inline void sqrt(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_sqrt(b[i]); }
inline void pow(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_pow(a[i], hl_round(b[i])); };
inline void root(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_pow(a[i], 1. / hl_round(b[i])); };
inline void cbrt(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_cbrt(b[i]); };
inline void square(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_pow(b[i], 2.); };
inline void inv(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = hl_inv(b[i]); }
inline void neg(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = -b[i]; }
inline void abs(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] = std::fabs(b[i]); }
inline void analytical_quotient(double* __restrict a, double const * __restrict b) noexcept { FOR(i) a[i] /= hl_sqrt(b[i]*b[i] + 1.); }

// vector - scalar operations
inline void load(double* __restrict a, double s) noexcept { FOR(i) a[i] = s; }
inline void add(double* __restrict a, double s) noexcept { FOR(i) a[i] += s; }
inline void sub(double* __restrict a, double s) noexcept { FOR(i) a[i] -= s; }
inline void mul(double* __restrict a, double s) noexcept { FOR(i) a[i] *= s; }
inline void div(double* __restrict a, double s) noexcept { FOR(i) a[i] /= s; }
inline void pow(double* __restrict dst, double const * __restrict src, double s) noexcept { FOR(i) dst[i] = hl_pow(src[i], s); }

// vector operations
inline void neg(double* __restrict a) noexcept { FOR(i) a[i] = -a[i]; }
inline void inv(double* __restrict a) noexcept { FOR(i) a[i] = hl_inv(a[i]); }
inline void exp(double* __restrict a) noexcept { FOR(i) a[i] = hl_exp(a[i]); }
inline void log(double* __restrict a) noexcept { FOR(i) a[i] = hl_log(a[i]); }
inline void sin(double* __restrict a) noexcept { FOR(i) a[i] = hl_sin(a[i]); }
inline void cos(double* __restrict a) noexcept { FOR(i) a[i] = hl_cos(a[i]); }
inline void tan(double* __restrict a) noexcept { FOR(i) a[i] = hl_tan(a[i]); }
inline void tanh(double* __restrict a) noexcept { FOR(i) a[i] = hl_tanh(a[i]); }
inline void round(double* __restrict a) noexcept { FOR(i) a[i] = hl_round(a[i]); }
inline void square(double* __restrict a) noexcept { FOR(i) a[i] = hl_pow(a[i], 2.); }
inline void cbrt(double* __restrict a) noexcept { FOR(i) a[i] = hl_cbrt(a[i]); }

#undef FOR
#endif
