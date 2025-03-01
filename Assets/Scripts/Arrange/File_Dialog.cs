using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UIを扱うために追加
using SFB;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using TMPro;
using NoteEditor.Model;
//using Newtonsoft.Json; // Newtonsoft.Json を使用

namespace NoteEditor.DTO
{

    public class File_Dialog : MonoBehaviour
    {
        public GameObject Button; // ボタンのGameObject
        private Button buttonComponent; // Buttonコンポーネント
        [SerializeField] Text selectText;
        [SerializeField] Text messageText = default; // メッセージ表示

        void Start()
        {
            if (Button != null)
            {
                buttonComponent = Button.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(OpenFile); // クリックイベントを登録
                } else
                {
                    Debug.LogError("endButtonにButtonコンポーネントがアタッチされていません！");
                }
            } else
            {
                Debug.LogError("endButtonが設定されていません！");
            }
        }

        // テキストアウトプット
        //[SerializeField] private Text outputText;

        // 読み込んだテキスト
        private string _loadedText = "";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //

    // StandaloneFileBrowserのブラウザスクリプトプラグインから呼び出す
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    // ファイルを開く
    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".", false);
    }

    // ファイルアップロード後の処理
    public void OnFileUpload(string url) {
        StartCoroutine(Load(url));
    }

#else
        //
        // OSビルド & Unity editor上
        //
        public void OnPointerDown(PointerEventData eventData) { }



        // ファイルを開く
        public void OpenFile()
        {
            Debug.Log("譜面データ選択関数実行");

            if (!Loadmanager.instance.WAV_Lode_Flg)
            {
                System.Windows.Forms.MessageBox.Show("先に「.wav」ファイルを選択してください");
                return;
            }

            // Windows のダウンロードフォルダの取得
            string defaultPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

            // 拡張子フィルタ
            var extensions = new[]
            {
        new ExtensionFilter("Text Files", "json"),
        new ExtensionFilter("All Files", "*")
    };

            // ファイルダイアログを開く（初期ディレクトリを `Downloads` に設定）
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultPath, extensions, false);

            if (paths.Length > 0 && paths[0].Length > 0)
            {
                string filePath = paths[0];
                selectText.text = filePath; // 選択したファイルのパスを表示
                Debug.Log("選択したファイルのパス :" + filePath);
                Load(filePath);
            }
        }


#endif


        void Load(string filePath)
        {
            try
            {
                Debug.Log($"File_Dialog: ファイルを読み込んでいます: {filePath}");

                if (!File.Exists(filePath))
                {
                    Debug.LogError($"File_Dialog: ファイルが見つかりません: {filePath}");
                    messageText.text = "ファイルが見つかりません";
                    return;
                }

                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                Debug.Log($"File_Dialog: 読み込んだJSONデータ: \n{json}");

                EditDataSerializer.Deserialize(json); // JSONを `EditData` に適用

                // デシリアライズ後のデータ確認
                Debug.Log($"File_Dialog: デシリアライズ完了。現在の譜面データ: " +
                          $"Name: {EditData.Name.Value}, " +
                          $"BPM: {EditData.BPM.Value}, " +
                          $"Offset: {EditData.OffsetSamples.Value}, " +
                          $"MaxBlock: {EditData.MaxBlock.Value}");

                messageText.text = $"{filePath} を読み込みました";
            } catch (System.Exception e)
            {
                Debug.LogError($"File_Dialog: ロードエラー: {e.Message}\n{e.StackTrace}");
                messageText.text = $"ロードエラー: {e.Message}";
            }
        }



    }
}