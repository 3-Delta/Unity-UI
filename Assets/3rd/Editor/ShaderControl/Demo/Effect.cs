using UnityEngine;
using System.Collections;

namespace ShaderControl {

	[ExecuteInEditMode]
	public class Effect : MonoBehaviour {

		Material mat;
		string[] keywords = new string[] {
			"ENABLE_RED_CHANNEL",
			"ENABLE_GREEN_CHANNEL",
			"ENABLE_BLUE_CHANNEL"
		};

		void Start () {
			mat = Resources.Load<Material> ("ChromaScreen") as Material;
		}

		void OnRenderImage (RenderTexture source, RenderTexture destination) {
			mat.shaderKeywords = keywords;
			Graphics.Blit (source, destination, mat);
		}
	
	}

}