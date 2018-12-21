using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WallController : MonoBehaviour {
    public enum Direction
    {
        Up,Down,Left,Right
    }
    private Puck m_PullBar;
    public Direction m_Direction;

    // Use this for initialization
    void Start () {
        m_PullBar = DataMgr.GetIntanstance().m_Pack;
    }

    public float Width()
    {
        return this.GetComponent<BoxCollider2D>().size.x;
    }

    public float Height()
    {
        return this.GetComponent<BoxCollider2D>().size.y;
    }

    //   void OnCollisionEnter2D(Collision2D coll)
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name != m_PullBar.name)
        {
            return;
        }
        m_PullBar.HittheWall(m_Direction);
    }

}
