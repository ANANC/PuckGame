using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMgr : MonoBehaviour {

    private static DataMgr m_Instance;

	public static DataMgr GetIntanstance()
    {
        return m_Instance;
    }

    public void Awake()
    {
        m_Instance = this;
    }

    public Puck m_Pack;
    public Transform m_WallPrarent;
    public Transform m_MyDoor;
    public Transform m_AIDoor;
    public Transform m_CenterLine;

    public Rect GetWallRect()
    {
        Rect wallRect = new Rect();

        Transform myTransform = this.transform;
        WallController[] walls = m_WallPrarent.GetComponentsInChildren<WallController>();
        if (walls == null || walls.Length < 4)
        {
            return wallRect;
        }


        for (int index = 0; index < walls.Length; index++)
        {
            Vector3 pos = walls[index].transform.position;
            if(walls[index].m_Direction == WallController.Direction.Left)
            {
                wallRect.xMin = pos.x + walls[index].Width()/2;
            }
            if (walls[index].m_Direction == WallController.Direction.Right)
            {
                wallRect.xMax =  pos.x - walls[index].Width() / 2; ;
            }
            if (walls[index].m_Direction == WallController.Direction.Up)
            {
                wallRect.yMax = pos.y - walls[index].Height() / 2; ;
            }
            if (walls[index].m_Direction == WallController.Direction.Down)
            {
                wallRect.yMin = pos.y + walls[index].Height() / 2; ;
            }
        }

        return wallRect;
    }

    public bool PlaySide()
    {
        if (m_Pack.transform.position.y < m_CenterLine.transform.position.y)
            return true;
        return false;
    }
}
