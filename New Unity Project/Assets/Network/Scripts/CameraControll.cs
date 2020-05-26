using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class CameraControll : MonoBehaviour {
 
    private GameObject mainCamera;      //メインカメラ格納用
    private GameObject subCamera;       //サブカメラ格納用 
    private GameObject playerObject;            //回転の中心となるプレイヤー格納用
    public float rotateSpeed = 2.0f;            //回転の速さ
 
 
    //呼び出し時に実行される関数
    void Start () {
        //メインカメラとサブカメラをそれぞれ取得
        mainCamera = GameObject.Find("MainCamera");
        subCamera = GameObject.Find("SubCamera");
        playerObject = GameObject.Find("unitychan");
 
        //サブカメラを非アクティブにする
        subCamera.SetActive(false); 
	}
	
 
	//単位時間ごとに実行される関数
	void Update () {
		//shiftキーが押されている間、サブカメラをアクティブにする
        if(Input.GetKey("left shift")){
            //サブカメラをアクティブに設定
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
        else{
            //メインカメラをアクティブに設定
            subCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
        //rotateCameraの呼び出し
        rotateCamera();
	}

    private void rotateCamera()
    {
        if (Input.GetMouseButton(0)) {
            //Vector3でX,Y方向の回転の度合いを定義
            Vector3 angle = new Vector3(Input.GetAxis("Mouse X") * rotateSpeed,Input.GetAxis("Mouse Y") * rotateSpeed, 0);
 
            //transform.RotateAround()をしようしてメインカメラを回転させる
            mainCamera.transform.RotateAround(playerObject.transform.position, Vector3.up, angle.x);
            mainCamera.transform.RotateAround(playerObject.transform.position, transform.right, angle.y);
        }
    }
}