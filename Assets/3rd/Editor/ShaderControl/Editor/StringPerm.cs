using UnityEngine;
using System.Collections.Generic;

namespace ShaderControl {

    public class StringPerm {

        public static List<List<string>> GetCombinations(List<string> items) {
            List<List<string>> variants = new List<List<string>>();
            int bitCount = items.Count;
            int mask = (int)Mathf.Pow(2, bitCount);
            for (int k = 1; k < mask; k++) {
                List<string> variant = new List<string>();
                int bit = 1;
                for (int j = 0; j < bitCount; j++, bit <<= 1) {
                    if ((k & bit) != 0) {
                        variant.Add(items[j]);
                    }
                }
                variants.Add(variant);
            }
            return variants;
        }

    }
}