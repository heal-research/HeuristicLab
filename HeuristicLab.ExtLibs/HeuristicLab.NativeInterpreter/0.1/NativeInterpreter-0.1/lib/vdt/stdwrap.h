#ifndef _VDT_STD_H_
#define _VDT_STD_H_

#include <cmath>

// wrapper around std function just to benefit of naming
// to be used in vectorized wrappers

namespace vdt {
  
  template<typename T>
  inline T fast_sqrt(T x) { return std::sqrt(x); }

  template<typename T>
  inline T fast_div(T x,T y) { return x/y; }

  template<typename T>
  inline T fast_fma(T x,T y, T z) { return x*y+z; }

  // correclty rounded
  template<typename T>
  inline T fast_fmac(T x, T y, T z) { return std::fma(x,y,z); }
  
  template<typename T>
  inline T fast_pow(T x, T y) { return std::pow(x, y); }

  template<typename T>
  inline T fast_round(T x) { return std::round(x); }
}
#endif
