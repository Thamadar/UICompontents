
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

using Lib.Avalonia.Services.Dialogs.MessageBox; 
using Lib.Avalonia.Services.Dialogs.ViewDialog; 

using System;
using System.Collections.Generic; 
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Lib.Avalonia.Services.Dialogs;

/// <summary>
/// Класс, с помощью которого можно управлять диалоговыми окнами в проекте.
/// </summary>
public static class DialogSystem
{

	#region Fields 

	private readonly static Subject<bool> _isDialogsOpenedStateSubject          = new Subject<bool>(); 

	private static readonly Stack<IViewDialog> _dialogsStack   = new(); 

	private static IApplicationLifetime? _applicationLifetime;

	private static IViewDialog<MessageBoxParameters, MessageBoxResult>? _messageBox => MessageBoxDialogData.CurrentDialog;

	private static IViewDialog? _currentDialog; 

	private static DialogData<MessageBoxParameters, MessageBoxResult> MessageBoxDialogData { get; } = new();
	  
	private static bool _isDialogsOpenedState; 

	private static bool IsDialogsOpenedState 
	{
		get { return _isDialogsOpenedState; }
		set
		{
			_isDialogsOpenedState = value;
			_isDialogsOpenedStateSubject.OnNext(value);
		}
	}  
	  
	#endregion

	#region Properties 

	/// <summary>
	/// Отслеживание текущего MessageBox.
	/// </summary>
	public static IObservable<IViewDialog<MessageBoxParameters, MessageBoxResult>?> MessageBoxObserve => MessageBoxDialogData.CurrentDialogObserve; 

	/// <summary>
	/// Отслеживание состояния "Открыто ли какой-либо IDialog (не окно)?".
	/// </summary>
	public static IObservable<bool> IsDialogsOpenedObserve => _isDialogsOpenedStateSubject.AsObservable();
	  
	#endregion

	#region Methods 
	   
	/// <summary>
	/// Закрыть все диалоги IDialog.
	/// </summary> 
	public static async Task CloseAllDialogs()
	{ 
		var currentDialog = _currentDialog;
		while(currentDialog != null)
		{
			await currentDialog.HardClose(true);
			if(currentDialog == _currentDialog)
				break;
			currentDialog = _currentDialog;
		}
		foreach(var dialog in _dialogsStack)
		{
			await dialog.HardClose(true);
		} 
	}

	public static void SetApplicationLifetime(IApplicationLifetime applicationLifetime) => _applicationLifetime = applicationLifetime;
	    
	/// <summary>
	/// Затменить текущее диалоговое окно IDialog.
	/// </summary> 
	public static void ShadeCurrentDialog()
	{
		_currentDialog?.Shade();
	}

	/// <summary>
	/// Скрыть текущее диалоговое окно IDialog.
	/// </summary> 
	public static void HideCurrentDialog()
	{
		_currentDialog?.Hide();
	}

	/// <summary>
	/// Расскрыть текущее диалоговое окно IDialog.
	/// </summary> 
	public static void UnhideCurrentDialog()
	{
		_currentDialog?.Unhide();
	}

	/// <summary>
	/// Снять затемнение с текущего диалогового окна IDialog.
	/// </summary> 
	public static void UnshadeCurrentDialog()
	{
		_currentDialog?.Unshade();
	} 

	/// <summary>
	/// Открыть диалоговую панель типа IDialog.
	/// </summary> 
	public static async Task<R> OpenDialog<D, P, R>(D dialog, P parameters)
		   where D : IViewDialog<P, R>
		   where P : ViewDialogParameters
		   where R : ViewDialogDataResult
	{ 
		if(_currentDialog != null &&
			_currentDialog.IsOpen)
		{
			_dialogsStack.Push(_currentDialog);
			ShadeCurrentDialog();
			HideCurrentDialog();
		}  

		dialog.Unhide();

		ChangeCurrentDialog(dialog);

		var result = await dialog.Open(parameters); 

		if(_dialogsStack.Count > 0)
		{
			var previousDialog = _dialogsStack.Pop();
			previousDialog.Unshade();
			previousDialog.Unhide();

			ChangeCurrentDialog(previousDialog); 
		} 
		else
		{
			ChangeCurrentDialog(null);  
		} 

		return result; 
	} 

	/// <summary>
	/// Открыть диалоговое окно.
	/// </summary>
	/// <param name="parameters">параметры MessageBox</param> 
	public static async Task<MessageBoxResult> OpenMessageBox(MessageBoxParameters parameters)
	{ 
		var messageBoxDialog = new MessageBoxViewModel(); 
		return await OpenDialog<IViewDialog<MessageBoxParameters, MessageBoxResult>, MessageBoxParameters, MessageBoxResult>(messageBoxDialog, parameters);
	}

	/// <summary>
	/// Открыть диалоговое окно, используя VM.
	/// </summary
	/// <param name="parameters">параметры MessageBox</param> 
	/// <param name="messageBoxViewModel">MessageBox VM</param> 
	public static async Task<MessageBoxResult> OpenMessageBox(MessageBoxParameters parameters, MessageBoxViewModel messageBoxViewModel)
	{ 
		return await OpenDialog<IViewDialog<MessageBoxParameters, MessageBoxResult>, MessageBoxParameters, MessageBoxResult>(messageBoxViewModel, parameters);
	} 
	 
	/// <summary>
	/// Прерывание MessageBox.
	/// </summary> 
	public static async Task CloseMessageBox()
	{ 
		if(_messageBox != null)
			await _messageBox.HardClose(true);
	} 
	  
	/// <summary>
	/// Смена текущего IDialog.
	/// </summary>
	/// <param name="dialog">новый IDialog</param>
	private static void ChangeCurrentDialog(IViewDialog? dialog)
	{
		_currentDialog = dialog;
		switch(_currentDialog)
		{
			case MessageBoxViewModel messageBoxViewModel:
				MessageBoxDialogData.CurrentDialog = messageBoxViewModel;
				break;
		}

		IsDialogsOpenedState = _currentDialog != null;
	}

	#endregion
}
