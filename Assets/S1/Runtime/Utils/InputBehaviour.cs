using S1.Runtime.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace S1.Runtime.Utils
{
	[RequireComponent(typeof(PlayerInput))]
	/// Use this class to create input behaviours that require the use of the InputHandler
	public abstract class InputBehaviour : MonoBehaviour
	{
		private void Awake()
		{
			if (!InputHandler.IsSetup)
			{
				var playerInput = GetComponent<PlayerInput>();
				if (playerInput != null)
				{
					InputHandler.Setup(playerInput);
					SetupInput();
				}
				else
				{
					throw new Exception("PlayerInput component not found. No input handler created.");
				}
			}
			
			OnAwake();
		}

		protected virtual void OnAwake(){}
		
		protected abstract void SetupInput();
	}
}