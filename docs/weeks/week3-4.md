[Back To Index](../index.md)

## Week 3/4
- [ ] Complete

### Initial Task
> Implement rotation of blocks when generating the mesh

### Challenges
- How should rotations be stored and handled
> A Coord3 thats 0-3 for each, x y z just like eulerAngles
```csharp
private int RotateInt(int dir, int rot, int i) {
    int[] xRot = { FaceDir.up, FaceDir.forward, FaceDir.down, FaceDir.backward }; // Every possible rotation of the X axis. 
    int[] yRot = { FaceDir.right, FaceDir.forward, FaceDir.left, FaceDir.backward };
    int[] zRot = { FaceDir.up, FaceDir.right, FaceDir.down, FaceDir.left };

    if (i == 0 && x != 0) {
        if (dir < 2 || dir > 3) { // The 2 and 3 directions are the faces that are rotated, rather than their texture index.
            var ind = xRot.TakeWhile(n => n != dir).Count(); // This is just an IndexOf();
            return xRot[(ind + rot) % 4]; // Modulus is simply a wrap around, instead of 0 1 2 3 4 5 6 its 0 1 2 3 0 1 2
        }
    } else if (i == 1 && y != 0) {
        if (dir > 1) {
            var ind = yRot.TakeWhile(n => n != dir).Count();
            return yRot[(ind + rot) % 4];
        }
    } else if (i == 2 && z != 0) {
        if (dir < 4) {
            var ind = zRot.TakeWhile(n => n != dir).Count();
            return zRot[(ind + rot) % 4];
        }
    }
    return dir;
}
```
> So far it still is not functioning, but it is closer to the goal.
> This current implementation defines 3 rotations arrays, which are basically the 4 possible directions of an axis, based on which axis im rotating around I then find the index of the current rotation in that array, and shift it along the array based on the rotation amount, this is a conversion between my Vector3 rotation value, and the 0 - 5 directional values for a cube. The returned value is a index of the texture array. So, when generating the 0-upwards face, it runs that through this rotation to find the texture that should be drawn there. 
> Along with index rotation there is also face rotation, which is the clockwise rotation of the UVs to rotate the texture on a face, this is definied by the 2 faces that are not rotated when rotating around an axis, for the x axis this would be the left and right faces, they rotate forward, but do not move texture indices. This is fairly simply as I simply return the rotation value if the in direction is one of the non moving sides.

> A possible fix for the current issues is to do one full rotation of the texture indicies and faces for each axis, there might be an issue since I am returning the fully rotated directions to the facerotation function which is then rotating the faces incorrectly. Because... R(1, 1, 0), rotates the x axis 1 and would expect face rotations, yet the only rotation it sees are the 1 y rotation because then the face rotations are considered.