using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneEditorUpdateManager
{
    private static List<Action> s_ActiveActions;
    private static List<Action> s_UpdateActions;

    public static void AddActiveAction(Action function)
    {
        if (s_ActiveActions == null)
        {
            s_ActiveActions = new List<Action>();
        }

        s_ActiveActions.Add(function);
    }

    public static void AddUpdateAction(Action function)
    {
        if (s_UpdateActions == null)
        {
            s_UpdateActions = new List<Action>();
        }

        s_UpdateActions.Add(function);
    }

    public static void RemoveUpdateAction(Action function)
    {
        if (s_UpdateActions == null)
        {
            return;
        }

        if (s_UpdateActions.Contains(function))
        {
            s_UpdateActions.Remove(function);
        }
    }

    public static void Update()
    {
        if (s_UpdateActions != null && s_UpdateActions.Count > 0)
        {
            for (var index = 0; index < s_UpdateActions.Count; index++)
            {
                s_UpdateActions[index]();
            }
        }

        Active();
    }

    private static void Active()
    {
        if (s_ActiveActions != null && s_ActiveActions.Count > 0)
        {
            for (var index = 0; index < s_ActiveActions.Count; index++)
            {
                s_ActiveActions[index]();
            }

            s_ActiveActions.Clear();
            s_ActiveActions = null;
        }
    }


    public static void DrawCollider(Vector3 transformPos, Rect colliderRect, Color color)
    {
        if (colliderRect.width <= 0 || colliderRect.height <= 0)
        {
            return;
        }

        Handles.color = color;
        Handles.DrawLines(new Vector3[]
        {
            transformPos + new Vector3(colliderRect.x, colliderRect.y),
            transformPos + new Vector3(colliderRect.x + colliderRect.width, colliderRect.y),

            transformPos + new Vector3(colliderRect.x + colliderRect.width, colliderRect.y),
            transformPos + new Vector3(colliderRect.x + colliderRect.width, colliderRect.y + colliderRect.height),

            transformPos + new Vector3(colliderRect.x + colliderRect.width, colliderRect.y + colliderRect.height),
            transformPos + new Vector3(colliderRect.x, colliderRect.y + colliderRect.height),

            transformPos + new Vector3(colliderRect.x, colliderRect.y + colliderRect.height),
            transformPos + new Vector3(colliderRect.x, colliderRect.y),
        });
    }

}