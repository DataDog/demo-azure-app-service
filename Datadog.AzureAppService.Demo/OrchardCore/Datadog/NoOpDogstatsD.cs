using StatsdClient;
using System;

namespace OrchardCore.Sample
{
	public class NoOpDogStatsd : IDogStatsd
	{
		public ITelemetryCounters TelemetryCounters => null;

		public void Configure(StatsdConfig config)
		{
			// no-op
		}

		public void Counter(string statName, double value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public void Decrement(string statName, int value = 1, double sampleRate = 1, params string[] tags)
		{
			// no-op
		}

		public void Dispose()
		{
			// no-op
		}

		public void Distribution(string statName, double value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public void Event(string title, string text, string alertType = null, string aggregationKey = null, string sourceType = null, int? dateHappened = null, string priority = null, string hostname = null, string[] tags = null)
		{
			// no-op
		}

		public void Gauge(string statName, double value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public void Histogram(string statName, double value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public void Increment(string statName, int value = 1, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public void ServiceCheck(string name, Status status, int? timestamp = null, string hostname = null, string[] tags = null, string message = null)
		{
			// no-op
		}

		public void Set<T>(string statName, T value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public void Set(string statName, string value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public IDisposable StartTimer(string name, double sampleRate = 1, string[] tags = null)
		{
			// no-op
			return default(IDisposable);
		}

		public void Time(Action action, string statName, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}

		public T Time<T>(Func<T> func, string statName, double sampleRate = 1, string[] tags = null)
		{
			// no-op
			return default(T);
		}

		public void Timer(string statName, double value, double sampleRate = 1, string[] tags = null)
		{
			// no-op
		}
	}
}