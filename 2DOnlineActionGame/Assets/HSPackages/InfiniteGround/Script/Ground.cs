//Copyright Highwalker Studios 2016
//Author: Luc Highwalker
//package: 2D Infinite Ground

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : MonoBehaviour
{	
	/// <summary>
	/// The modifier which is used to determine the speed that the texture scrolls.
	/// </summary>
	[Range(0, 1)] public float modifier;

	/// <summary>
	/// The render displaying the texture.
	/// </summary>
	Renderer render;

	/// <summary>
	/// Various variables used to determine the offset of the texture.
	/// </summary>
	Vector2 newOffset, offset;
	Vector3 lastPosition;

	void Start()
	{
		// Sets the render variable.
		render = GetComponent<Renderer>();
	}

	void Update()
	{
		// Sets the current texture offset.
		Vector2 oldOffset = offset; 

		// Determines where the texture should be offset based on previous position.
		newOffset = (transform.position - lastPosition) * modifier; 
		lastPosition = transform.position;

		// Sets the new offset.
		offset = oldOffset + newOffset; 

		// Applies the new offset.
		render.material.mainTextureOffset = offset; 
	}
}