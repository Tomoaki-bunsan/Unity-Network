using System.Collections.Generic;
using UnityEngine;
using System.Collections;
[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour
{
    #region �ϐ��錾
    //�͈̓`���b�g�����̂��߂̃I�u�W�F�N�g�A�ϐ���`
    GameObject[] players;   //�S�Ẵv���C���[�L�����擾�p
    GameObject sender;      //���M�L�����擾�p
    GameObject myPlayer;    //�����̃L�����擾�p
    GUIStyle ChatStyle = new GUIStyle();    //�͈̓`���b�gStyle
    GUIStyleState ChatStyleState = new GUIStyleState();
    GUIStyle AllChatStyle = new GUIStyle(); //�S�̃`���b�gStyle
    GUIStyleState AllChatStyleState = new GUIStyleState();
    public Rect GuiRect = new Rect(0, 0, 300, 200); //�`���b�gUI�̑傫���ݒ�p
    public bool IsVisible = true;   //�`���b�gUI�\����\���t���O
    public bool AlignBottom = true;
    public List<string> messages = new List<string>();  //�`���b�g���O�i�[�pList
    public List<bool> chatKind = new List<bool>(); //�`���b�g���O�̎�ފi�[�p(�͈̓`��or�S�`��)
    public string inputLine = "";//���͕��͊i�[�pString
    private Vector2 scrollPos = Vector2.zero;   //�X�N���[���o�[�ʒu
    #endregion
    #region Start�֐� Updata�֐�
    public void Start()
    {
        //myPlayer�I�u�W�F�N�g�擾(�͈̓`���b�g��������position��myPM�g��)
        GetmyPlayer();
        //�͈̓`���b�g�̏ꍇ�͔������ɂ��A�������t�h���炠�ӂꂽ�ꍇ�͐܂�Ԃ��ݒ�
        ChatStyleState.textColor = Color.white;
        ChatStyle.normal = ChatStyleState;
        ChatStyle.wordWrap = true;
        //�S�̃`���b�g�̏ꍇ�͐ԕ����ɂ��A�������t�h���炠�ӂꂽ�ꍇ�͐܂�Ԃ��ݒ�
        AllChatStyleState.textColor = Color.red;
        AllChatStyle.normal = AllChatStyleState;
        AllChatStyle.wordWrap = true;
    }
    public void Update()
    {
        //ChatUI�̈ʒu�𒲐�
        this.GuiRect.y = Screen.height - this.GuiRect.height;
        //ChatUI�̑傫������
        GuiRect.width = Screen.width / 3;
        GuiRect.height = Screen.height / 3;
    }
    #endregion
    #region OnGUI�֐�
    public void OnGUI()
    {
        if (!this.IsVisible || !PhotonNetwork.inRoom)   //�\���t���O��OFF�܂���photon�ɂȂ����Ă��Ȃ��Ƃ�
        {
            //UI��\��
            return;
        }
        //ChatUI�̍쐬�J�n
        //�`���b�gUI�����@Begin&EndArea�Ń`���b�gUI�̈ʒu�Ƒ傫����ݒ� 
        GUILayout.Window(0, GuiRect, ChatUIWindow, "");   //�`���b�gUI�E�C���h�E���쐬
                                                          //Enter��������
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            //�`���b�g���͑҂���Ԃɂ���
            GUI.FocusControl("ChatInput");
        }
    }
    #endregion
    #region �`���b�gUI����
    void ChatUIWindow(int windowID)
    {
        //Focus���`���b�gUI�ɏ���Ă�Ƃ���Enter�������ƃ`���b�g���������s�����
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine))  //�`���b�g���͗���Null��Empty�łȂ��ꍇ
            {
                //�͈̓`���b�g���M�֐����s
                SendChat(false);
                return;
            }
        }
        //�����̃R���g���[���O���[�v�J�n
        GUILayout.BeginVertical();
        //�X�N���[���r���[�J�n�ʒu
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        //�`���b�g���O�\���p�t���L�V�u���X�y�[�X����
        GUILayout.FlexibleSpace();
        //�t���L�V�u���X�y�[�X�Ƀ`���b�g���O��\��
        for (int i = 0; i <= messages.Count - 1; i++)
        {
            if (chatKind[i] != true)    //�͈̓`���b�g�ł����
            {
                GUILayout.Label(messages[i], ChatStyle);
            }
            else                        //�S�`���b�g�ł����
            {
                GUILayout.Label(messages[i], AllChatStyle);
            }
        }
        //�X�N���[���r���[�I��
        GUILayout.EndScrollView();
        //�����̃R���g���[���O���[�v�J�n
        GUILayout.BeginHorizontal();
        //���̓e�L�X�g�t�B�[���h�����AFocus���������Ԃ�ChatInput�Ɩ���
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine, 200);
        //�uSend�v�{�^���𐶐����������Ƃ��ɂ͔͈̓`���b�g���M
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
        {
            //�͈̓`���b�g���M�֐����s
            SendChat(false);
        }
        //All�{�^���𐶐����������Ƃ��ɂ͑S�̃`���b�g���M
        if (GUILayout.Button("All", GUILayout.ExpandWidth(false)))
        {
            //�S�̃`���b�g���M�֐����s
            SendChat(true);
        }
        //�����̃R���g���[���O���[�v�I��
        GUILayout.EndHorizontal();
        //�����̃R���g���[���O���[�v�I��
        GUILayout.EndVertical();
    }
    #endregion
    #region GetmyPlayer ���L�����̃I�u�W�F�N�g��myPlayer�ɓo�^
    void GetmyPlayer()
    {
        //���L������ID�擾
        int myPlayerID = PhotonNetwork.player.ID;
        //�S�Ẵv���C���[�I�u�W�F�N�g���擾
        players = GameObject.FindGameObjectsWithTag("Player");
        //�S�Ẵv���C���[�I�u�W�F�N�g���玩�L������ID�Ō������A���o��
        foreach (GameObject player in players)
        {
            int playerLoopId = player.GetComponent<PhotonView>().owner.ID;
            if (playerLoopId == myPlayerID)
            {
                //���v���C���[�I�u�W�F�N�g���擾
                myPlayer = player;
            }
        }
        return;
    }
    #endregion
    #region �`���b�g���M�֐�
    void SendChat(bool isAll)
    {
        //chatRPC
        this.photonView.RPC("Chat", PhotonTargets.All, myPlayer.transform.position, this.inputLine, isAll);
        //���M��A���͗�����ɂ��A�X�N���[���ŉ��ʒu�Ɉړ�
        this.inputLine = "";
        scrollPos.y = Mathf.Infinity;
    }
    #endregion
    #region ChatRPC RPC�ďo���F���M�ҁ@RPC��M���F��M��
    [PunRPC]
    public void Chat(Vector3 senderposition, string newLine, bool isAll, PhotonMessageInfo mi)
    {
        if (messages.Count >= 100)          //�`���b�g���O�������Ȃ��ė����烍�O���폜���Ă����M
        {
            messages.Clear();               //�S�Ẵ`���b�g���O���폜
            chatKind.Clear();               //�S�Ẵ`���b�g�̎�ޏ��폜
        }
        if (!isAll) //�͈̓`���b�g�Ƃ��Ď�M
        {
            //myPlayer��sender�̋��������M���邩���f
            if (Vector3.Distance(myPlayer.transform.position, senderposition) < 10)
            {
                //chat��M
                ReceiveChat(newLine, isAll, mi);
            }
        }
        else if (isAll) //�S�`���Ƃ��Ď�M
        {
            //chat��M
            ReceiveChat(newLine, isAll, mi);
        }
        //��M�����Ƃ��̓X�N���[���ŉ��ʒu
        scrollPos.y = Mathf.Infinity;
    }
    #endregion
    #region �`���b�g��M�֐�
    void ReceiveChat(string _newLine, bool isAll, PhotonMessageInfo _mi)
    {
        //���M�҂̖��O�p�ϐ�
        string senderName = "anonymous";
        if (_mi.sender != null)
        {
            //���M�҂̖��O�������
            if (!string.IsNullOrEmpty(_mi.sender.NickName))
            {
                senderName = _mi.sender.NickName;
            }
            else
            {
                senderName = "player " + _mi.sender.ID;
            }
        }
        //��M�����`���b�g�����O�ɒǉ�
        this.messages.Add(senderName + ": " + _newLine);
        this.chatKind.Add(isAll);
        return;
    }
    #endregion

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }
}