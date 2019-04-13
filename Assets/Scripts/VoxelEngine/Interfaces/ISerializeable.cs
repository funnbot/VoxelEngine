using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public interface ISerializeable {
    void Serialize(BinaryWriter writer);
    void Deserialize(BinaryReader reader);
}