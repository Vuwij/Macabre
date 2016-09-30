using UnityEngine;
using System.Collections;

public class RootMotionToParent : MonoBehaviour {
	float moveRatio = 32;

	void OnAnimatorMove() {
		Animator animator = GetComponent<Animator>();
		if (animator) {
			Vector3 newPosition = transform.parent.position;
			newPosition.y += moveRatio * animator.deltaPosition.y;
			newPosition.x += moveRatio * animator.deltaPosition.x;
			transform.parent.position = newPosition;
		}
	}
}