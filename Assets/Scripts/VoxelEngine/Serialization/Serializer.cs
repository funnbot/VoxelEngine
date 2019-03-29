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
            MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.StandardResolver.Instance);
            var config = new SerializerConfig();
            //config.KnownTypes.Add(typeof(BlockData));
            serializer = new CerasSerializer(config);
        }

        static byte[] Sbuffer = null;
        static byte[] Dbuffer = null;

        public static void SaveChunk(string worldSave, Coord2 pos, SerialChunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            serializer.Serialize<SerialChunk>(chunk, ref Sbuffer);
            File.WriteAllBytes(saveFile, Sbuffer);

            //using(FileStream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None)) {
            //MessagePackSerializer.Serialize<SerialChunk>(stream, chunk);
            //}
        }

        public static bool LoadChunk(string worldSave, Coord2 pos, out SerialChunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            if (!File.Exists(saveFile)) {
                chunk = null;
                return false;
            }

            Dbuffer = File.ReadAllBytes(saveFile);
            chunk = new SerialChunk();
            serializer.Deserialize<SerialChunk>(ref chunk, Dbuffer);

            /* using(FileStream stream = new FileStream(saveFile, FileMode.Open)) {
                chunk = MessagePackSerializer.Deserialize<SerialChunk>(stream);
            }*/
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

}