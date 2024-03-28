namespace Bargeh.Main.Wapp.Client.Infrastructure;

public class NotFoundListener
{
	public Action OnNotFound { get; set; } = null!;

	public void NotifyNotFound()
	{
		OnNotFound.Invoke();
	}
}