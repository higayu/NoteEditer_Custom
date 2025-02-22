using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UIを扱うために追加

public class Exit : MonoBehaviour
{
    public GameObject endButton; // ボタンのGameObject
    private Button buttonComponent; // Buttonコンポーネント

    void Start()
    {
        if (endButton != null)
        {
            buttonComponent = endButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(Exit_Click); // クリックイベントを登録
            } else
            {
                Debug.LogError("endButtonにButtonコンポーネントがアタッチされていません！");
            }
        } else
        {
            Debug.LogError("endButtonが設定されていません！");
        }
    }

    public void Exit_Click()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }
}
