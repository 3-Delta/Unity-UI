using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lowest_Quality_At_Start : MonoBehaviour
{
	void OnEnable()
	{
		QualitySettings.SetQualityLevel(2,true);
		QualitySettings.maximumLODLevel = 0;
		QualitySettings.SetQualityLevel(1,true);
		QualitySettings.maximumLODLevel = 1;
		QualitySettings.SetQualityLevel(0,true);
		QualitySettings.maximumLODLevel = 2;
	}
}

