using UnityEngine;

public class Unit : MonoBehaviour
{
	private bool destinationReached = false;
	private Vector3 targetDestination;
		
	public void Move(Vector3 destination)
	{
		destinationReached = false;
		targetDestination = destination;
	}

	private void Update()
	{
		if (destinationReached) return;
			
		var distance = Vector3.Distance(transform.position, targetDestination);
		if (distance < 0.1f)
		{
			destinationReached = true;
			return;
		}
			
		transform.position = Vector3.MoveTowards(transform.position, targetDestination, 0.1f);
	}
}