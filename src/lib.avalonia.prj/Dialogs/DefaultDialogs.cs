using Lib.Avalonia.Services.Dialogs.MessageBox;
using ReactiveUI; 

namespace Lib.Avalonia.Services.Dialogs;

/// <summary>
/// Хранилище базовых диалоговых окон IDialog.
/// </summary>
public class DefaultDialogs : ViewModelBase
{
    #region Fields  

    private IDialog? _messageBox; 

    #endregion Fields

    #region Properties 

    /// <summary>
    /// Экземпляр диалоговой панели MessageBox.
    /// </summary>
    public IDialog? MessageBox
	{
		get => _messageBox;
		set => this.RaiseAndSetIfChanged(ref _messageBox, value);
    } 

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Хранилище базовых диалоговых окон IDialog.
    /// </summary>
    public DefaultDialogs()
	{
		MessageBox = new MessageBoxViewModel(); 

        DialogSystem.MessageBoxObserve
			.BindTo(this, x => x.MessageBox);
	}

	#endregion Constructors 
}
