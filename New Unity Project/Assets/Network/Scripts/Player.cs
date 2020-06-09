using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using UnityEngine.UI; 
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

namespace Network.Scenes.NewScene
{

    public class Player : NetworkBehaviour
    {   
        private bool m_Jump;
        // 歩行速度（メートル/秒）
        [SerializeField] float m_WalkSpeed = 1.5f;

        // 各種コンポーネントの参照
        CharacterController m_CharacterController;
        Animator m_Animator;
        
        // チャットのメッセージ履歴を表示するText
        Text m_ChatHistory;
        // メッセージの入力欄
        InputField m_ChatInputField;

        TextMesh m_PlayerNameText;

        // プレイヤー名
        //[SyncVar]
        //public string m_PlayerName;



        void Start()
        {
            // 各種コンポーネントの参照を取得する
            m_CharacterController = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();

            // メッセージ履歴を表示するTextを検索して取得
            m_ChatHistory = GameObject.Find("ChatHistory").GetComponent<Text>();

            // 頭上のプレイヤー名表示を取得
            //m_PlayerNameText = transform.Find("PlayerNameText").GetComponent<TextMesh>();

            //if (isLocalPlayer){
                // 頭上のプレイヤー名表示は無効に
            //    m_PlayerNameText.gameObject.SetActive(false);
            //}
            //else
            //{
                // 頭上のプレイヤー名表示に文字列を設定
            //    m_PlayerNameText.text = m_PlayerName;
            //}
        }
    
        // ローカルプレイヤーが初期化される際に呼ばれる。
        public override void OnStartLocalPlayer()
        {
            // メッセージ入力欄のInputFieldを検索して取得
            m_ChatInputField = GameObject.Find("ChatInputField").GetComponent<InputField>();
            //カメラのTargetを指定
            //Camera.main.GetComponent<FollowTarget>().target = this.transform;
        }

        void Update()
        {

            
            // ローカルプレイヤー以外では何もしない
            if (!isLocalPlayer){
                gameObject.GetComponent<Camera>().enabled = false;
                gameObject.GetComponent<AudioListener>().enabled = false;
            }
                

            // 移動量(メートル/秒)
            Vector3 motion = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * m_WalkSpeed;

            // 左シフトキーでダッシュ（2倍速）
            if (Input.GetKey(KeyCode.LeftShift))
            {
                motion *= 2f;
            }

            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            // CharacterControllerで移動する
            m_CharacterController.Move(motion * Time.deltaTime);

            // 移動先を向く
            transform.LookAt(transform.position + motion);

            // Animatorに「Speed」というパラメーター名で移動速度を渡す
            m_Animator.SetFloat("Speed", motion.magnitude);

            // Enterキーが押されて
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // 文字列が入力されていたら
                if (m_ChatInputField.text.Length > 0)
                {
                    // Commandを使って入力された文字列をサーバーへ送信
                    CmdPost(m_ChatInputField.text);

                    // 入力欄を空にする
                    m_ChatInputField.text = "";
                }
            }
        }
        // 文字列をサーバーに送信する。
        [Command]
        void CmdPost(string text)
        {
            RpcPost(text);
        }

        // ClientRpcは各クライアントで実行される。
        // チャット履歴表示欄に文字列を追加する。
        [ClientRpc]
        void RpcPost(string text)
        {
            m_ChatHistory.text += text + System.Environment.NewLine;
        }
    }
}