using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    public static int IndexOf(this int[] arr, int value) {
        int len = arr.Length;
        for (int i = 0; i < len; i++) {
            if (arr[i] == value) return i;
        }
        return -1;
    }
}