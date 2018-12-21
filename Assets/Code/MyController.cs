using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class MyController : MonoBehaviour
{
    private Puck m_PullBar;
    private Transform m_Transform;
    private Vector3 m_RecordPos;

    // Use this for initialization
    void Start()
    {
        m_PullBar = DataMgr.GetIntanstance().m_Pack;
        m_Transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        m_RecordPos = m_Transform.localPosition;
    }

    // void OnCollisionEnter2D(Collision2D collider)
    void OnTriggerEnter2D(Collider2D collider)
    {
        Vector3 curPos = m_Transform.localPosition;
        Vector3 force = curPos - m_RecordPos;

        m_PullBar.AddForce(force);
    }
}
