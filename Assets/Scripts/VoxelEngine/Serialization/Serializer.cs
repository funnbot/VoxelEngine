using System.Collections.Generic;
using System.IO;
using LZ4;
using VoxelEngine.Data;
using VoxelEngine.Internal;

namespace VoxelEngine.Serialization {

    public static class Serializer {
        public static readonly string SaveFolder = "Worlds";

        public static void SaveChunk(string worldSave, Coord2 pos, Chunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);
            
            using(var stream = new LZ4Stream(File.Open(saveFile, FileMode.Create, FileAccess.Write), LZ4StreamMode.Compress)) {
                using(var writer = new BinaryWriter(stream)) {
                    chunk.Serialize(writer);
                }
            }

            // using(var stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write)) {
            //     using(var writer = new BinaryWriter(stream)) {
            //         chunk.Serialize(writer);
            //     }
            // }
        }

        public static bool IsChunkSaved(string worldSave, Coord2 pos) {
            string saveFile = FolderName(worldSave) + FileName(pos);
            return File.Exists(saveFile);
        }

        public static void LoadChunk(string worldSave, Coord2 pos, Chunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            using(var stream = new LZ4Stream(File.Open(saveFile, FileMode.Open), LZ4StreamMode.Decompress)) {
                using(var reader = new BinaryReader(stream)) {
                    chunk.Deserialize(reader);
                }
            }

            // using(var stream = new FileStream(saveFile, FileMode.Open)) {
            //     using(var reader = new BinaryReader(stream)) {
            //         chunk.Deserialize(reader);
            //     }
            // }
        }

        static string FileName(Coord2 pos) =>
            $"{pos.x},{pos.y}.chunk";

        static string FolderName(string worldSave) {
            string saveLocation = $"{SaveFolder}/{worldSave}/";
            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);
            return saveLocation;
        }
    }

    public enum ReservedBytes : byte {
        Air = 255,
        AllAir = 254,
        AllStone = 253
    }
}