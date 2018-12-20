using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WallController : MonoBehaviour {
    public enum Direction
    {
        Up,Down,Left,Right
    }
    private PullBar m_PullBar;
    public Direction m_Direction;

    // Use this for initialization
    void Start () {
        m_PullBar = DataMgr.GetIntanstance().m_PullBar;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name != m_PullBar.name)
        {
            return;
        }
        m_PullBar.HittheWall(m_Direction);
    }
}
