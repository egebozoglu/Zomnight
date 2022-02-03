using UnityEngine;
using System.Collections;

public class DummyCam : MonoBehaviour {
	Rigidbody2D body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 direction = Vector2.zero;

		if (Input.GetKey ("w")) {
			direction.y += 500;
		} 
		if (Input.GetKey ("s")) {
			direction.y -= 500;
		}

		if (Input.GetKey ("a")) {
			direction.x -= 500;
		}
		if (Input.GetKey ("d")) {
			direction.x += 500;
		}

		body.velocity = direction * Time.deltaTime;
	}
}
