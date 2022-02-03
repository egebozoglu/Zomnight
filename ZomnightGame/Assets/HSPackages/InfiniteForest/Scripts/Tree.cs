//Copyright Highwalker Studios 2016
//Author: Luc Highwalker
//package: 2D Infinite Forest

using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {
	/// <summary>
	/// Various variables stored within this class. These should not be changed.
	/// </summary>
	public SpriteRenderer rend, crownRend, stumpRend;
	public Transform crown;

	/// <summary>
	/// The various animation controllers used by the tree.
	/// </summary>
	public RuntimeAnimatorController low, lowStump, high, highStump;

	/// <summary>
	/// The tree's various animators.
	/// </summary>
	public Animator crownAnim, stumpAnim;

	void Start () {
		// The shadow's animator.
		Animator anim = GetComponent<Animator>();
		// Sets a random number from 0 to 1 for use in randomizing the animation.
		float rand = Random.Range (0f, 1f);

		// This block sets the proper animation controllers based on the quality setting.
		if (ForestHandler.Handler.treeQuality == 4) {
			anim.runtimeAnimatorController = high;
			crownAnim.runtimeAnimatorController = high;
			stumpAnim.runtimeAnimatorController = highStump;
		} else {
			anim.runtimeAnimatorController = low;
			crownAnim.runtimeAnimatorController = low;
			stumpAnim.runtimeAnimatorController = lowStump;
		}
		// ------------------------------------------------------------------------------

		// Removes references as they are no longer needed.
		low = null;
		lowStump = null;
		high = null;
		highStump = null;

		// This block randomizes the animations if the trees are animated. Else, it disables the animators.
		if (ForestHandler.Handler.treeQuality >= 3) {
			AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo (0);
			anim.Play (state.fullPathHash, -1, rand);

			AnimatorStateInfo crownState = crownAnim.GetCurrentAnimatorStateInfo (0);
			crownAnim.Play (crownState.fullPathHash, -1, rand);

			AnimatorStateInfo stumpState = stumpAnim.GetCurrentAnimatorStateInfo (0);
			stumpAnim.Play (stumpState.fullPathHash, -1, rand);
		} else {
			anim.enabled = false;
			crownAnim.enabled = false;
			stumpAnim.enabled = false;
		}
		// -------------------------------------------------------------------------------------------------

		// Checks to see if the stump should be active. If it is, sets the proper color and alpha to the stump.
		if (ForestHandler.Handler.treeQuality == 1) {
			stumpRend.gameObject.SetActive (false);
		} else {
			Color color = stumpRend.color;
			stumpRend.color = new Color (color.r, color.g, color.b, ForestHandler.Handler.stumpAlpha);
		}
		// --------------------------------------------------------------------------------------------------

		// Disables the shadow if quality setting is at the lowest.
		if (ForestHandler.Handler.treeQuality == 0) {
			rend.enabled = false;

			// Disables the Tree script, as it is no longer needed.
			this.enabled = false;
		}
	}

	/// <summary>
	/// This is a workaround for trees popping in and out of view.
	/// </summary>
	void OnBecameInvisible () {
		crownRend.enabled = false;
		stumpRend.enabled = false;
	}

	/// <summary>
	/// This is a workaround for trees popping in and out of view.
	/// </summary>
	void OnBecameVisible () {
		crownRend.enabled = true;
		stumpRend.enabled = true;
	}
}
