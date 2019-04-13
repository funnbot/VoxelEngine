using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VoxelEngine.Interfaces;

public static class ArrayExtensions {
    public static int IndexOf(this int[] arr, int value) {
        int len = arr.Length;
        for (int i = 0; i < len; i++) {
            if (arr[i] == value) return i;
        }
        return -1;
    }

    public static void Initialize<T>(this T[][] array, int sizeX, int sizeY) {
        for (int x = 0; x < sizeX; x++) {
            
        }
    }

    public static void Serialize(this ISerializeable[] array, BinaryWriter writer) {
        for (int i = 0; i < array.Length; i++) array[i]?.Serialize(writer);
    }

    public static void Deserialize(this ISerializeable[] array, BinaryReader reader) {
        for (int i = 0; i < array.Length; i++) array[i]?.Deserialize(reader);
    }

    public static void Serialize(this ISerializeable[][] array, BinaryWriter writer) {
        for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
                array[x]?[y]?.Serialize(writer);
            }
        }
    }

    public static void Deserialize(this ISerializeable[][] array, BinaryReader reader) {
        for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
                array[x]?[y]?.Deserialize(reader);
            }
        }
    }

    public static void Serialize(this ISerializeable[][][] array, BinaryWriter writer) {
        for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
                for (int z = 0; z < array.GetLength(2); z++) {
                    array[x]?[y]?[z]?.Serialize(writer);
                }
            }
        }
    }

    public static void Deserialize(this ISerializeable[][][] array, BinaryReader reader) {
        for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
                for (int z = 0; z < array.GetLength(2); z++) {
                    array[x]?[y]?[z]?.Deserialize(reader);
                }
            }
        }
    }
}