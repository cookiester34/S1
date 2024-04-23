using S1.Runtime.Teams;
using UnityEngine;

namespace S1.Runtime.UnitManagement
{
	public class Unit : MonoBehaviour
	{
		[SerializeField]
		private bool isEnemy;
		
		private bool destinationReached = true;
		private Vector3 targetDestination;
		private Quaternion targetRotation;
		private Collider unitCollider;
		private GameObject preview;
	
		public Team Team { get; private set; }
		public Vector3 UnitPosition => transform.position;
		public Vector3 Forward => transform.forward;
		public Vector2 LastScreenPosition { get; private set; }

		private void Awake()
		{
			if (isEnemy)
			{
				
			}
			
			unitCollider = GetComponent<Collider>();
			
			preview = new GameObject("Preview");
			
			var meshFilter = preview.AddComponent<MeshFilter>();
			meshFilter.mesh = GetComponentInChildren<MeshFilter>().mesh;
			preview.AddComponent<MeshRenderer>();
			preview.transform.SetParent(transform);
			preview.SetActive(false);
		}
		
		/// <summary>
		/// Will add the unit to the team and remove it from the previous team if it was on one.
		/// </summary>
		/// <param name="team"></param>
		public void TrySetTeam(Team team)
		{
			Team?.RemoveUnit(this);
			Team = team;
			Team.AddUnit(this);
		}

		//TODO: Movement needs an overhaul
		public void Move(Vector3 destination, Quaternion unitRotation)
		{
			destinationReached = false;
			targetDestination = destination;
			targetRotation = unitRotation;
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
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.1f);
		}

		public Vector2 PositionOnScreen(Camera mainCamera)
		{
			var screenPoint = mainCamera.WorldToScreenPoint(transform.position);
			return LastScreenPosition = new Vector2(screenPoint.x, Screen.height - screenPoint.y);
		}

		public void ShowPreview(Vector3 position, Quaternion rotation)
		{
			preview.SetActive(true);
			var previewTransform = preview.transform;
			previewTransform.position = position;
			previewTransform.rotation = rotation;
		}
		
		public void HidePreview()
		{
			preview.SetActive(false);
		}
	}
}