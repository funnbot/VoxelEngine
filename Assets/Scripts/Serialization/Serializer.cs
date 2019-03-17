using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using VoxelEngine;

public static class Serializer {
    public static readonly string SaveFolder = "Worlds";

    public static void SaveColumn(string worldSave, SerialChunkColumn column) {
        string saveFile = FolderName(worldSave) + FileName(column.position);

        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);

        formatter.Serialize(stream, column);
        stream.Close();
    }

    public static bool LoadColumn(string worldSave, Coord2 pos, ref SerialChunkColumn column) {
        column = null;
        string saveFile = FolderName(worldSave) + FileName(pos);

        if (!File.Exists(saveFile)) return false;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFile, FileMode.Open);

        column = (SerialChunkColumn)formatter.Deserialize(stream);
        stream.Close();
        return true;
    }

    public static string FileName(Coord2 pos) =>
        $"{pos.x},{pos.y}.bin";

    public static string FolderName(string worldSave) {
        string saveLocation = $"{Application.persistentDataPath}/{SaveFolder}/{worldSave}/";
        if (!Directory.Exists(saveLocation))
            Directory.CreateDirectory(saveLocation);
        return saveLocation;
    }
}