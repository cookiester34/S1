using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
	private bool destinationReached = true;
	private Vector3 targetDestination;
	private Collider unitCollider;
	
	public Vector3 UnitPosition => transform.position;
	public Vector3 Forward => transform.forward;
	public Vector2 LastScreenPosition { get; private set; }

	private void Awake()
	{
		UnitManager.Instance.AddUnit(this);
		unitCollider = GetComponent<Collider>();
	}

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

	public Vector2 CameraPosition(Camera mainCamera)
	{
		var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
		LastScreenPosition = new Vector2(screenPoint.x, Screen.height - screenPoint.y);
		return LastScreenPosition;
	}
	
	public bool IsBoundsWithinRect(Camera mainCamera, Rect selectionRect)
	{
		var bounds = unitCollider.bounds;

		var corners = new Vector3[8];
		corners[0] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z));
		corners[1] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z));
		corners[2] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z));
		corners[3] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z));
		corners[4] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z));
		corners[5] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z));
		corners[6] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z));
		corners[7] = mainCamera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z));

		return corners.Any(corner => selectionRect.Contains(corner));
	}
}