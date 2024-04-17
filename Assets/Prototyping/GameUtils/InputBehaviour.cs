using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class InputBehaviour : MonoBehaviour
{
	protected InputHandler inputHandler;

	private void Awake()
	{
		var playerInput = GetComponent<PlayerInput>();
		if (playerInput != null)
		{
			inputHandler = new InputHandler(playerInput);
			SetupInput();
		}
		else
		{
			throw new Exception("PlayerInput component not found. No input handler created.");
		}
			
		OnAwake();
	}

	protected virtual void OnAwake(){}
		
	protected abstract void SetupInput();
}