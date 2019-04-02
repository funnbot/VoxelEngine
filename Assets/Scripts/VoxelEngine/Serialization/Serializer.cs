using System.Collections.Generic;
using System.IO;
using Ceras;
using MessagePack;
using VoxelEngine.Data;

namespace VoxelEngine.Serialization {

    public static class Serializer {
        public static readonly string SaveFolder = "Worlds";

        static CerasSerializer serializer;

        static Serializer() {
            //MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.StandardResolver.Instance);
            var config = new SerializerConfig();
            //config.KnownTypes.Add(typeof(BlockData));
            serializer = new CerasSerializer(config);
        }

        static byte[] Sbuffer = null;
        static byte[] Dbuffer = null;

        public static void SaveChunk(string worldSave, Coord2 pos, SerialChunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            int length = serializer.Serialize<SerialChunk>(chunk, ref Sbuffer);
            WriteAllBytes(saveFile, length);
        }

        public static bool IsChunkSaved(string worldSave, Coord2 pos) {
            string saveFile = FolderName(worldSave) + FileName(pos);
            return File.Exists(saveFile);
        }

        public static void LoadChunk(string worldSave, Coord2 pos, ref SerialChunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            ReadAllBytes(saveFile);
            serializer.Deserialize<SerialChunk>(ref chunk, Dbuffer);
        }

        static void ReadAllBytes(string saveFile) {
            using(FileStream fs = new FileStream(saveFile, FileMode.Open)) {
                int length = (int) fs.Length;
                System.Array.Resize(ref Dbuffer, length);
                fs.Read(Dbuffer, 0, length);
            }
        }

        static void WriteAllBytes(string saveFile, int length) {
            using(FileStream fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write)) {
                fs.Write(Sbuffer, 0, length);
            }
        }

        static string FileName(Coord2 pos) =>
            $"{pos.x},{pos.y}.bin";

        static string FolderName(string worldSave) {
            string saveLocation = $"{SaveFolder}/{worldSave}/";
            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);
            return saveLocation;
        }
    }

}