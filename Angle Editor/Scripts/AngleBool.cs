using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Snorlax.Prototype.AngleEditor
{
    [CreateAssetMenu(menuName ="Angles")]
    public class AngleBool : ScriptableObject
    {
        [HideInInspector]public AngleInformation[] AngleList;
    }

    [Serializable]
    public struct AngleInformation
    {
        public float StartAngle;
        public float EndAngle;
        //public float Distance;
        public bool IsInverted;
        public float Angle;
    }

    public static class AngleFunctions
    {
        /// <summary>
        /// If target is in current angle parameters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isInverted"></param>
        /// <returns></returns>
        public static bool AngleBool(AngleInformation AI, Transform from, Transform to)
        {
            float angle = AngleFromTarget(from, to);

            //x < y ? y - x : Mathf.Abs(360f - x + y);
            float offset = AI.StartAngle > AI.EndAngle ? AI.EndAngle + 360f : 0f;
            return angle >= AI.StartAngle && angle <= AI.EndAngle + offset;

            //return false;
        }

        public static void TestAngleBool(AngleInformation AI, float angle)
        {
            bool Return = false;
            if (AI.StartAngle < AI.EndAngle)
                Return = angle >= AI.StartAngle && angle <= AI.EndAngle;
            else if (AI.StartAngle > AI.EndAngle)
                Return =! (angle <= AI.StartAngle && angle >= AI.EndAngle);

            Debug.Log(Return);
            //return false;
        }

        /// <summary>
        /// Inverts start and end angle dots to give opposite result
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isInverted"></param>
        /// <returns></returns>
        public static bool InvertedAngle(ref float x, ref float y)
        {
            Vector2 inverted = new Vector2(x, y);

            x = inverted.y;
            y = inverted.x;

            return true;
        }

        /// <summary>
        /// Returns Opposite side of an angle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool OppositeAngle(ref float x, ref float y, float offset = 0f)
        {
            x = (x + 180f) % 360f;
            y = (y + 180f) % 360f;

            return true;
        }

        public static float AngleFromTarget(Transform from, Transform to)
        {
            return Quaternion.FromToRotation(from.forward, to.position).eulerAngles.y;
        }
    }
}
