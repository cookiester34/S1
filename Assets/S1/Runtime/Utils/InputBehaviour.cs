using S1.Runtime.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace S1.Runtime.Utils
{
	[RequireComponent(typeof(PlayerInput))]
	// Use this class to create input behaviours that require the use of the InputHandler
	// It makes sure that the input handler is setup and disposed correctly.
	// Should only be used for input that is used throughout the entire game.
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
				}
				else
				{
					throw new Exception("PlayerInput component not found. No input handler created.");
				}
			}
			
			InputHandler.RegisterScriptUsage(this);
			SetupInput();
			OnAwake();
		}

		protected virtual void OnAwake(){}
		
		protected abstract void SetupInput();

		private void OnDestroy()
		{
			InputHandler.DisposeScriptUsage(this);
		}
	}
}