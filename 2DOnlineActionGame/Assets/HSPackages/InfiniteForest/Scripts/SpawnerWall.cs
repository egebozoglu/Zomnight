//Copyright Highwalker Studios 2016
//Author: Luc Highwalker
//package: 2D Infinite Forest

using UnityEngine;
using System.Collections;

public class SpawnerWall : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D col) {
		if (col.tag == "Disable") {
			col.gameObject.SetActive (false);
		} else {
			col.transform.position = Spawner.Handler.GetRandomSpawn (transform.localPosition);
		}
	}

	void OnTriggerExit2D (Collider2D col) {
		if (col.tag == "Disable") {
			col.gameObject.SetActive (false);
		} else {
			col.transform.position = Spawner.Handler.GetRandomSpawn (transform.localPosition);
		}
	}
}
