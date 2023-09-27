using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0f;
    private float v = 0f;
    private Transform tr;
    private PhotonView pv;
    public float speed = 10f;
    public float rotSpeed = 10f;
    private Animator animator;
    public TextMesh playerName;
    new string name = "";//
    private Vector3 currPos;
    private Quaternion currRot;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();

        pv.ObservedComponents[0] = this;

        if (pv.isMine)
            Camera.main.GetComponent<FollowCam>().targetTr = tr.Find("Cube").gameObject.transform;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.isMine)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
            tr.Translate(moveDir.normalized * Time.deltaTime * speed);
            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

            if (moveDir.magnitude > 0)
            {
                animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
            }

            if (Input.GetButtonDown("Fire1")){
                animator.SetTrigger("Attack");
            }

        }
        else
        {
            if (tr.position != currPos)
            {
                animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                animator.SetFloat("Speed", 0.0f);
            }
        }
        tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime*10.0f);
        tr.rotation = Quaternion.Lerp(tr.rotation, currRot, Time.deltaTime *10.0f);//
        
    }
    
    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
            stream.SendNext(name);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            SetPlayerName((string)stream.ReceiveNext());
        }
    }
    public void SetPlayerName(string name)
    {
        this.name = name;
        GetComponent<PlayerCtrl>().playerName.text = this.name;
    }
}
