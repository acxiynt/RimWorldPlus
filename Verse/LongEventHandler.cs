using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RimWorld;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Verse;

public static class LongEventHandler
{
	private class QueuedLongEvent
	{
		public Action eventAction;

		public Action callback;

		public IEnumerator eventActionEnumerator;

		public string levelToLoad;

		public string eventTextKey = "";

		public string eventText = "";

		public bool doAsynchronously;

		public Action<Exception> exceptionHandler;

		public bool alreadyDisplayed;

		public bool canEverUseStandardWindow = true;

		public bool showExtraUIInfo = true;

		public bool UseAnimatedDots
		{
			get
			{
				if (!doAsynchronously)
				{
					return eventActionEnumerator != null;
				}
				return true;
			}
		}

		public bool ShouldWaitUntilDisplayed
		{
			get
			{
				if (!alreadyDisplayed && UseStandardWindow)
				{
					return !eventText.NullOrEmpty();
				}
				return false;
			}
		}

		public bool UseStandardWindow
		{
			get
			{
				if (canEverUseStandardWindow && !doAsynchronously)
				{
					return eventActionEnumerator == null;
				}
				return false;
			}
		}
	}

	private static Queue<QueuedLongEvent> eventQueue = new Queue<QueuedLongEvent>();

	private static QueuedLongEvent currentEvent = null;

	private static Thread eventThread = null;

	private static AsyncOperation levelLoadOp = null;

	private static List<Action> toExecuteWhenFinished = new List<Action>();

	private static bool executingToExecuteWhenFinished = false;

	private static readonly object CurrentEventTextLock = new object();

	private static readonly Vector2 StatusRectSize = new Vector2(240f, 75f);

	public static bool ShouldWaitForEvent
	{
		get
		{
			if (!AnyEventNowOrWaiting)
			{
				return false;
			}
			if (currentEvent != null && !currentEvent.UseStandardWindow)
			{
				return true;
			}
			if (Find.UIRoot == null || Find.WindowStack == null)
			{
				return true;
			}
			return false;
		}
	}

	public static bool AnyEventNowOrWaiting
	{
		get
		{
			if (currentEvent == null)
			{
				return eventQueue.Count > 0;
			}
			return true;
		}
	}

	public static bool AnyEventWhichDoesntUseStandardWindowNowOrWaiting
	{
		get
		{
			QueuedLongEvent queuedLongEvent = currentEvent;
			if (queuedLongEvent != null && !queuedLongEvent.UseStandardWindow)
			{
				return true;
			}
			return eventQueue.Any((QueuedLongEvent x) => !x.UseStandardWindow);
		}
	}

	public static bool ForcePause => AnyEventNowOrWaiting;

	public static void QueueLongEvent(Action action, string textKey, bool doAsynchronously, Action<Exception> exceptionHandler, bool showExtraUIInfo = true, Action callback = null)
	{
		QueuedLongEvent item = new QueuedLongEvent
		{
			eventAction = action,
			eventTextKey = textKey,
			doAsynchronously = doAsynchronously,
			exceptionHandler = exceptionHandler,
			canEverUseStandardWindow = !AnyEventWhichDoesntUseStandardWindowNowOrWaiting,
			showExtraUIInfo = showExtraUIInfo,
			callback = callback
		};
		eventQueue.Enqueue(item);
	}

	public static void QueueLongEvent(IEnumerable action, string textKey, Action<Exception> exceptionHandler = null, bool showExtraUIInfo = true)
	{
		QueuedLongEvent queuedLongEvent = new QueuedLongEvent();
		queuedLongEvent.eventActionEnumerator = action.GetEnumerator();
		queuedLongEvent.eventTextKey = textKey;
		queuedLongEvent.doAsynchronously = false;
		queuedLongEvent.exceptionHandler = exceptionHandler;
		queuedLongEvent.canEverUseStandardWindow = !AnyEventWhichDoesntUseStandardWindowNowOrWaiting;
		queuedLongEvent.showExtraUIInfo = showExtraUIInfo;
		eventQueue.Enqueue(queuedLongEvent);
	}

	public static void QueueLongEvent(Action preLoadLevelAction, string levelToLoad, string textKey, bool doAsynchronously, Action<Exception> exceptionHandler, bool showExtraUIInfo = true)
	{
		QueuedLongEvent queuedLongEvent = new QueuedLongEvent();
		queuedLongEvent.eventAction = preLoadLevelAction;
		queuedLongEvent.levelToLoad = levelToLoad;
		queuedLongEvent.eventTextKey = textKey;
		queuedLongEvent.doAsynchronously = doAsynchronously;
		queuedLongEvent.exceptionHandler = exceptionHandler;
		queuedLongEvent.canEverUseStandardWindow = !AnyEventWhichDoesntUseStandardWindowNowOrWaiting;
		queuedLongEvent.showExtraUIInfo = showExtraUIInfo;
		eventQueue.Enqueue(queuedLongEvent);
	}

	public static void ClearQueuedEvents()
	{
		eventQueue.Clear();
	}

	public static void LongEventsOnGUI()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		if (currentEvent == null)
		{
			GameplayTipWindow.ResetTipTimer();
			return;
		}
		float num = StatusRectSize.x;
		lock (CurrentEventTextLock)
		{
			Text.Font = GameFont.Small;
			num = Mathf.Max(num, Text.CalcSize(currentEvent.eventText + "...").x + 40f);
		}
		bool flag = Find.UIRoot != null && !currentEvent.UseStandardWindow && currentEvent.showExtraUIInfo;
		bool flag2 = Find.UIRoot != null && Current.Game != null && !currentEvent.UseStandardWindow && currentEvent.showExtraUIInfo;
		Vector2 val = (flag2 ? ModSummaryWindow.GetEffectiveSize() : Vector2.zero);
		float num2 = StatusRectSize.y;
		if (flag2)
		{
			num2 += 17f + val.y;
		}
		if (flag)
		{
			num2 += 17f + GameplayTipWindow.WindowSize.y;
		}
		float num3 = ((float)UI.screenHeight - num2) / 2f;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(((float)UI.screenWidth - GameplayTipWindow.WindowSize.x) / 2f, num3 + StatusRectSize.y + 17f);
		Vector2 offset = default(Vector2);
		((Vector2)(ref offset))._002Ector(((float)UI.screenWidth - val.x) / 2f, val2.y + GameplayTipWindow.WindowSize.y + 17f);
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((float)UI.screenWidth - num) / 2f, num3, num, StatusRectSize.y);
		val3 = val3.Rounded();
		if (!currentEvent.UseStandardWindow || Find.UIRoot == null || Find.WindowStack == null)
		{
			if (UIMenuBackgroundManager.background == null)
			{
				UIMenuBackgroundManager.background = new UI_BackgroundMain();
			}
			UIMenuBackgroundManager.background.BackgroundOnGUI();
			Widgets.DrawShadowAround(val3);
			Widgets.DrawWindowBackground(val3);
			DrawLongEventWindowContents(val3);
			if (flag)
			{
				GameplayTipWindow.DrawWindow(val2, useWindowStack: false);
			}
			if (flag2)
			{
				ModSummaryWindow.DrawWindow(offset, useWindowStack: false);
				TooltipHandler.DoTooltipGUI();
			}
		}
		else
		{
			DrawLongEventWindow(val3);
			if (flag)
			{
				GameplayTipWindow.DrawWindow(val2, useWindowStack: true);
			}
		}
	}

	private static void DrawLongEventWindow(Rect statusRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Find.WindowStack.ImmediateWindow(62893994, statusRect, WindowLayer.Super, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			DrawLongEventWindowContents(statusRect.AtZero());
		});
	}

	public static void LongEventsUpdate(out bool sceneChanged)
	{
		sceneChanged = false;
		if (currentEvent != null)
		{
			if (currentEvent.eventActionEnumerator != null)
			{
				UpdateCurrentEnumeratorEvent();
			}
			else if (currentEvent.doAsynchronously)
			{
				UpdateCurrentAsynchronousEvent();
			}
			else
			{
				UpdateCurrentSynchronousEvent(out sceneChanged);
			}
		}
		if (currentEvent == null && eventQueue.Count > 0)
		{
			currentEvent = eventQueue.Dequeue();
			if (currentEvent.eventTextKey == null)
			{
				currentEvent.eventText = "";
			}
			else
			{
				currentEvent.eventText = currentEvent.eventTextKey.Translate();
			}
		}
	}

	public static void ExecuteWhenFinished(Action action)
	{
		toExecuteWhenFinished.Add(action);
		if ((currentEvent == null || currentEvent.ShouldWaitUntilDisplayed) && !executingToExecuteWhenFinished)
		{
			ExecuteToExecuteWhenFinished();
		}
	}

	public static void ForceExecuteToExecuteWhenFinished()
	{
		ExecuteToExecuteWhenFinished();
	}

	public static void SetCurrentEventText(string newText)
	{
		lock (CurrentEventTextLock)
		{
			if (currentEvent != null)
			{
				currentEvent.eventText = newText;
			}
		}
	}

	private static void UpdateCurrentEnumeratorEvent()
	{
		try
		{
			float num = Time.realtimeSinceStartup + 0.1f;
			do
			{
				if (!currentEvent.eventActionEnumerator.MoveNext())
				{
					if (currentEvent.eventActionEnumerator is IDisposable disposable)
					{
						disposable.Dispose();
					}
					currentEvent = null;
					eventThread = null;
					levelLoadOp = null;
					ExecuteToExecuteWhenFinished();
					break;
				}
			}
			while (!(num <= Time.realtimeSinceStartup));
		}
		catch (Exception ex)
		{
			Log.Error("Exception from long event: " + ex);
			if (currentEvent != null)
			{
				if (currentEvent.eventActionEnumerator is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
				if (currentEvent.exceptionHandler != null)
				{
					currentEvent.exceptionHandler(ex);
				}
			}
			currentEvent = null;
			eventThread = null;
			levelLoadOp = null;
		}
	}

	private static void UpdateCurrentAsynchronousEvent()
	{
		if (eventThread == null)
		{
			eventThread = new Thread((ThreadStart)delegate
			{
				RunEventFromAnotherThread(currentEvent.eventAction);
			});
			eventThread.Start();
		}
		else
		{
			if (eventThread.IsAlive)
			{
				return;
			}
			bool flag = false;
			if (!currentEvent.levelToLoad.NullOrEmpty())
			{
				if (levelLoadOp == null)
				{
					levelLoadOp = SceneManager.LoadSceneAsync(currentEvent.levelToLoad);
				}
				else if (levelLoadOp.isDone)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				ExecuteToExecuteWhenFinished();
				currentEvent.callback?.Invoke();
				currentEvent = null;
				eventThread = null;
				levelLoadOp = null;
			}
		}
	}

	private static void UpdateCurrentSynchronousEvent(out bool sceneChanged)
	{
		sceneChanged = false;
		if (currentEvent.ShouldWaitUntilDisplayed)
		{
			return;
		}
		try
		{
			if (currentEvent.eventAction != null)
			{
				currentEvent.eventAction();
			}
			if (!currentEvent.levelToLoad.NullOrEmpty())
			{
				SceneManager.LoadScene(currentEvent.levelToLoad);
				sceneChanged = true;
			}
			currentEvent = null;
			eventThread = null;
			levelLoadOp = null;
			ExecuteToExecuteWhenFinished();
		}
		catch (Exception ex)
		{
			Log.Error("Exception from long event: " + ex);
			if (currentEvent != null && currentEvent.exceptionHandler != null)
			{
				currentEvent.exceptionHandler(ex);
			}
			currentEvent = null;
			eventThread = null;
			levelLoadOp = null;
		}
	}

	private static void RunEventFromAnotherThread(Action action)
	{
		CultureInfoUtility.EnsureEnglish();
		try
		{
			action?.Invoke();
		}
		catch (Exception ex)
		{
			Log.Error("Exception from asynchronous event: " + ex);
			try
			{
				if (currentEvent != null && currentEvent.exceptionHandler != null)
				{
					currentEvent.exceptionHandler(ex);
				}
			}
			catch (Exception ex2)
			{
				Log.Error("Exception was thrown while trying to handle exception. Exception: " + ex2);
			}
		}
	}

	private static void ExecuteToExecuteWhenFinished()
	{
		if (executingToExecuteWhenFinished)
		{
			Log.Warning("Already executing.");
			return;
		}
		executingToExecuteWhenFinished = true;
		if (toExecuteWhenFinished.Count > 0)
		{
			DeepProfiler.Start("ExecuteToExecuteWhenFinished()");
		}
		for (int i = 0; i < toExecuteWhenFinished.Count; i++)
		{
			DeepProfiler.Start(toExecuteWhenFinished[i].Method.DeclaringType.ToString() + " -> " + toExecuteWhenFinished[i].Method.ToString());
			try
			{
				toExecuteWhenFinished[i]();
			}
			catch (Exception ex)
			{
				Log.Error("Could not execute post-long-event action. Exception: " + ex);
			}
			finally
			{
				DeepProfiler.End();
			}
		}
		if (toExecuteWhenFinished.Count > 0)
		{
			DeepProfiler.End();
		}
		toExecuteWhenFinished.Clear();
		executingToExecuteWhenFinished = false;
	}

	private static void DrawLongEventWindowContents(Rect rect)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (currentEvent == null)
		{
			return;
		}
		if ((int)Event.current.type == 7)
		{
			currentEvent.alreadyDisplayed = true;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		float num = 0f;
		if (levelLoadOp != null)
		{
			float f = 1f;
			if (!levelLoadOp.isDone)
			{
				f = levelLoadOp.progress;
			}
			TaggedString taggedString = "LoadingAssets".Translate() + " " + f.ToStringPercent();
			num = Text.CalcSize(taggedString).x;
			Widgets.Label(rect, taggedString);
		}
		else
		{
			lock (CurrentEventTextLock)
			{
				num = Text.CalcSize(currentEvent.eventText).x;
				Widgets.Label(rect, currentEvent.eventText);
			}
		}
		Text.Anchor = (TextAnchor)3;
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).center.x + num / 2f;
		Widgets.Label(rect, (!currentEvent.UseAnimatedDots) ? "..." : GenText.MarchingEllipsis());
		Text.Anchor = (TextAnchor)0;
	}
}
