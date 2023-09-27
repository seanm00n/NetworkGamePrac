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

    public Transform firePos;
    public GameObject bullet;
    private bool isDie = false;
    private int hp = 100;
    private float respawnTime = 3.0f;

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
        if (pv.isMine && isDie == false)
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
                Fire();
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

    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log("HIT!:" + coll.gameObject.tag);
        if(coll.gameObject.tag == "BULLET")
        {
            Debug.Log("Hit");
            Destroy(coll.gameObject);
            hp -= 10;
            animator.SetTrigger("Hit");
            if (hp <= 0) {
                animator.SetTrigger("Die");
                StartCoroutine(RespawnPlayer(respawnTime));
            }
        }
    }

    IEnumerator RespawnPlayer(float waitTime)
    {
        Debug.Log("Died!");
        isDie = true;
        StartCoroutine(PlayerVisible(true, 0.5f));
        yield return new WaitForSeconds(waitTime);

        tr.position = new Vector3(Random.Range(-20.0f, 20.0f), 0.0f, Random.Range(-20.0f, 20.0f));
        hp = 100;
        isDie = false;
        animator.SetTrigger("Reset");
        StartCoroutine(PlayerVisible(true, 1.0f));
    }

    IEnumerator PlayerVisible(bool visibled, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GetComponentInChildren<MeshRenderer>().enabled = visibled;
    }

    IEnumerator CreateBullet()
    {
        GameObject bulletObject = Instantiate(bullet, firePos.position, firePos.rotation);
        bulletObject.GetComponent<Bullet>().owner = name;
        yield return null;
    }

    private void Fire()
    {
        StartCoroutine(CreateBullet());
        pv.RPC("FireRPC", PhotonTargets.Others);
    }

    [PunRPC]
    private void FireRPC()
    {
        StartCoroutine(CreateBullet());
    }
}
