using System.IO;
using MessagePack;

namespace VoxelEngine.Serialization {

    public static class Serializer {
        public static readonly string SaveFolder = "Worlds";

        public static void SaveColumn(string worldSave, Coord2 pos, SerialChunkColumn column) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            using(FileStream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None)) {
                MessagePackSerializer.Serialize<SerialChunkColumn>(stream, column);
            }
        }

        public static bool LoadColumn(string worldSave, Coord2 pos, out SerialChunkColumn column) {
            string saveFile = FolderName(worldSave) + FileName(pos);

            if (!File.Exists(saveFile)) {
                column = null;
                return false;
            }

            using(FileStream stream = new FileStream(saveFile, FileMode.Open)) {
                column = MessagePackSerializer.Deserialize<SerialChunkColumn>(stream);
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