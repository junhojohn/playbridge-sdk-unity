using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 1. com.unity3d.player.UnityPlayer 클래스에 접근
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // 2. 정적(static) 필드인 currentActivity를 AndroidJavaObject로 가져옴
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (currentActivity != null)
                {
                    Debug.Log("Android Current Activity를 성공적으로 가져왔습니다.");

                    // 3. PlayBridgeBannerView 생성자에 currentActivity를 파라미터로 전달
                    // Java의 `new PlayBridgeBannerView(currentActivity)` 와 같은 동작입니다.
                    AndroidJavaObject androidJavaObject = new AndroidJavaObject("kr.co.playbridge.sdk.ads.PlayBridgeBannerView", currentActivity);
                    
                    // 4. 메서드 호출
                    androidJavaObject.Call("load");
                    androidJavaObject.Call("show");
                }
                else
                {
                    Debug.LogError("currentActivity를 가져오지 못했습니다.");
                }
            }
        }
#else
        Debug.LogWarning("안드로이드 환경이 아니거나 에디터 상태입니다.");
#endif
    }

    void Update()
    {
        
    }
}
