using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/*
 https://blog.uwa4d.com/archives/Severe_MOBA.html
 https://edu.uwa4d.com/lesson-detail/172/960/0?isPreview=0
 https://zhuanlan.zhihu.com/p/344751308
 https://blog.uwa4d.com/archives/TechSharing_137.html
 
 SrcColorFactor DestColorFactory | SrcAlphaFactor DestAlphaFactor
                         A   B   |    C   D
                         E   F   |    G   H
                        
                         A   B   |    C   D
                         1   B   |    1   D
                         
    但是呢 存在问题就是我们需要知道B, D是多少，否则一切无从谈起。我们可以固定渲染到RT的元素的B， D必须是固定的。
*/

// 用于3d模型或者半透明渲染到RT中，然后RT的image和screenColor混合的方式
public class RTImage : RawImage {
    public void Set(Renderer renderer) {
        if (renderer != null) {
            Set(renderer.sharedMaterial);
        }
    }

    public void Set(Material mat) {
        if (mat != null) {
            Set((BlendMode) mat.GetFloat("_DestColorFactor"), (BlendMode) mat.GetFloat("_DestAlphaFactor"));
        }
    }

    public void Set(BlendMode destColorFactor, BlendMode destAlphaFactor) {
        material = Resources.Load<Material>("Material/UI/RTImage");
        if (material != null) {
            material.SetFloat("_DestColorFactor", (byte) destColorFactor);
            material.SetFloat("_DestAlphaFactor", (byte) destAlphaFactor);
        }
    }
}
