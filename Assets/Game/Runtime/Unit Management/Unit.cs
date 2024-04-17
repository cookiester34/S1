using UnityEngine;

namespace S1.Runtime.UnitManagement
{
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

		//TODO: Movement needs an overhaul
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

		public Vector2 PositionOnScreen(Camera mainCamera)
		{
			var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
			return LastScreenPosition = new Vector2(screenPoint.x, Screen.height - screenPoint.y);
		}

		public void DestroyUnit()
		{
			UnitManager.Instance.DestroyUnit(this);
		}
	}
}