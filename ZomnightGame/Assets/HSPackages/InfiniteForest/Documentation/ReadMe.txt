-Copyright Highwalker Studios 2016
-Created by Luc Highwalker

------------------------------------------------------------
0. INDEX:
------------------------------------------------------------

1. Installation.
2. How to use.
3. Variables explained.
4. Enjoy, Thanks!


------------------------------------------------------------
1. INSTALLATION:
------------------------------------------------------------

- Make sure that the Spawner and ForestHandler scripts are above the default time of your script 
	execution order. To do this, go to Edit -> Project Settings -> Script Execution Order. 
	Then click the + symbol, and select the Spawner and ForestHandler scripts. Drag the scripts 
	so that they are above the Default Time block in their respective order. Spawner first, then
	ForestHandler.

- Drop the ForestHandler prefab into your scene. 

- Attach the Spawner prefab to your main camera. Attach a Rigidbod2D to the same camera, and change 
	the gravity scale to 0.

- Configure the handler, or use the default settings.

------------------------------------------------------------
2. HOW TO USE:
------------------------------------------------------------
	
- For best results, make sure that the tree's shadows are on a layer above your
	characters'. Adjust other tree parts as needed.
	
- For a tutorial on how to make your own trees, check out my website at 
	http://www.highwalkerstudios.com

- If the camera moves to fast, it can happen that some trees and rocks end up out of bounds.
	If this is the case for your game, turn on CheckForStrags in the ForestHandler's
	"Misc" options.

- If trees are popping into view of the camera, you may need to adjust the Spawner's 
	"Cam Size Mod" variable.

- This asset is configured to work out of the box. If it is not functioning after following 
	the above instructions. Please feel free to send me an e-mail at luc@highwalkerstudios.com. 
	Please attach your current project file.

** To increase the spawner's size. Modify the offset of each spawner wall's collider (children of 
	the Spawner object). Adjust the collider's size so that all the colliders form a square 
	around the camera. Then change the Spawner's spawnerSize variable, so that object's spawn 
	on inner edge of said square. To easily determine the value of this variable. Place the 
	camera at a zero coordinate. Then create an empty object, and place it along the inner edge 
	of the collider formed square. Take the highest coordinate number and subtract 1. The result
	is the new spawner size value. 

------------------------------------------------------------
3. VARIABLES EXPLAINED
------------------------------------------------------------

-SPAWNER
--------------------------

- Spawner Size: The amount of units that the spawner can use to spawn objects.
	So a size of 19, will allow for coordinates of -19 to 19 in addition to the 
	spawners current coordinates. Only change this if you modify the overall size
	of the spawner. **

- Cam Size Mod: A modifier for the camera size. Increase this variable if objects pop
	into view.
	
	
- FOREST HANDLER
--------------------------

DENSITY SETTINGS:

- Tree Density: The amount of trees which are spawned.

- Rock Density: Same as above but for rocks.	


TREE SETTINGS:

- Tree Quality: The quality that the Trees will be set to.
		4: Uses high quality tree animations.
		3: Uses low quality tree animations.
		2: Tree animations are disabled.
		1: Tree animations and stumps are disable.
		0: Tree aniamtions, stumps, and shadows are disabled.


- Fadeable: If true, the trees will fade away to see what is underneath.

- Fade Distance: The distance from the center of the screen that the trees have to be in 
	order to fade away.

- Tree Fade Alpha: The minimum alpha that the trees reach when fading away.

- Fade Speed: This variable controls how quickly the trees fade away when near them, if fadeable.

- Stump Alpha: The alpha level of the leaveless tree sprites, visible when faded away and quality is 
	2 or higher.


- Tree Size Min: The minimum size that the trees will spawn in.

- Tree Size Max: Opposite from the above.

- Tree Mod: This variable controls how much the trees pan acording to the camera's position.


ROCK SETTINGS:

- Rock Size Min: The minimum size that the rocks will spawn in.

- Rocks Size Max: The maximum size that the rocks will spawn in.


PREFABS:
	
- Tree Prefabs: The Tree prefabs which are spawnable. 

- Rocks Prefabs: Same as above but for rocks.	


MISC:

- Check For Strags: Whether the forest handler should regularly check for stragglers.

- Strag Check: The amount of seconds between each straggler check.

- Strag Dist: The distance at which a tree or rock is considered a straggler.
	

------------------------------------------------------------
4. THANK YOU!
------------------------------------------------------------


Thank you so much for the download! Every download is a huge help! If you need help using this asset, 
feel free to contact me at luc@highwalkerstudios.com.

Thanks again! :)