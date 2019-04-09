using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelEngine;
using VoxelEngine.Blocks;

public class PipeObject : MonoBehaviour {
    public GameObject PipeStraight;
    public GameObject PipeCorner;
    public GameObject PipeEnd;

    GameObject current;

    public void SetType(PipeBlock.PipeType type, Coord3 rotation) {
        if (current != null) Destroy(current);

        if (type == PipeBlock.PipeType.Straight) current = PipeStraight;
        else if (type == PipeBlock.PipeType.Corner) current = PipeCorner;
        else if (type == PipeBlock.PipeType.End) current = PipeEnd;

        current = Instantiate(current, transform);
        current.transform.rotation = Quaternion.Euler(rotation * 90);
        current.transform.localPosition = Vector3.zero;
    }
}