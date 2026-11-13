using UnityEngine;

// 파일명(PlayBridgeTest.cs)과 클래스명이 같아야 Unity가 컴포넌트로 인식합니다.
public class PlayBridgeTest : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    // GC로 Java 참조가 해제되지 않도록 필드로 보관
    private AndroidJavaObject bannerView;
    private AndroidJavaObject currentActivity;
#endif

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        if (currentActivity == null)
        {
            Debug.LogError("[PlayBridge] currentActivity를 가져오지 못했습니다.");
            return;
        }

        Debug.Log("[PlayBridge] Android Current Activity를 성공적으로 가져왔습니다.");

        // Android View는 반드시 UI 스레드에서 생성/조작해야 합니다.
        // (Unity의 Start()는 Unity 메인 스레드이지 Android UI 스레드가 아님)
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(CreateBanner));
#else
        Debug.LogWarning("[PlayBridge] 안드로이드 환경이 아니거나 에디터 상태입니다.");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void CreateBanner()
    {
        try
        {
            bannerView = new AndroidJavaObject("kr.co.playbridge.sdk.ads.PlayBridgeBannerView", currentActivity);
            bannerView.Call("load");
            bannerView.Call("show");
            Debug.Log("[PlayBridge] 배너 load/show 호출 완료");
        }
        catch (AndroidJavaException e)
        {
            // ClassNotFoundException 이면 SDK(AAR)가 빌드에 포함되지 않은 것
            Debug.LogError("[PlayBridge] 배너 생성 실패: " + e);
        }
    }

    void OnDestroy()
    {
        bannerView?.Dispose();
        currentActivity?.Dispose();
    }
#endif
}
