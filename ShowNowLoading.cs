using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

#if UNITY_EDITOR
// [Unity Editor 時専用処理] 下記 namespace は Editor 専用
using UnityEditor;
#endif


/*:en
 * @addondesc Displays "Now Loading..." when transitioning from the title screen
 * @author Zuisho
*/

/*:ja
 * @addondesc タイトル画面からの遷移時にNowLoading...を表示
 * @author Zuisho
 */

namespace RPGMaker.Codebase.Addon
{
    public class ShowNowLoading
    {
        public static bool IsEnabled { get; private set; }

        // コンストラクタ (アドオン有効化時に呼ばれる)
        public ShowNowLoading(bool isEnabled) {
            Debug.Log("[DEBUG] AddonSample Activated.");
            IsEnabled = isEnabled;

            // 「シーンを跨ぐ常駐」の初期化
            InitializeResidentAddon();
        }

        // ■■■■■ 「シーンを跨ぐ常駐」関連 ■■■■■

        private void InitializeResidentAddon() {
#if UNITY_EDITOR
            // [Unity Editor 時専用処理]
            // Play ボタンが押されたときに
            // アドオンの実行時クラスを登録
            EditorApplication.playModeStateChanged +=
                OnPlayModeStateChanged;
#else
            // [実配布時 (例：Windows なら exe) 専用処理]
            // 既にゲーム起動済のはずなので、直ちに登録
            CreateShowNowLoadingRunner();
#endif
        }

#if UNITY_EDITOR
        // [Unity Editor 時専用処理]
        // Play ボタンが押されたとき用の
        // アドオンの実行時クラスを登録処理
        private static void OnPlayModeStateChanged(
            PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                CreateShowNowLoadingRunner();
            }
        }
#endif

        // アドオンの実行時にシーン読み込み時のアクションを登録する
        private static void CreateShowNowLoadingRunner() {
            SceneManager.sceneLoaded += (scene, LoadSceneMode) =>
            {
                if (SceneManager.GetActiveScene().name == "SceneMap")
                {
                    new GameObject("ShowNowLoadingRunner").AddComponent<ShowNowLoadingRunner>();
                }
            };
                
        }
    }

    /// <summary>
    /// SceneMapシーン読み込み時に2フレームだけNowLoading...の文言を表示する
    /// Displays the "Now Loading..." message for only 2 frames when loading the SceneMap scene
    /// RPGMakerUniteでは読み込みが完了するまでフリーズが発生し、フリーズが解消されるまでNowLoading...が表示され続ける
    /// In RPGMakerUnite, a freeze occurs until the loading is complete, and "Now Loading..." continues to be displayed until the freeze is resolved.
    /// </summary>
    public class ShowNowLoadingRunner : MonoBehaviour
    {
        TextMeshProUGUI _nowLoadingText;

        // ゲーム開始時の処理
        private void Start() {
            Debug.Log("[DEBUG] ShowNowLoadingRunner Started.");

            // NowLoading 表示用キャンバスを作成
            var fpsCanvasParent =
                new GameObject("ShowNowLoadingRunner - Canvas");
            var fpsCanvas =
                fpsCanvasParent.AddComponent<Canvas>();
            var fpsScaler =
                fpsCanvasParent.AddComponent<CanvasScaler>();
            fpsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fpsCanvas.sortingOrder = 10000; // 他の Unite UI より優先
            fpsScaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            fpsScaler.screenMatchMode =
                CanvasScaler.ScreenMatchMode.Expand;
            fpsScaler.referenceResolution = new(1920, 1080);

            // NowLoading 表示用テキストオブジェクトを作成
            _nowLoadingText = new GameObject("ShowNowLoadingRunner - NowLoading Disp")
                .AddComponent<TextMeshProUGUI>();
            _nowLoadingText.fontSize = 80;
            _nowLoadingText.fontWeight = FontWeight.Bold;
            _nowLoadingText.alignment = TextAlignmentOptions.Center;
            _nowLoadingText.enableWordWrapping = false;
            _nowLoadingText.color = new(1, 1, 1, 1);
            _nowLoadingText.UpdateMeshPadding();

            // NowLoading キャンバスの下にテキストオブジェクトをぶら下げる
            _nowLoadingText.transform.SetParent(fpsCanvas.transform);
            _nowLoadingText.rectTransform.localPosition = new Vector3(-480, 480);

            _nowLoadingText.text = "NowLoading...";
            StartCoroutine(DestroyAfterTwoFrame(fpsCanvasParent));
        }

        private IEnumerator DestroyAfterTwoFrame(GameObject fpsCanvasParent) {
            yield return null;
            yield return null;
            Destroy(fpsCanvasParent);
        }
    }
}