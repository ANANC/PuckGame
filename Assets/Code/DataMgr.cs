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

    public PullBar m_PullBar;

    public Rect GetWallRect()
    {
        Rect wallRect = new Rect();

        Transform myTransform = this.transform;
        WallController[] walls = myTransform.parent.GetComponentsInChildren<WallController>();
        if (walls == null || walls.Length <4)
        {
            return wallRect;
        }


        for (int index =0;index<walls.Length;index++)
        {
            Vector3 pos = walls[index].transform.position;
            if(index == 0)
            {
                wallRect.x = pos.x;
                wallRect.y = pos.y;
                wallRect.width = 0;
                wallRect.height = 0;
            }
            else
            {
                if(pos.x < wallRect.x)
                {
                    wallRect.x = pos.x;
                }else
                {
                    wallRect.width = pos.x - wallRect.x;
                }

                if (pos.y > wallRect.y)
                {
                    wallRect.y = pos.y;
                }
                else
                {
                    wallRect.height = pos.y - wallRect.y;
                }
            }
        }

        return wallRect;
    }
}
