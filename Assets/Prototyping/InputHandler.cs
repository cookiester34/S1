using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputHandler
{
	private PlayerInput playerInput;
	private Dictionary<string, AbstractInputHandler> inputActions = new();
	
	public InputHandler(PlayerInput playerInput)
	{
		this.playerInput = playerInput;
	}
	
	public bool TryGetInputAction(string actionName, out AbstractInputHandler inputHandler)
	{
		return inputActions.TryGetValue(actionName, out inputHandler);
	}
	
	public void AddInputAction<T>(string actionName, out InputHandlerOfType<T> inputHandlerOfType) where T : struct
	{
		try
		{
			var inputAction = playerInput.actions[actionName];
			inputHandlerOfType = new InputHandlerOfType<T>(inputAction);
			inputActions.Add(actionName, inputHandlerOfType);
		}
		catch
		{
			throw new Exception("Failed to add input action, action already exists");
		}
	}
	
	public void AddInputAction<T>(string actionName, Action<T> onPerformed, Action onCancel = null) where T : struct
	{
		try
		{
			var inputAction = playerInput.actions[actionName];
			var inputHandlerOfType = new InputHandlerOfType<T>(inputAction);
			inputHandlerOfType.OnPerformed += onPerformed;
			inputHandlerOfType.OnCanceled += onCancel;
			inputActions.Add(actionName, inputHandlerOfType);
		}
		catch
		{
			throw new Exception("Failed to add input action, action already exists");
		}
	}
	
	public void AddInputAction(string actionName, Action onPerformed, Action onCancel = null)
	{
		try
		{
			var inputAction = playerInput.actions[actionName];
			var inputHandlerOfType = new ActionInputHandler(inputAction);
			inputHandlerOfType.OnPerformed += onPerformed;
			inputHandlerOfType.OnCanceled += onCancel;
			inputActions.Add(actionName, inputHandlerOfType);
		}
		catch
		{
			throw new Exception("Failed to add input action, action already exists");
		}
	}
	
	public bool TryAddInputAction<T>(string actionName, out InputHandlerOfType<T> inputHandlerOfType) where T : struct
	{
		try
		{
			var inputAction = playerInput.actions[actionName];
			inputHandlerOfType = new InputHandlerOfType<T>(inputAction);
			inputActions.Add(actionName, inputHandlerOfType);
		}
		catch (Exception _)
		{
			inputHandlerOfType = null;
			return false;
		}
		return true;
	}
	
	public bool TryAddInputAction(string actionName, out ActionInputHandler inputHandlerOfType)
	{
		try
		{
			var inputAction = playerInput.actions[actionName];
			inputHandlerOfType = new ActionInputHandler(inputAction);
			inputActions.Add(actionName, inputHandlerOfType);
		}
		catch (Exception _)
		{
			inputHandlerOfType = null;
			return false;
		}
		return true;
	}
	public void Dispose()
	{
		foreach (var inputHander in inputActions)
		{
			inputHander.Value.Dispose();
		}
		inputActions.Clear();
		playerInput = null;
	}
}
public abstract class AbstractInputHandler
{
	public abstract void Dispose();
}

public class InputHandlerOfType<T> : AbstractInputHandler where T : struct
{
	public InputAction InputAction { get; }
	
	public event Action<T> OnPerformed;
	public event Action OnCanceled;
	
	public T Value => InputAction.ReadValue<T>();
	
	public InputHandlerOfType(InputAction inputAction)
	{
		InputAction = inputAction;
		InputAction.performed += OnInputActionOnperformed;
		InputAction.canceled += OnInputActionOncanceled;
	}
	private void OnInputActionOncanceled(InputAction.CallbackContext _)
	{
		OnCanceled?.Invoke();
	}
	private void OnInputActionOnperformed(InputAction.CallbackContext callbackContext)
	{
		OnPerformed?.Invoke(callbackContext.ReadValue<T>());
	}
	
	public override void Dispose()
	{
		OnPerformed = null;
		OnCanceled = null;
		InputAction.performed -= OnInputActionOnperformed;
		InputAction.canceled -= OnInputActionOncanceled;
		InputAction.Dispose();
	}
}
public class ActionInputHandler : AbstractInputHandler
{
	public InputAction InputAction { get; }
	public event Action OnPerformed;
	public event Action OnCanceled;
	public ActionInputHandler(InputAction inputAction)
	{
		InputAction = inputAction;
		InputAction.performed += OnInputActionOnperformed;
		InputAction.canceled += OnInputActionOncanceled;
	}
	private void OnInputActionOncanceled(InputAction.CallbackContext _)
	{
		OnCanceled?.Invoke();
	}
	private void OnInputActionOnperformed(InputAction.CallbackContext _)
	{
		OnPerformed?.Invoke();
	}
	public override void Dispose()
	{
		OnPerformed = null;
		OnCanceled = null;
		InputAction.performed -= OnInputActionOnperformed;
		InputAction.canceled -= OnInputActionOncanceled;
		InputAction.Dispose();
	}
}