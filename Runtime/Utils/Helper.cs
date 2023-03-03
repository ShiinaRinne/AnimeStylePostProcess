using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnimeStylePostProcess.Utils
{
    public static class Helper
    {
        public static float GetLightScreenSpaceAngle(this Light light)
        {
            Light mainLight = light == null ? RenderSettings.sun : light;
            Vector3 lightDirection = -mainLight.transform.forward;

            /* =================================================================
            // 使用屏幕空间的方式，但会随相机位置变化
            Vector3 lightDirectionInScreenSpace = Camera.main.WorldToScreenPoint(lightDirection);
            float angle = Mathf.Atan2(lightDirectionInScreenSpace.y - Screen.height / 2f, lightDirectionInScreenSpace.x - Screen.width / 2f) * Mathf.Rad2Deg;
            ================================================================= */
            // 将方向投影至屏幕所在平面计算
            Vector3 screenNormal = Camera.main.transform.forward;
            Vector3 projectedDirection = Vector3.ProjectOnPlane(lightDirection, screenNormal);
            // =================================================================

            // 当摄像机旋转 180 度(背身)时，光照方向会错误
            // 根据右向量与上方向的点乘判断是否需要取反
            Vector3 screenRight = Camera.main.transform.right;

            /* =================================================================
            // 当摄像机 rotation.y 与 rotation.z 更改时，可能会错误取反
            if (Vector3.Dot(screenRight, Vector3.up) < 0f)
            {
                screenRight = -screenRight;
            }
            ================================================================= */
            // 通过判断 screenRight 和 screenNormal 在摄像机坐标系下的方向关系来避免这个问题
            // 计算 screenRight 和 screenNormal 在摄像机坐标系下的 z，
            // 如果 screenRight 在摄像机坐标系下的 z 比 screenNormal 大，则将 screenRight 取反
            float screenRightZ = Camera.main.transform.InverseTransformDirection(screenRight).z;
            float screenNormalZ = Camera.main.transform.InverseTransformDirection(screenNormal).z;
            if (screenRightZ > screenNormalZ) 
                screenRight = -screenRight;
            // =================================================================

            // 根据 screenRight 和 Vector3.right 的夹角，创建一个旋转矩阵
            // 将其应用于投影向量，消除摄像机翻转的影响
            Quaternion rotation = Quaternion.AngleAxis(Vector3.SignedAngle(screenRight, Vector3.right, Vector3.up), Vector3.up);
            projectedDirection = rotation * projectedDirection;

            float angle = Mathf.Atan2(projectedDirection.y, projectedDirection.x) * Mathf.Rad2Deg;

            return angle > 0f ? 90f - angle : -270f - angle;
        }
    }
}