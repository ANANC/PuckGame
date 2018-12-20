using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PullBar : MonoBehaviour
{
    public Rigidbody2D m_Rigidbody2d;
    private Vector3 m_RecordPos;
    private readonly float m_ForceScale = 100f;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody2d = this.GetComponent<Rigidbody2D>();
        m_Rigidbody2d.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_RecordPos = this.transform.localPosition;
    }

    public void AddForce(Vector2 force)
    {
        Debug.Log(force);
        m_Rigidbody2d.AddForce(force * m_ForceScale);
    }

    public void HittheWall(WallController.Direction ditection)
    {
        Vector3 curPos = this.transform.localPosition;
        Vector3 force = curPos - m_RecordPos;

        if(ditection == WallController.Direction.Up || ditection == WallController.Direction.Down)
        {
            force.y = -force.y;
        }else if(ditection == WallController.Direction.Left || ditection == WallController.Direction.Right)
        {
            force.x = -force.x;
        }

        AddForce(force);
    }
}
