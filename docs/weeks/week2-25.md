[Back To Index](../index.md)

## Week 2/25
- [ ] Complete

### Initial Task
> Convert chunk dict into columns storing vector2
It has become messy dealing with the conversion of vector2 and 3

#### Sub  Tasks
- [ ] Chunk has too many public props, isolate and hide.
- [ ] Chunk/Chunkcolumn pooling

### Challenges
- Reading forums on prefabs vs scratch building in script made me realize i should be pooling chunks instead of instantiating new ones and destroying, will do this week or next. Possibly make a chunkColumn pool, would also clean up if chunk columns were monobehaviours instead of structs because could specify coordinate in name. The chunkcolumns would then instantiate a number of chunks as children which would be constant size since world size wont change, base it on the chunkHeight const val.
- The StandardArray shader does not handle shadows correctly

### Log
- Tuesday
  - Created a chunk column class to handle columns since they should all be instantiated seperately, chunks cannot be combined because time it takes to regenerate mesh.
  - Move the VoxelWorld chunk handling into chunk column
  - Creating a PrefabPool behaviour
- Wednesday
  - Setup a github pages for weekly notes to easily keep track and share them.
  - Converting to ChunkColumns
- Thursday
  - Creating a Coord2 and Coord3 to replace Vector3Int usage
- Saturday
  - Created working build
  - Converted all Uses of vector3/2int to a coord3/2int
  - Converting the ChunkColumns to be pooled

#### Notes For Next Week