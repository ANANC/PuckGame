using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class MyController : MonoBehaviour
{
    private PullBar m_PullBar;
    private Vector3 m_RecordPos;

    // Use this for initialization
    void Start()
    {
        m_PullBar = DataMgr.GetIntanstance().m_PullBar;
    }

    // Update is called once per frame
    void Update()
    {
        m_RecordPos = this.transform.localPosition;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Vector3 curPos = this.transform.localPosition;
        Vector3 force = curPos - m_RecordPos;

        m_PullBar.AddForce(force );
    }
}
