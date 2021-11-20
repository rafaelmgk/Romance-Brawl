using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerConnect : NetworkBehaviour{
	// public static Vector2 velocity2D;
	// public static Vector2 targetVelocity;
	// public static Vector3 velocity3D;
	// public static float damping;
	// public static float jumpForce;

	public Rigidbody2D rgbd2d;

	[Command]
	public void CmdMove(float move, Vector3 m_Velocity, float m_MovementSmoothing) {
		Vector3 targetVelocity = new Vector2(move * 10f, rgbd2d.velocity.y);
		rgbd2d.velocity = Vector3.SmoothDamp(rgbd2d.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
	}

	[Command]
	public void CmdJump(float m_JumpForce) {
		rgbd2d.AddForce(new Vector2(0f, m_JumpForce));
	}

	[Command]
	public void CmdFlip() {
		transform.Rotate(0f, 180f, 0f);
	}
}
