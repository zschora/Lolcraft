using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    Cinemachine.CinemachineVirtualCamera myVirtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.playerCameraController = this;
        myVirtualCamera = gameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        //Debug.Log("playerCamera: " + (GameManager.Instance.playerCamera != null).ToString());
    }

    public void ChangeFollow(GameObject go)
    {
        myVirtualCamera.Follow = go.transform;
    }

}
