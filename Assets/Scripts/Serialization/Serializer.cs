using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using VoxelEngine;

public static class Serializer {
    public static readonly string SaveFolder = "Worlds";

    public static void SaveColumn(string worldSave, Coord2 pos, SerialChunkColumn column) {
        string saveFile = FolderName(worldSave) + FileName(pos);

        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);

        Debug.Log("Save: " + pos);

        formatter.Serialize(stream, column);
        stream.Close();
    }

    public static bool LoadColumn(string worldSave, Coord2 pos, out SerialChunkColumn column) {
        string saveFile = FolderName(worldSave) + FileName(pos);

        if (!File.Exists(saveFile)) {
            column = null;
            return false;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFile, FileMode.Open);

        Debug.Log("Loaded: " + pos);

        column = (SerialChunkColumn)formatter.Deserialize(stream);
        Debug.Log(column.blocks[0][0][0][0]);
        stream.Close();
        return true;
    }

    public static string FileName(Coord2 pos) =>
        $"{pos.x},{pos.y}.bin";

    public static string FolderName(string worldSave) {
        string saveLocation = $"{SaveFolder}/{worldSave}/";
        if (!Directory.Exists(saveLocation))
            Directory.CreateDirectory(saveLocation);
        return saveLocation;
    }
}