//Copyright Highwalker Studios 2016
//Author: Luc Highwalker
//package: 2D Infinite Forest

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForestHandler : MonoBehaviour {
	public static ForestHandler Handler { get; private set; }

	[Header ("Density Settings:")] 

	[Tooltip ("The number of trees which are spawned.")]
	/// <summary>
	/// The number of trees which are spawned.
	/// </summary>
	[SerializeField] int treeDensity; 

	[Tooltip ("The number of rocks which are spawned.")]
	/// <summary>
	/// The number of rocks which are spawned.
	/// </summary>
	[SerializeField] int rockDensity;

	[Header ("Tree Settings:")]

	[Tooltip ("The quality of the trees.\n" +
		"4: Uses high quality tree animations.\n" +
		"3: Uses low quality tree animations.\n" +
		"2: Tree animations are disabled.\n" +
		"1: Tree animations and stumps are disable.\n" +
		"0: Tree aniamtions, stumps, and shadows are disabled.")]
	/// <summary>
	/// The quality of the trees.
	/// </summary>
	[Range(0, 4)] public int treeQuality;

	[Space]

	[Tooltip ("Whether or not the tree's fade from view in the center of the screen.")]
	/// <summary>
	/// Whether or not the tree's fade from view in the center of the screen.
	/// </summary>
	[SerializeField] bool fadeable;

	[Tooltip ("The distance from the center at which trees should fade from view.")]
	/// <summary>
	/// The distance from the center at which trees should fade from view.
	/// </summary>
	[SerializeField] float fadeDistance;

	[Tooltip ("The target alpha values for faded away trees.")]
	/// <summary>
	/// The target alpha values for faded away trees.
	/// </summary>
	[SerializeField] float treeFadeAlpha;

	[Tooltip ("The speed at which the trees fade from view.")]
	/// <summary>
	/// The speed at which the trees fade from view.
	/// </summary>
	[SerializeField] float fadeSpeed;

	[Tooltip ("The default alpha value for the leafless trees.\nOnly visible when trees are fadeable and qualioty is 2 or higher.")]
	/// <summary>
	/// The default alpha value for the leafless trees. Only visible when trees are fadeable.
	/// </summary>
	public float stumpAlpha;

	[Space]

	[Tooltip ("The minimum size of trees.")]
	/// <summary>
	/// The minimum size of trees.
	/// </summary>
	[SerializeField] float treeSizeMin;

	[Tooltip ("The maximum size of trees.")]
	/// <summary>
	/// The maximum size of trees.
	/// </summary>
	[SerializeField] float treeSizeMax;

	[Tooltip ("A modifier value used for the panning of trees.")]
	/// <summary>
	/// A modifier value used for the panning of trees.
	/// </summary>
	[SerializeField] float treeMod;

	[Header ("Rock Settings:")]

	[Tooltip ("The minimum size of the rocks.")]
	/// <summary>
	/// The minimum size of the rocks.
	/// </summary>
	[SerializeField] float rockSizeMin;

	[Tooltip ("The maximum size of the rocks.")]
	/// <summary>
	/// The maximum size of the rocks.
	/// </summary>
	[SerializeField] float rockSizeMax;

	[Header ("Prefabs:")]

	[Tooltip ("The various tree prefabs which are spawned.")]
	/// <summary>
	/// The various tree prefabs which are spawned.
	/// </summary>
	[SerializeField] private List<GameObject> TreePrefabs;

	[Tooltip ("The various rock prefabs which are spawned.")]
	/// <summary>
	/// The various rock prefabs which are spawned.
	/// </summary>
	[SerializeField] private List<GameObject> RocksPrefabs;

	/// <summary>
	/// A cache of various parts for each tree for quick access.
	/// </summary>
	List<SpriteRenderer> Leaves, Stumps;
	List<Transform> Trees, Crowns;
	List<float> TreeScale;

	/// <summary>
	/// The transform of the spawner.
	/// </summary>
	Transform spawner;

	/// <summary>
	/// This is used to split the movement of the trees into two phases. Rather than moving all 
	/// of them every frame.
	/// </summary>
	int phase = 0;

	[Header ("Misc:")]

	[Tooltip ("Whether the handler should regularly check for objects out of bounds.\nDefault: off")]
	/// <summary>
	/// Whether the handler should regularly check for objects out of bounds.
	/// </summary>
	[SerializeField] bool checkForStrags;

	[Tooltip ("The amount of seconds between each straggler check.")]
	/// <summary>
	/// The amount of seconds between each straggler check.
	/// </summary>
	[SerializeField] float stragCheck;

	[Tooltip ("The distance for trees or rocks to be considered stragglers.")]
	/// <summary>
	/// The distance for trees or rocks to be considered stragglers.
	/// </summary>
	[SerializeField] float stragDist;

	/// <summary>
	/// The list of rocks used for straggler checking.
	/// </summary>
	List<Transform> Rocks;

	void Start () {
		// Assigns the main handler, for use outside of this script.
		if (Handler != null && Handler != this) {
			Destroy (gameObject);
		} else if (Handler == null) {
			Handler = this;
		}

		// Locates and assigns the spawner transform.
		spawner = Spawner.Handler.transform;

		InitializeForest ();
		InitializeRocks ();

		// If checking for stragglers, invokes the proper method repeatingly.
		if (checkForStrags) {
			InvokeRepeating ("CheckForStragglers", stragCheck, stragCheck);
		}
	}

	void Update () {
		if (treeQuality > 0) {
			MoveTrees ();
		}
	}

	/// <summary>
	/// Spawns the forest into the world.
	/// </summary>
	void InitializeForest () {
		// Initializes the cache for quick acces.
		Leaves = new List<SpriteRenderer> ();
		Stumps = new List<SpriteRenderer> ();
		Trees = new List<Transform> ();
		Crowns = new List<Transform> ();
		TreeScale = new List<float> ();

		for (int i = 0; i < treeDensity; i++) {
			// Assigns a random tree prefab 
			int randTree = Random.Range (0, TreePrefabs.Count);
			// Randomies the tree's rotation
			float randRot = Random.Range (0, 360);
			// Randomizes the treee's size.
			float randSize = Random.Range (treeSizeMin, treeSizeMax);

			// Applies the random rotation to a quaternion.
			Quaternion rotation = Quaternion.Euler (new Vector3 (0, 0, randRot));
			// Instantiates a tree using the Proper prefab.
			GameObject tree = (GameObject) Instantiate (TreePrefabs [randTree], Spawner.Handler.GetRandomSpawn (), rotation);
			tree.transform.localScale = new Vector3 (randSize, randSize, 1);

			// Gets the various tree variables stored in the trees Tree script.
			Tree treeVar = tree.GetComponent<Tree> ();

			// Ensures that the larger trees appear above the smaller trees.
			// This should be disabled if you're using this for an isometric game.
			treeVar.crownRend.sortingOrder = (int) (randSize * 1000);

			// Cashes the various components in the tree script for quick access.
			Leaves.Add (treeVar.crownRend);
			Stumps.Add (treeVar.stumpRend);
			Trees.Add (treeVar.transform);
			Crowns.Add (treeVar.crown);
			TreeScale.Add (randSize);
		}
	}

	/// <summary>
	/// Spawns various collidable rocks into the world.
	/// </summary>
	void InitializeRocks () {
		Rocks = new List<Transform> ();

		for (int i = 0; i < rockDensity; i++) {
			// Assigns a random rock prefab.
			int randRock = Random.Range (0, RocksPrefabs.Count);
			// Randomizes the rock's rotation.
			float randRot = Random.Range (0, 360);
			// Randomizes the rock's size
			float randSize = (Random.Range (rockSizeMin, rockSizeMax));

			// Applies the random rotation to a quaternion.
			Quaternion rotation = Quaternion.Euler (new Vector3 (0, 0, randRot));
			// Instantiates a rock using the proper prefab.
			GameObject rock = (GameObject)Instantiate (RocksPrefabs [randRock], Spawner.Handler.GetRandomSpawn (), rotation);
			rock.transform.localScale = new Vector3 (randSize, randSize, 1);

			// This next line is only needed if you're using my day and night cycle asset.
//			DayNight.Handler.StaticSprites.Add (rock.GetComponent<SpriteRenderer> ());

			// If stragCheck is enabled, adds the rock's transform to the proper list used for strag checking.
			if (checkForStrags) {
				Rocks.Add (rock.transform);
			}
		}
	}

	/// <summary>
	/// Moves the trees to give a pseudo 3D effect. This isn't needed for an isometric game.
	/// </summary>
	void MoveTrees () {
		// This block simply splits the process into two update cycles rather than one.
		int start = 0, finish = 0;

		if (phase == 0) {
			start = 0;
			finish = Trees.Count / 2;
			phase = 1;
		} else if (phase == 1) {
			start = Trees.Count / 2;
			finish = Trees.Count;
			phase = 0;
		}
		//------------------------------------------------------------------------------

		// Gets the spawner's current location.
		Vector2 spawnPos = spawner.position;

		for (int i = start; i < finish; i++) {
			// Checks to see if the tree is visible. It would be pointless to move it otherwise.
			if (Leaves [i].enabled) {
				// Gets the current location of the tree.
				Vector2 treePos = Trees [i].position;
				// Checks the distance between the spawner and the tree.
				float distance = Vector2.Distance (treePos, spawner.position);

				// Calculates the new position for the tree's crown.
				Vector2 newPos = treePos + ((treePos - spawnPos).normalized * ((distance / treeMod) * TreeScale [i]));
				// Applies the croown's new position.
				Crowns [i].position = newPos;

				// Sets the default color. If you're using my day and night cycle asset, replace this with Color mainColor = DayNight.Handler.mainColor; 
				Color mainColor = Color.white;
				// Checks to see if the tree should fade away.
				if (fadeable) {
					// Checks the proximity to the spawner.
					if (distance < fadeDistance) {
						// Gets the leaves' current color.
						Color curCol = Leaves [i].color;
						// If the leaves alpha is greater than the minimum alpha (treeFadeAlpha).
						if (curCol.a > treeFadeAlpha) {
							// Calculates the new color.
							Color newCol = new Color (mainColor.r, mainColor.g, mainColor.b, curCol.a - fadeSpeed * Time.deltaTime);
							// Applies the new color.
							Leaves [i].color = newCol;
						}
						// The next few lines are only needed if you are using my day and night cycle.
						if (Stumps [i].isVisible) {
							Stumps [i].color = new Color (mainColor.r, mainColor.g, mainColor.b, stumpAlpha);
						}
					} else { 	// If the tree is far away enough from the spawner, do the same as above but in reverse.
						// Gets the leaves' current color.
						Color curCol = Leaves [i].color;
						// If the leaves alpha is less than 1.
						if (curCol.a < 1) {
							// Calculates the new color.
							Color newCol = new Color (mainColor.r, mainColor.g, mainColor.b, curCol.a + fadeSpeed * Time.deltaTime);
							// Applies the new color.
							Leaves [i].color = newCol;
						} 
					}
				} else {	// If not fadeable, applies the main color.
					Leaves [i].color = mainColor;
				}
			}
		}
	}

	/// <summary>
	/// Checks for out of bounds trees, which can happen if the camera moves too fast.
	/// </summary>
	void CheckForStragglers () {
		for (int i = 0; i < Trees.Count; i++) {
			float dist = Vector2.Distance (Trees [i].position, spawner.position);
			if (dist > stragDist) {
				Vector2 newPos = Spawner.Handler.GetRandomSpawn (Trees [i].position);
				Trees [i].position = newPos;
			}
		}

		for (int i = 0; i < Rocks.Count; i++) {
			float dist = Vector2.Distance (Rocks [i].position, spawner.position);
			if (dist > stragDist) {
				Vector2 newPos = Spawner.Handler.GetRandomSpawn (Rocks [i].position);
				Rocks [i].position = newPos;
			}
		}
	}
}