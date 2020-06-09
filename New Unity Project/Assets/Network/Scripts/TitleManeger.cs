using UnityEngine;
using UnityEngine.UI;

namespace TitleMenuManager{
   public class TitleMenuManager : MonoBehaviour
    {
        // 入力されたプレイヤー名を保持しておくためのstatic変数
        public static string s_PlayerName = "名無し";
    

        // プレイヤー名入力欄の参照
        InputField m_PlayerNameInputField;
    

        void Start()
        {
            // UIの参照を取得
            m_PlayerNameInputField = GameObject.Find("PlayerNameInputField").GetComponent<InputField>();
        

            // 初期化
            m_PlayerNameInputField.text = s_PlayerName;
        

            // プレイヤー名変更時のイベント登録
            m_PlayerNameInputField.onEndEdit.AddListener(val => s_PlayerName = val);
        }
    } 
}