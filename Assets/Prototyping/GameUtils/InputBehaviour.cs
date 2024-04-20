using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public abstract class InputBehaviour : MonoBehaviour
{
	protected InputHandlerPrototype inputHandlerPrototype;

	private void Awake()
	{
		var playerInput = GetComponent<PlayerInput>();
		if (playerInput != null)
		{
			inputHandlerPrototype = new InputHandlerPrototype(playerInput);
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