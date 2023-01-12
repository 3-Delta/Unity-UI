using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoQualityFPS : MonoBehaviour
{
	/// <summary>
	/// Automatically adapt image quality according to frames per second. Version = 1.0.3
	/// </summary>
	[Tooltip("Do you want to see infos in the console?")]
	public bool ShowDebugLog = true;

	[Tooltip("Below that target (in frames per second), the quality will automatically decrease")]
	public float TargetFPS_min = 75.0f; //in frames per second

	[Tooltip("Over that target (in frames per second), the quality will automatically increase")]
	public float TargetFPS_max = 105.0f; //in frames per second

	[Tooltip("Time to wait before FPS is more stable (in seconds)")]
	public float DelayBeforeStarting = 5.0f; //in seconds

	[Tooltip("While we're under min FPS or over max FPS, check FPS and quality every __ seconds")]
	public float AtStart_CheckEvery = 2.0f; //in seconds

	[Tooltip("If the perfect quality and FPS is reached, we now check FPS and quality every __ seconds")]
	public float Then_WhenItsStable_CheckEvery = 30.0f; //in seconds

	private bool StayOnTheMostUsedQualitySettingsAfterTheTimeBelow; //Do you want to stay on the most used quality settings after some time? (in seconds)

	[Tooltip("If > 0, the most used quality settings will be chosen after __ seconds")]
	public float StayOnTheMostUsedQualitySettingsAfter = 120.0f; //in seconds

	private int[] SaveHowManyTimesTheCurrentQualityWasUsed_Array;

	[Tooltip("Should expensive quality changes be applied? Like anti-aliasing etc")]
	public bool ApplyExpensiveQualityChanges = true; //Should expensive changes be applied? Like anti-aliasing etc

	private int Current_Frames; //in frames
	private float Current_Time; //in seconds
	private float Total_Time; //in seconds

	private bool OK_To_Start_Counting;
	private float Currently_CheckEvery; //in seconds
	
	[HideInInspector]
	public float Current_FPS; //in frames per second

	private int Current_Quality;
	
	private bool ItsTimeToSkipFrames_ToLetTheNewQualityApply = false;
	private int CurrentFramesSkiped_AfterTheQualityHasChanged = 0;
	private int FramesToSkip_AfterTheQualityHasChanged = 5;

	void Start ()
	{
		//Initializing variables
		OK_To_Start_Counting = false;
		Current_Frames = 0;
		Current_Time = 0.0f;
		Total_Time = DelayBeforeStarting;
		Currently_CheckEvery = AtStart_CheckEvery;
		ItsTimeToSkipFrames_ToLetTheNewQualityApply = false;
		CurrentFramesSkiped_AfterTheQualityHasChanged = 0;

		//If we want to stay on the most used quality settings
		if(StayOnTheMostUsedQualitySettingsAfter > 0)
		{
			//We keep that information
			StayOnTheMostUsedQualitySettingsAfterTheTimeBelow = true;

			//We create the array used to count how many times a quality settings is used
			SaveHowManyTimesTheCurrentQualityWasUsed_Array = new int[QualitySettings.names.Length];
		}
		else //Continually check FPS and adapt quality
		{
			//We keep that information
			StayOnTheMostUsedQualitySettingsAfterTheTimeBelow = false;
		}

		//Main coroutine
		StartCoroutine(LetsCheckFPS());
	}


	void Update ()
	{
		//When FPS is stable
		if(OK_To_Start_Counting)
		{
			//If it's not yet the time to skip frames (to let the new quality apply)
			if(!ItsTimeToSkipFrames_ToLetTheNewQualityApply)
			{
				//We start counting frames over time
				Current_Frames++;
				Current_Time += Time.deltaTime;

				//If it's time to check the current fps
				if(Current_Time >= Currently_CheckEvery)
				{
					//Frames per second
					Current_FPS = Current_Frames/Current_Time;
					//Current Quality
					Current_Quality = QualitySettings.GetQualityLevel();

					if(ShowDebugLog) { Debug.Log("Current FPS = " + Current_FPS); }

					//If fps is below the minimum fps target
					if(Current_FPS < TargetFPS_min)
					{
						//Decreasing Quality Level
						QualitySettings.DecreaseLevel(ApplyExpensiveQualityChanges);

						//If the quality has changed
						if(Current_Quality != QualitySettings.GetQualityLevel())
						{
							ItsTimeToSkipFrames_ToLetTheNewQualityApply = true;

							//If we want to show some informations in the console
							if(ShowDebugLog) { Debug.Log("Decreasing Quality Level from (" + Current_Quality + ") to (" + QualitySettings.GetQualityLevel() + ")"); }
						}
						else //Current_Quality == QualitySettings.GetQualityLevel()
						{
							//If we want to show some informations in the console
							if(ShowDebugLog) { Debug.Log("Currently in the lowest quality level (" + Current_Quality + ")"); }
						}

						//We count how many times a quality settings is used
						SaveCurrentQualitySettings();
					}
					else
					{
						//If fps is over the maximum fps target
						if(Current_FPS > TargetFPS_max)
						{
							//Increasing Quality Level
							QualitySettings.IncreaseLevel(ApplyExpensiveQualityChanges);

							//If the quality has changed
							if(Current_Quality != QualitySettings.GetQualityLevel())
							{
								ItsTimeToSkipFrames_ToLetTheNewQualityApply = true;

								//If we want to show some informations in the console
								if(ShowDebugLog) { Debug.Log("Increasing Quality Level from (" + Current_Quality + ") to (" + QualitySettings.GetQualityLevel() + ")"); }
							}
							else //Current_Quality == QualitySettings.GetQualityLevel()
							{
								//If we want to show some informations in the console
								if(ShowDebugLog) { Debug.Log("Currently in the highest quality level (" + Current_Quality + ")"); }
							}

							//We count how many times a quality settings is used
							SaveCurrentQualitySettings();
						}
						else //Between min and max fps
						{
							if(ShowDebugLog) { Debug.Log("Between min and max fps: No changes to the current quality level (" + Current_Quality + ")"); }
							Currently_CheckEvery = Then_WhenItsStable_CheckEvery;
							SaveCurrentQualitySettings();
						}
					}

					//Initializing variables
					Total_Time += Current_Time;
					Current_Frames = 0;
					Current_Time = 0.0f;
				}

				//If we've reached the total time
				if(StayOnTheMostUsedQualitySettingsAfterTheTimeBelow && Total_Time >= StayOnTheMostUsedQualitySettingsAfter)
				{
					//We stop counting
					OK_To_Start_Counting = false;
					StayOnTheMostUsedQualitySettings();
				}
			}
			else //Now it's time to skip frames (to let the new quality apply)
			{
				//Increase the frame counter
				CurrentFramesSkiped_AfterTheQualityHasChanged++;

				//If enough frames were skipped
				if(CurrentFramesSkiped_AfterTheQualityHasChanged >= FramesToSkip_AfterTheQualityHasChanged)
				{
					//We can continue counting frames
					ItsTimeToSkipFrames_ToLetTheNewQualityApply = false;
					CurrentFramesSkiped_AfterTheQualityHasChanged = 0;
				}
			}
		}
	}


	//We count how many times a quality settings is used
	void SaveCurrentQualitySettings()
	{
		if(StayOnTheMostUsedQualitySettingsAfterTheTimeBelow)
		{
			SaveHowManyTimesTheCurrentQualityWasUsed_Array[QualitySettings.GetQualityLevel()] += 1;
		}
	}


	//We choose the most used quality settings
	void StayOnTheMostUsedQualitySettings()
	{
		int save_cpt = 0;
		int cpt = 1;

		//While the array is not read to the end
		while(cpt < SaveHowManyTimesTheCurrentQualityWasUsed_Array.Length)
		{
			//If the "saved quality" is used less often than the "current quality"
			if(SaveHowManyTimesTheCurrentQualityWasUsed_Array[save_cpt] < SaveHowManyTimesTheCurrentQualityWasUsed_Array[cpt])
			{
				//Then we save the "current quality"
				save_cpt = cpt;
			}

			//Next line
			cpt++;
		}

		if(ShowDebugLog) { Debug.Log("The most used quality level is (" + save_cpt + ")."); }

		//If the current quality level is NOT the most used quality level
		if(QualitySettings.GetQualityLevel() != save_cpt)
		{
			if(ShowDebugLog) { Debug.Log("Changing quality level from (" + QualitySettings.GetQualityLevel() + ") to (" + save_cpt + ")."); }

			//We change the current quality to the most used quality
			QualitySettings.SetQualityLevel(save_cpt, ApplyExpensiveQualityChanges);
		}
		else //the current quality level is the most used quality level
		{
			if(ShowDebugLog) { Debug.Log("And that's the current quality level."); }
		}

		//We disable the script
		this.enabled = false;
	}


	//Main coroutine
	IEnumerator LetsCheckFPS()
	{
		//We wait for FPS to be more stable
		yield return new WaitForSeconds(DelayBeforeStarting);

		//It's OK to start counting FPS
		OK_To_Start_Counting = true;
	}


}
