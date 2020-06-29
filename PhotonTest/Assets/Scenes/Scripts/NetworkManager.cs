using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : Photon.PunBehaviour
{

	// Use this for initialization
	void Start()
	{
		//　ログをすべて表示する
		PhotonNetwork.logLevel = PhotonLogLevel.Full;

		//　ロビーに自動で入る
		PhotonNetwork.autoJoinLobby = true;

		//　ゲームのバージョン設定
		PhotonNetwork.ConnectUsingSettings("test");
	}

	//　ログインまでの情報を表示するテキスト
	[SerializeField]
	private Text informationText;

	// Update is called once per frame

	void Update()
	{
		//　サーバ接続状態を表示
		informationText.text = PhotonNetwork.connectionStateDetailed.ToString();

	}

	//　ログイン画面
	[SerializeField]
	private GameObject loginUI;

	//　ロビーに入った時に呼ばれる
	public override void OnJoinedLobby()
	{
		Debug.Log("ロビーに入る");
		loginUI.SetActive(true);
	}

	//　部屋リストを表示するドロップダウン
	[SerializeField]
	private Dropdown roomLists;
	//　部屋の名前
	[SerializeField]
	private InputField roomName;

	//　ログインボタンを押した時に実行するメソッド
	public void LoginGame()
	{
		//　ルームオプションを設定
		RoomOptions ro = new RoomOptions()
		{
			//　ルームを見えるようにする
			IsVisible = true,
			//　部屋の入室最大人数
			MaxPlayers = 10
		};

		if (roomName.text != "")
		{
			//　部屋がない場合は作って入室
			PhotonNetwork.JoinOrCreateRoom(roomName.text, ro, TypedLobby.Default);
		}
		else
		{
			//　部屋が存在すれば
			if (roomLists.options.Count != 0)
			{
				Debug.Log(roomLists.options[roomLists.value].text);
				PhotonNetwork.JoinRoom(roomLists.options[roomLists.value].text);
				//　部屋が存在しなければDefaultRoomという名前で部屋を作成
			}
			else
			{
				PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
			}
		}
	}
	//　部屋が更新された時の処理
	public override void OnReceivedRoomListUpdate()
	{
		Debug.Log("部屋更新");
		//　部屋情報を取得する
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		//　ドロップダウンリストに追加する文字列用のリストを作成
		List<string> list = new List<string>();

		//　部屋情報を部屋リストに表示
		foreach (RoomInfo room in rooms)
		{
			//　部屋が満員でなければ追加
			if (room.PlayerCount < room.MaxPlayers)
			{
				list.Add(room.Name);
			}
		}

		//　ドロップダウンリストをリセット
		roomLists.ClearOptions();

		//　部屋が１つでもあればドロップダウンリストに追加
		if (list.Count != 0)
		{
			roomLists.AddOptions(list);
		}
	}

	//　ログアウトボタン
	[SerializeField]
	private GameObject logoutButton;
	//　プレイヤーの名前入力欄
	[SerializeField]
	private InputField playerName;

	//　プレイヤーのインスタンス
	private GameObject player;

	//　部屋に入室した時に呼ばれるメソッド
	public override void OnJoinedRoom()
	{
		loginUI.SetActive(false);
		logoutButton.SetActive(true);
		Debug.Log("入室");

		//　InputFieldに入力した名前を設定
		PhotonNetwork.player.NickName = playerName.text;
		//　プレイヤーキャラクターを登場させる
		StartCoroutine("SetPlayer", 0f);
	}

	//　プレイヤーをゲームの世界に出現させる
	IEnumerator SetPlayer(float time)
	{
		yield return new WaitForSeconds(time);
		//　ネットワークごしにEthanをインスタンス化する
		player = PhotonNetwork.Instantiate("unitychan", Vector3.up, Quaternion.identity, 0);
		player.GetPhotonView().RPC("SetName", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
	}

	//　部屋の入室に失敗した
	void OnPhotonJoinRoomFailed()
	{
		Debug.Log("入室に失敗");

		//　ルームオプションを設定
		RoomOptions ro = new RoomOptions()
		{
			//　ルームを見えるようにする
			IsVisible = false,
			//　部屋の入室最大人数
			MaxPlayers = 10
		};
		//　入室に失敗したらDefaultRoomを作成し入室
		PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
	}

	//　ログアウトボタンを押した時の処理
	public void LogoutGame()
	{
		PhotonNetwork.LeaveRoom();
	}

	//　部屋を退室した時の処理
	public override void OnLeftRoom()
	{
		Debug.Log("退室");
		logoutButton.SetActive(false);
	}

}