using UnityEngine;

public class CollisionHandler : MonoBehaviour
{

	public int numOfCollisions = 0;
	public int MainBoneCollision = 0;

	public void IsGrounded()
	{
		numOfCollisions += 1;
	}

	public void MainBoneGrounded()
	{
		MainBoneCollision += 1;
	}

	public void IsNotGrounded()
	{
		numOfCollisions -= 1;
	}

	public void MainBoneNotGrounded()
	{
		MainBoneCollision -= 1;
	}
	
	

}