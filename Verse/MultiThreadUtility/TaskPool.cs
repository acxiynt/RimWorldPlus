using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Verse.MultiThreadUtility;

public class TaskPool : IDisposable
{
	private interface IQueuedTask
	{
		void Run();
	}

	private class QueuedTask<T> : IQueuedTask
	{
		public Func<T> func;

		public TaskCompletionSource<T> promise;

		public void Run()
		{
			try
			{
				promise.TrySetResult(func());
			}
			catch (Exception exception)
			{
				promise.TrySetException(exception);
			}
		}

		public QueuedTask(Func<T> func, TaskCompletionSource<T> promise = null)
		{
			this.func = func;
			this.promise = promise ?? new TaskCompletionSource<T>();
		}
	}

	private readonly object _tasksLock = new object();

	private readonly Thread[] _threads;

	private bool _shutdown;

	private bool _disposed;

	private readonly Queue<IQueuedTask> _tasks = new Queue<IQueuedTask>();

	public Task<T> AddTask<T>(Func<T> func)
	{
		if (_shutdown)
		{
			throw new InvalidOperationException("Tried to add task while shut down");
		}
		QueuedTask<T> queuedTask = new QueuedTask<T>(func);
		lock (_tasksLock)
		{
			_tasks.Enqueue(queuedTask);
			Monitor.Pulse(_tasksLock);
		}
		return queuedTask.promise.Task;
	}

	public int GetCount()
	{
		lock (_tasksLock)
		{
			return _tasks.Count;
		}
	}

	public void Clear()
	{
		int count;
		lock (_tasksLock)
		{
			count = _tasks.Count;
			_tasks.Clear();
		}
		Log.Message(string.Format("[Verse.MultiThreadUtility] Cleared {0} tasks in task pool {1}", count, "TaskPool"));
	}

	public TaskPool(int threadCount)
	{
		_threads = new Thread[threadCount];
		for (int i = 0; i < threadCount; i++)
		{
			_threads[i] = new Thread(Threads)
			{
				IsBackground = true,
				Name = $"Thread{i}"
			};
			_threads[i].Start();
		}
		_tasks = new Queue<IQueuedTask>();
	}

	private void Threads()
	{
		while (true)
		{
			IQueuedTask queuedTask = null;
			lock (_tasksLock)
			{
				while (!_shutdown && _tasks.Count == 0)
				{
					Monitor.Wait(_tasksLock);
				}
				if (_shutdown && _tasks.Count == 0)
				{
					break;
				}
				queuedTask = _tasks.Dequeue();
			}
			try
			{
				queuedTask.Run();
			}
			catch (Exception arg)
			{
				Log.Error($"[Verse.MultiThreadUtility] Task execution error in thread {Thread.CurrentThread.Name}: {arg}");
			}
		}
	}

	public Task AddTask(Action action)
	{
		if (_shutdown)
		{
			throw new InvalidOperationException("Tried to add task while shut down");
		}
		TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
		QueuedTask<bool> item = new QueuedTask<bool>(delegate
		{
			action();
			return true;
		}, taskCompletionSource);
		lock (_tasksLock)
		{
			_tasks.Enqueue(item);
			Monitor.Pulse(_tasksLock);
		}
		return taskCompletionSource.Task;
	}

	public Task AddAsyncTask(Func<Task> func)
	{
		if (_shutdown)
		{
			throw new InvalidOperationException("Tried to add task while shut down");
		}
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		QueuedTask<bool> item = new QueuedTask<bool>(delegate
		{
			func().ContinueWith(delegate(Task t)
			{
				if (t.IsFaulted)
				{
					tcs.TrySetException(t.Exception);
				}
				else if (t.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					tcs.TrySetResult(result: true);
				}
			});
			return true;
		}, tcs);
		lock (_tasksLock)
		{
			_tasks.Enqueue(item);
			Monitor.Pulse(_tasksLock);
		}
		return tcs.Task;
	}

	public void Shutdown()
	{
		lock (_tasksLock)
		{
			if (_shutdown)
			{
				return;
			}
			_shutdown = true;
			Monitor.PulseAll(_tasksLock);
		}
		Thread[] threads = _threads;
		for (int i = 0; i < threads.Length; i++)
		{
			threads[i].Join();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				Shutdown();
			}
			_disposed = true;
		}
	}

	~TaskPool()
	{
		Dispose(disposing: false);
	}
}
