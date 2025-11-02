# Rabbit Game
Jonathan Q's side project, project started on June 8th 2025

Unity Editor version 6000.2.9f1

## Notable Systems

### FIMF 3D Modeling Format
- Proprietary 3D modeling file format that better translates from 3D modeling software such as Blockbench into Unity 3D's archetecture. Designed for blocky art styles it supports 3D meshes, UV mapping, vertex normals and animation sequences all in one text based file format. See the [FIMF Folder](./FIMF/) for code integrating FIMF format with Unity Engine.

### Multithreaded A-Star Path Finding
- An A-Star path finding system built with multi-threading in mind to allow the user to off-load expensive path finding computation from the main thread. This has the benefit of eliminating frame drops during expensive computations. The system is primarily designed for NPC path finding.

### Model-View-Controller Game Item System
- The game item's system for things such books, sword and coins are designed following the Model-View-Controller archetecture. This allows for clean seperation of complexity between other systems such as user interface and game logic.   

## Screen Shots
![./Previews/Screenshot%202025-07-13%20193226.png](./Previews/Screenshot%202025-07-13%20193226.png)

![./Previews/Screenshot%202025-07-13%20193226.png](./Previews/Screenshot%202025-09-30%20121532.png)

![./Previews/Screenshot%202025-11-01%20190603.png](./Previews/Screenshot%202025-11-01%20190603.png)
