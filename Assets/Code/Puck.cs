using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Puck : MonoBehaviour
{
    private readonly float m_ForceScale = 100f;
    private float m_MaxSqr;

    private Rigidbody2D m_Rigidbody2d;
    private Image m_Image;
    private Transform m_Transform;
    private Vector3 m_RecordPos;
    private Rect m_WallRect;
    private bool m_HitWall = false;


    // Use this for initialization
    void Start()
    {
        m_MaxSqr = Vector2.SqrMagnitude(new Vector2(500, 500));

        m_Transform = this.transform;

        m_Rigidbody2d = this.GetComponent<Rigidbody2D>();
        m_Rigidbody2d.gravityScale = 0;
        m_WallRect = DataMgr.GetIntanstance().GetWallRect();

        RectTransform rectTransform = m_Transform as RectTransform;
        m_WallRect.xMin += rectTransform.sizeDelta.x/2;
        m_WallRect.xMax -= rectTransform.sizeDelta.x / 2;
        m_WallRect.yMin += rectTransform.sizeDelta.y / 2;
        m_WallRect.yMax -= rectTransform.sizeDelta.y / 2;

    }

    // Update is called once per frame
    void Update()
    {
        m_RecordPos = m_Rigidbody2d.position;

        if (m_RecordPos.x < m_WallRect.xMin)
        {
            m_RecordPos.x = m_WallRect.xMin;
            m_HitWall = true;
        }
        if (m_RecordPos.x > m_WallRect.xMax)
        {
            m_RecordPos.x = m_WallRect.xMax;
            m_HitWall = true;

        }
        if (m_RecordPos.y > m_WallRect.yMax)
        {
            m_RecordPos.y = m_WallRect.yMax;
            m_HitWall = true;

        }
        if (m_RecordPos.y < m_WallRect.yMin)
        {
            m_RecordPos.y = m_WallRect.yMin;
            m_HitWall = true;

        }

        if (m_HitWall)
        {
            m_Rigidbody2d.position  =m_RecordPos;
            m_HitWall = false;

        }

    }

    public void AddForce(Vector2 force)
    {
        if(force.sqrMagnitude > m_MaxSqr)
        {
            return;
        }

        Debug.Log(force);
        m_Rigidbody2d.AddForce(force * m_ForceScale);
    }

    public void HittheWall(WallController.Direction ditection)
    {
        Vector3 curPos = m_Rigidbody2d.position;
        Vector3 force = curPos - m_RecordPos;

        if(ditection == WallController.Direction.Up || ditection == WallController.Direction.Down)
        {
            force.y = -force.y;
        }else if(ditection == WallController.Direction.Left || ditection == WallController.Direction.Right)
        {
            force.x = -force.x;
        }

        AddForce(force*30);
    }
}
