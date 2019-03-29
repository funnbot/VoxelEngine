using System.IO;
using MessagePack;

namespace VoxelEngine.Serialization {

    public static class Serializer {
        public static readonly string SaveFolder = "Worlds";

        static Serializer() {
            MessagePackSerializer.SetDefaultResolver(MessagePack.Resolvers.StandardResolver.Instance);
        }

        public static void SaveChunk(string worldSave, Coord2 pos, SerialChunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            using(FileStream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None)) {
                MessagePackSerializer.Serialize<SerialChunk>(stream, chunk);
            }
        }

        public static bool LoadChunk(string worldSave, Coord2 pos, out SerialChunk chunk) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            if (!File.Exists(saveFile)) {
                chunk = null;
                return false;
            }

            using(FileStream stream = new FileStream(saveFile, FileMode.Open)) {
                chunk = MessagePackSerializer.Deserialize<SerialChunk>(stream);
            }
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