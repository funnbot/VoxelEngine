[Back To Index](../index.md)

## Week 3/11

> A proposed name for the game is Factor3 Cubed (Factory³ / Factory^3 / Factor³) and a logo was made (not by me)


![Factor3Cubed](../images/factor3logo.png)

### Challenges
##### Block rotation
> Block rotation now works as intented, it takes in a Coord3 rotation value, similar to eulerAngles in the inspector, except divided by 90 since blocks can only be rotated by 90 degress to maintain symmetry. Given that rotation, and the current face of the cube that is being draw, it outputs the texture that should be placed on that face, and the rotation of the UVs for that face. The end result turned out a bit more hardcoded than as an agorithm, but it still functions.

```cs
private static readonly int[] zRot = { BlockFace.top, BlockFace.left, BlockFace.bottom, BlockFace.right },
    yRot = { BlockFace.front, BlockFace.right, BlockFace.back, BlockFace.left },
    xRot = { BlockFace.front, BlockFace.top, BlockFace.back, BlockFace.bottom };
private static readonly int[][] zFace = { new [] { 3, 1, 3, 3, 3, 3 }, new [] { 2, 2, 2, 2, 2, 2, }, new [] { 1, 3, 1, 1, 1, 1 } },
    yFace = { new [] { 0, 0, 3, 1, 0, 0 }, new [] { 0, 0, 2, 2, 0, 0 }, new [] { 0, 0, 1, 3, 0, 0, } },
    xFace = { new [] { 0, 2, 0, 2, 1, 3 }, new [] { 2, 2, 0, 2, 2, 2 }, new [] { 0, 2, 2, 0, 3, 1 } };
public static(int index, int face) Rotate(int dir, Coord3 rot) {
    rot = rot % 4;
    int frot = 0;
    if (rot.z != 0) {
        if (dir != BlockFace.front && dir != BlockFace.back) {
            var ind = zRot.IndexOf(dir);
            dir = zRot[(rot.z + ind + 4) % 4];
        }
        frot += zFace[rot.z < 0 ? 3 + rot.z : rot.z - 1][dir];
    }
    if (rot.y != 0) {
        if (dir != BlockFace.top && dir != BlockFace.bottom) {
            var ind = yRot.IndexOf(dir);
            dir = yRot[(rot.y + ind + 4) % 4];
        }
        frot += yFace[rot.y < 0 ? 3 + rot.y : rot.y - 1][dir];
    }
    if (rot.x != 0) {
        if (dir != BlockFace.right && dir != BlockFace.left) {
            var ind = xRot.IndexOf(dir);
            dir = xRot[(rot.x + ind + 4) % 4];
        }
        frot += xFace[rot.x < 0 ? 3 + rot.x : rot.x - 1][dir];
    }
    return (index: dir, face: frot);
}
```

##### Sub meshing and blocks without collision
> Previously I had a game object for each mesh that was being generated, this meant that there was two objects with mesh renderers for opaque and transparent blocks, and a object for the block collider. Now, both opaque and transparent blocks share a mesh renderer using submeshing. Two shaders are assigned to the material, and two triangles arrays are stored. The same vertices are used for both, but they are used by different triangle indicies. The other issue was that objects without collision would have no ability to be interacted with by raycasts. Now, a new trigger collider is generated for these blocks, which will not collide with the player, but still get hit by a raycast.


##### Chunk loading
> Chunk loading around the player needs to be able to predict where they will be, while not loading too many chunks to drop framerate. 

```cs
void SpiralOut(ref Coord2 c) {
    int x = c.x, y = c.y;
    if (x >= 0 && y == 0) y++;

    else if (x > 0 && y >= 0) {
        x--;
        y++;
    } else if (x <= 0 && y > 0) {
        x--;
        y--;
    } else if (x < 0 && y <= 0) {
        x++;
        y--;
    } else if (x >= 0 && y < 0) {
        x++;
        y++;
    }

    c.x = x;
    c.y = y;
}
```

> Chunks are not considered chunk columns, since in general a column of chunks is easier to work with and will always be loaded, generated, but not rendered together. By spiralling outward from the player when they enter a new chunk, it will build the map outwards. One problem of chunks in general is when one chunk is rendered, the 4 chunks around it needs to be proceduraly generated aswell so it knows which faces to cull. And.. the 4 chunks around those built chunks also need to be atleast loaded because some structures go outside the bounds of a chunk, like trees, and need to be able to place blocks in those chunks. This means that 13 chunk columns need to be loaded, 5 need to be built, each time you are rendering one chunk. These will be reduced some because in general chunks are generated next to eachother, but it is still alot. By dividing these with coroutines waiting for a frame it can reduce the frame stutter, but also it will take longer for chunks to load.

##### Threading
> The initial tries with threading have shown that doing the procedural generation in a seperate thread because it takes the longest from all the simplexnoise speeds up spawn generation and should minimize the lag between frames when loading new chunks. The procedural mesh generation or render that is done each time a block is placed can also be placed in a seperate thread, although currently there is almost no visible lag.


### Log
- Block behaviours are functioning allowing for an ECS approach to the tick updates to each chunk. Instead of each block having its own update method, the ie. grass block will all be assigned to one block behaviour which will run through the list of all of them, and turn them to dirt or other actions.
- The interface for each block data type allows for the implementation of a GUI, when clicking on a block, that method is called in the OnGUI event which allows for unities basic GUI functions, instead of making my own.
- Coord3's now have their own property drawers similar to Vector3s


#### Next week
> Serializing chunks and saving them instead of re generating them each time they are loaded.