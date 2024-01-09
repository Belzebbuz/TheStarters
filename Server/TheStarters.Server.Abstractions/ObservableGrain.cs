using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace TheStarters.Server.Abstractions;

public interface IObservableGrain<in T>
	where T : IGrainObserver
{
	ValueTask SubscribeAsync(T observer);
	ValueTask UnsubscribeAsync(T observer);
}

public class ObservableGrain<T>(ILogger<ObservableGrain<T>> logger) : Grain, IObservableGrain<T>
	where T : IGrainObserver
{
	protected readonly ObserverManager<T> ObserverManager = new(TimeSpan.FromMinutes(60), logger);

	public ValueTask SubscribeAsync(T observer)
	{
		ObserverManager.Subscribe(observer,observer);
		return ValueTask.CompletedTask;
	}

	public ValueTask UnsubscribeAsync(T observer)
	{
		ObserverManager.Unsubscribe(observer);
		return ValueTask.CompletedTask;
	}
}