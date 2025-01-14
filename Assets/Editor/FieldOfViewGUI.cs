using AI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;


namespace Editor
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewGUI : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;
            Handles.color = Color.white;
            //Draw Wire Arc to show field of vision radius
            Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
            
            //Draw Vision Cone
            Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.viewAngle / 2);
            Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.viewAngle / 2);

            Handles.color = Color.yellow;
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.viewRadius);
            
            //Draw Line for Raycast simulation
            if (fov.isPlayerVisible)
            {
                Handles.color = Color.green;
                Handles.DrawLine(fov.transform.position, fov.visionTarget.transform.position);
            }
        }

        private static Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(angleInRadians), 0f, Mathf.Cos(angleInRadians));
        }
    }
}
