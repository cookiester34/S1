using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace S1.Runtime.Input
{
	public static class InputHandler
	{
		private static PlayerInput playerInput;
		private static readonly Dictionary<string, AbstractInputHandler> inputActions = new();
		private static readonly HashSet<object> scriptsUsingInput = new();
		
		public static bool IsSetup => playerInput != null;
		
		public static void Setup(PlayerInput playerInput)
		{
			InputHandler.playerInput ??= playerInput;
		}

		/// <summary>
		/// Register a script to use the input handler
		/// </summary>
		/// <param name="script"></param>
		public static void RegisterScriptUsage(object script)
		{
			scriptsUsingInput.Add(script);
		}
		
		/// <summary>
		/// We remove a script from the list of scripts using the input handler
		/// When the list is empty we dispose the input handler
		/// </summary>
		/// <param name="script"></param>
		public static void DisposeScriptUsage(object script)
		{
			scriptsUsingInput.Remove(script);
			
			if (scriptsUsingInput.Count == 0)
			{
				Dispose();
			}
		}

		public static bool TryGetInputAction(string actionName, out AbstractInputHandler inputHandler)
		{
			return inputActions.TryGetValue(actionName, out inputHandler);
		}

		public static void AddInputAction<T>(string actionName, out InputHandlerOfType<T> inputHandlerOfType) where T : struct
		{
			if (TryAddInputAction<T>(actionName, out inputHandlerOfType))
			{
				return;
			}
			throw new Exception("Failed to add input action, action already exists");
		}

		public static bool TryAddInputAction<T>(string actionName, out InputHandlerOfType<T> inputHandlerOfType) where T : struct
		{
			try
			{
				if (inputActions.TryGetValue(actionName, out var abstractInputHandler))
				{
					inputHandlerOfType = (InputHandlerOfType<T>)abstractInputHandler;
				}
				else
				{
					var inputAction = playerInput.actions[actionName];
					inputHandlerOfType = new InputHandlerOfType<T>(inputAction);
					inputActions.Add(actionName, inputHandlerOfType);
				}
			}
			catch (Exception _)
			{
				inputHandlerOfType = null;
				return false;
			}

			return true;
		}

		public static bool TryAddInputAction(string actionName, out ActionInputHandler inputHandlerOfType)
		{
			try
			{
				if (inputActions.TryGetValue(actionName, out var abstractInputHandler))
				{
					inputHandlerOfType = (ActionInputHandler)abstractInputHandler;
				}
				else
				{
					var inputAction = playerInput.actions[actionName];
					inputHandlerOfType = new ActionInputHandler(inputAction);
					inputActions.Add(actionName, inputHandlerOfType);
				}
			}
			catch (Exception _)
			{
				inputHandlerOfType = null;
				return false;
			}

			return true;
		}
		
		
		/// <summary>
		/// Should only be used by the InputBehaviour class
		/// Unless you register the class and unregister it when it's destroyed
		/// Actions will not be disposed until all scripts using the input handler are destroyed
		/// </summary>
		/// <param name="actionName"></param>
		/// <param name="onPerformed"></param>
		/// <param name="onCancel"></param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="Exception"></exception>
		public static void AddInputAction<T>(string actionName, Action<T> onPerformed, Action onCancel = null) where T : struct
		{
			if (TryAddInputAction<T>(actionName, out var inputHandlerOfType))
			{
				inputHandlerOfType.OnPerformed += onPerformed;
				inputHandlerOfType.OnCanceled += onCancel;
				return;
			}
			throw new Exception("Failed to add input action, action already exists");
		}

		/// <summary>
		/// Should only be used by the InputBehaviour class
		/// Unless you register the class and unregister it when it's destroyed
		/// Actions will not be disposed until all scripts using the input handler are destroyed
		/// </summary>
		/// <param name="actionName"></param>
		/// <param name="onPerformed"></param>
		/// <param name="onCancel"></param>
		/// <exception cref="Exception"></exception>
		public static void AddInputAction(string actionName, Action onPerformed, Action onCancel = null)
		{
			if (TryAddInputAction(actionName, out var inputHandlerOfType))
			{
				inputHandlerOfType.OnPerformed += onPerformed;
				inputHandlerOfType.OnCanceled += onCancel;
				return;
			}
			throw new Exception("Failed to add input action, action already exists");
		}

		public static void Dispose()
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
}