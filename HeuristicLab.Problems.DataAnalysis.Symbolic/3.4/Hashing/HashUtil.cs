#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Security.Cryptography;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class HashUtil {
    // This class contains some hash functions adapted from http://partow.net/programming/hashfunctions/index.html#AvailableHashFunctions

    // A simple hash function from Robert Sedgwicks Algorithms in C book.I've added some simple optimizations to the algorithm in order to speed up its hashing process. 
    public static int RSHash(int[] input) {
      const int b = 378551;
      int a = 63689;
      int hash = 0;

      foreach (var v in input) {
        hash = (hash * a) + v;
        a *= b;
      }
      return hash;
    }

    // A bitwise hash function written by Justin Sobel
    public static int JSHash(int[] input) {
      int hash = 1315423911;
      for (int i = 0; i < input.Length; ++i)
        hash ^= (hash << 5) + input[i] + (hash >> 2);
      return hash;
    }

    // This hash function comes from Brian Kernighan and Dennis Ritchie's book "The C Programming Language". It is a simple hash function using a strange set of possible seeds which all constitute a pattern of 31....31...31 etc, it seems to be very similar to the DJB hash function. 
    public static int BKDRHash(int[] input) {
      const int seed = 131;
      int hash = 0;
      foreach (var v in input) {
        hash = (hash * seed) + v;
      }
      return hash;
    }

    // This is the algorithm of choice which is used in the open source SDBM project. The hash function seems to have a good over-all distribution for many different data sets. It seems to work well in situations where there is a high variance in the MSBs of the elements in a data set. 
    public static int SDBMHash(int[] input) {
      int hash = 0;
      foreach (var v in input) {
        hash = v + (hash << 6) + (hash << 16) - hash;
      }
      return hash;
    }

    // An algorithm produced by Professor Daniel J. Bernstein and shown first to the world on the usenet newsgroup comp.lang.c. It is one of the most efficient hash functions ever published. 
    public static int DJBHash(int[] input) {
      int hash = 5381;
      foreach (var v in input) {
        hash = (hash << 5) + hash + v;
      }
      return hash;
    }

    // An algorithm proposed by Donald E.Knuth in The Art Of Computer Programming Volume 3, under the topic of sorting and search chapter 6.4. 
    public static int DEKHash(int[] input) {
      int hash = input.Length;
      foreach (var v in input) {
        hash = (hash << 5) ^ (hash >> 27) ^ v;
      }
      return hash;
    }

    public static int CryptoHash(HashAlgorithm ha, int[] input) {
      return BitConverter.ToInt32(ha.ComputeHash(input.ToByteArray()), 0);
    }

    public static byte[] ToByteArray(this int[] input) {
      var bytes = new byte[input.Length * sizeof(int)];
      int pos = 0;
      foreach (var v in input) {
        var b0 = (byte)((v >> 24) & 0xFF);
        var b1 = (byte)((v >> 16) & 0xFF);
        var b2 = (byte)((v >> 8) & 0xFF);
        var b3 = (byte)(v & 0xFF);
        bytes[pos++] = b0;
        bytes[pos++] = b1;
        bytes[pos++] = b2;
        bytes[pos++] = b3;
      }
      return bytes;
    }
  }
}
