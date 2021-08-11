using System;
using System.Collections.Generic;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: InitializeAfterPlugins(typeof(TaskWindow))]

namespace Ludiq.PeekCore
{
	public sealed class TaskWindow : EditorWindow
	{
		public static TaskWindow instance { get; internal set; }
		
		internal readonly List<Task> tasks = new List<Task>();
		
		private DateTime lastRepaintTime;

		private float progressAnimationSpeed = 2f; // bar per second

		private float repaintDeltaTime => (float)(DateTime.UtcNow - lastRepaintTime).TotalSeconds;


		#region Lifecycle

		private bool shown;

		private bool close;
		
		private void OnEnable()
		{
			instance = this;
		}

		private void OnShow()
		{
			this.Center();
		}

		public void OnGUI()
		{
			try
			{
				if (Event.current.type == EventType.Layout && close)
				{
					base.Close();
					return;
				}

				if (!shown)
				{
					OnShow();
					shown = true;
				}

				UnityAPI.Process();

				Draw();
				Repaint();
				Resize();
				UpdateTip();

				if (Event.current.type == EventType.Repaint)
				{
					lastRepaintTime = DateTime.UtcNow;
				}
			}
			catch (ExitGUIException) { }
			catch (Exception ex)
			{
				base.Close();
				Debug.LogException(ex);
			}
		}

		public new void Close()
		{
			close = true;
		}
		
		private void OnDisable()
		{
			instance = null;

			foreach (var task in tasks)
			{
				task.Cancel();
			}

			ConsoleProfiler.Dump();
		}

		#endregion


		#region Tips
		
		private string tip;

		private DateTime tipSwitchTime;
		
		private void UpdateTip()
		{
			if (DateTime.UtcNow > tipSwitchTime)
			{
				if (Tips.any)
				{
					tip = Tips.random;
					tipSwitchTime = DateTime.UtcNow + Tips.TimeToRead(tip);
				}
				else
				{
					tip = null;
				}
			}
		}

		#endregion


		#region Drawing

		private readonly List<Task> drawnTasks = new List<Task>();

		private string drawnTip;

		private void UpdateDrawnElements()
		{
			drawnTip = tip;

			drawnTasks.Clear();

			lock (tasks)
			{
				drawnTasks.AddRange(tasks);
			}
		}

		private void Draw()
		{
			if (Event.current.type == EventType.Layout)
			{
				UpdateDrawnElements();
			}

			LudiqGUI.BeginVertical(Styles.background);
			
			LudiqGUI.BeginHorizontal(Styles.tipArea, GUILayout.Height(Styles.tipHeight));

			LudiqGUI.BeginVertical();
			LudiqGUI.FlexibleSpace();
			LudiqGUI.LoaderLayout();
			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndVertical();

			if (drawnTip != null)
			{
				LudiqGUI.Space(Styles.spaceBetweenSpinnerAndTip);

				LudiqGUI.BeginVertical();
				LudiqGUI.FlexibleSpace();
				GUILayout.Label("<b>Tip:</b> " + drawnTip, Styles.tip);
				LudiqGUI.FlexibleSpace();
				LudiqGUI.EndVertical();
			}

			LudiqGUI.EndHorizontal();

			if (drawnTasks.Count > 0)
			{
				var first = true;

				foreach (var task in drawnTasks)
				{
					if (!first)
					{
						GUILayout.Space(Styles.spaceBetweenTasks);
					}
					
					if (Event.current.type == EventType.Layout)
					{
						task.animatedRatio = Mathf.MoveTowards(task.animatedRatio, task.ratio, progressAnimationSpeed * repaintDeltaTime);
					}
					
					LudiqGUI.BeginVertical(Styles.task);

					GUILayout.Label(task.title + $" <color=#{ColorPalette.unityForegroundDim.ToHexString()}>({task.elapsed.TotalSeconds:###0.0} seconds)</color>", Styles.taskTitle);

					var progressBarPosition = EditorGUILayout.GetControlRect();
						
					EditorGUI.ProgressBar(progressBarPosition, task.animatedRatio, null);

					if (task.stepsHaveStarted)
					{
						var currentItemPosition = new Rect
						(
							progressBarPosition.x + (int)(task.animatedRatio * progressBarPosition.width) - 1,
							progressBarPosition.y,
							Mathf.Max(3, Mathf.CeilToInt((1f / task.totalSteps) * (progressBarPosition.width)) + 1),
							progressBarPosition.height
						);

						if (Event.current.type == EventType.Repaint)
						{
							var opacity = Mathf.Lerp(0.25f, 0.25f, (Mathf.Sin((float)EditorApplication.timeSinceStartup * 3) + 1) / 2);

							using (LudiqGUI.color.Override(LudiqGUI.color.value.WithAlphaMultiplied(opacity)))
							{
								Styles.currentItemFill.Draw(currentItemPosition, false, false, false, false);
							}
						}

						LudiqGUI.DrawEmptyRect(currentItemPosition, Styles.currentItemBorder);

						if (Event.current.type == EventType.Repaint)
						{
							Styles.currentItemText.Draw(progressBarPosition, new GUIContent(task.currentStepLabel), false, false, false, false);
						}
					}

					LudiqGUI.EndVertical();

					first = false;
				}
			}

			LudiqGUI.EndVertical();
		}

		private void Resize()
		{
			var width = Styles.width;
			var height = Styles.tipHeight + (drawnTasks.Count * Styles.groupHeight);

			minSize = maxSize = new Vector2(width, height);
		}

		#endregion
		


		
		public static class Styles
		{
			static Styles()
			{
				background = new GUIStyle(LudiqStyles.windowBackground);
				background.padding = new RectOffset(0, 0, 0, 0);

				taskTitle = new GUIStyle(EditorStyles.label);
				taskTitle.richText = true;

				task = new GUIStyle("IN BigTitle");
				task.margin = new RectOffset(0, 0, 0, 0);
				task.padding = new RectOffset(12, 12, 12, 12);

				tipArea = new GUIStyle(LudiqStyles.windowBackground);
				tipArea.margin = new RectOffset(0, 0, 0, 0);
				tipArea.padding = new RectOffset(25, 25, 0, 0);

				tip = new GUIStyle(EditorStyles.label);
				tip.wordWrap = true;
				tip.richText = true;
				tip.alignment = TextAnchor.MiddleLeft;
				tip.stretchWidth = true;

				currentItemFill = new GUIStyle("ProgressBarBar");
				currentItemText = new GUIStyle("ProgressBarText");
			}

			public static readonly GUIStyle background;
			public static readonly GUIStyle taskTitle;
			public static readonly GUIStyle task;
			public static readonly GUIStyle tipArea;
			public static readonly GUIStyle tip;
			public static readonly float spaceBetweenTasks = -1;
			public static readonly float spaceBetweenSpinnerAndTip = 20;
			
			public static readonly float width = 370;
			public static readonly float groupHeight = 57;
			public static readonly float tipHeight = 80;

			public static readonly GUIStyle currentItemFill;
			public static readonly GUIStyle currentItemText;
			public static readonly SkinnedColor currentItemBorder = new SkinnedColor(ColorUtility.Gray(0.57f), ColorUtility.Gray(0.16f));
		}
	}
}
