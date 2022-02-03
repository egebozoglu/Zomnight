//Copyright Highwalker Studios 2016
//Author: Luc Highwalker
//package: 2D Infinite Forest

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
	public static Spawner Handler { get; private set; }

	[Tooltip ("The amount of space the spawner has to spawn objects.\n" +
		"Check the documentation for instructions on properly modifying this variable")]
	/// <summary>
	/// The size of the spawner.
	/// </summary>
	[SerializeField] private float spawnerSize;

	/// <summary>
	/// The size of the camera.
	/// </summary>
	float cameraSize;

	[Tooltip ("The size modifier for the camera. Increase this if objects pop into view.")]
	/// <summary>
	/// The size modifier for the camera. Increase this if objects pop into view.
	/// </summary>
	[SerializeField] float camSizeMod;

	/// <summary>
	/// The main camera the spawner is attached to.
	/// </summary>
	Camera mainCam;

	void Start () {
		// Assigns the main handler, for use outside of this script.
		if (Handler != null && Handler != this) {
			Destroy (gameObject);
		} else if (Handler == null) {
			Handler = this;
		}

		// Retrives the camera component from the spawner's parent.
		mainCam = GetComponentInParent<Camera> ();
	}

	void Update () {
		// Sets the camera size variable. 
		// If you never zoom your camera in or out, you can comment this out and set it manually.
		cameraSize = mainCam.orthographicSize + camSizeMod;
	}

	/// <summary>
	/// Returns a random spawn point anywhere in the spawner area. 
	/// This should only be used in initialization processes, as it allows for items to spawn
	/// in the camera's view.
	/// </summary>
	/// <returns>A random spawn point.</returns>
	public Vector2 GetRandomSpawn () {
		Vector2 spawn = transform.position;

		spawn.x += Random.Range (-spawnerSize, spawnerSize);
		spawn.y += Random.Range (-spawnerSize, spawnerSize);

		return spawn;
	}

	/// <summary>
	/// Returns a random spawn point outside of the cmera's view, based on which spawner wall called
	/// the function (pos).
	/// </summary>
	/// <returns>A random spawn point.</returns>
	/// <param name="pos">The spawner wall's position.</param>
	public Vector2 GetRandomSpawn (Vector2 pos) {
		Vector2 spawn = transform.position;

		if (pos.x > 0) {
			spawn.x -= Random.Range (cameraSize, spawnerSize);
			spawn.y += Random.Range (-spawnerSize, spawnerSize);
		} else if (pos.x < 0) {
			spawn.x += Random.Range (cameraSize, spawnerSize);
			spawn.y += Random.Range (-spawnerSize, spawnerSize);
		} else if (pos.y > 0) {
			spawn.y -= Random.Range (cameraSize, spawnerSize);
			spawn.x += Random.Range (-spawnerSize, spawnerSize);
		} else if (pos.y < 0){
			spawn.y += Random.Range (cameraSize, spawnerSize);
			spawn.x += Random.Range (-spawnerSize, spawnerSize);
		}

		return spawn;
	}
}
