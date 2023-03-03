using UnityEditor;
using UnityEngine;
public class AngleAttribute : PropertyAttribute
{
    public readonly float snap;
    public readonly float min;
    public readonly float max;

    public AngleAttribute()
    {
        snap = 1;
        min = -360;
        max = 360;
    }

    public AngleAttribute(float snap)
    {
        this.snap = snap;
        min = -360;
        max = 360;
    }

    public AngleAttribute(float snap, float min, float max)
    {
        this.snap = snap;
        this.min = min;
        this.max = max;
    }
}

[CustomPropertyDrawer(typeof(AngleAttribute))]
public class AngleDrawer : PropertyDrawer
{
    private static Vector2 mousePosition;
    private static Texture2D KnobBack = Resources.Load("Editor Icons/Dial") as Texture2D;
    private static Texture2D Knob = Resources.Load("Editor Icons/DialButton") as Texture2D;
    private static Texture2D GreenLine = Resources.Load("Editor Icons/Line") as Texture2D;
    private static Texture2D RedLine = Resources.Load("Editor Icons/RedLine") as Texture2D;
    private static Texture2D WhiteLine = Resources.Load("Editor Icons/WhiteLine") as Texture2D;

    public static void DrawBackGround(Rect rect)
    {
        Rect knobRect = new Rect(rect.x, rect.y, rect.height, rect.height);
        GUI.DrawTexture(knobRect, KnobBack);
        //Matrix4x4 matrix = GUI.matrix;
    }

    public static void FloatAngle(Rect rect, ref float originalValueX, ref float originalValueY, float snap, float min, float max, Vector2 zeroVector, float Angle)
    {
        int id = GUIUtility.GetControlID(FocusType.Passive, rect);
        float valueX = originalValueX;
        float valueY = originalValueY;
        Rect knobRect = new Rect(rect.x, rect.y, rect.height, rect.height);

        float delta;
        if (min != max)
            delta = ((max - min) / 360);
        else
            delta = 1;

        if (Event.current != null)
        {
            if (Event.current.type == EventType.MouseDown && knobRect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = id;
                mousePosition = Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == id)
            {
                GUIUtility.hotControl = 0;
            }
            else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == id)
            {
                Vector2 move = mousePosition - Event.current.mousePosition;

                if (Event.current.button == 0)
                {

                    if (knobRect.Contains(mousePosition))
                    {
                        Vector2 mouseStartDirection = (mousePosition - knobRect.center).normalized;
                        float startAngle = CalculateAngle(Vector2.up, mouseStartDirection);

                        Vector2 mouseNewDirection = (Event.current.mousePosition - knobRect.center).normalized;
                        float newAngle = CalculateAngle(Vector2.up, mouseNewDirection);


                        float sign = Mathf.Sign(newAngle - startAngle);
                        float delta2 = Mathf.Min(Mathf.Abs(newAngle - startAngle), Mathf.Abs(newAngle - startAngle + 360f), Mathf.Abs(newAngle - startAngle - 360f));
                        valueX -= delta2 * sign;
                    }

                    if (snap > 0)
                    {
                        float mod = valueX % snap;

                        if (mod < (delta * 3) || Mathf.Abs(mod - snap) < (delta * 3))
                            valueX = Mathf.Round(valueX / snap) * snap;
                    }

                    if (originalValueX != valueX)
                    {
                        mousePosition = Event.current.mousePosition;
                        GUI.changed = true;
                    }
                }

                if (Event.current.button == 1)
                {

                    if (knobRect.Contains(mousePosition))
                    {
                        Vector2 mouseStartDirection = (mousePosition - knobRect.center).normalized;
                        float startAngle = CalculateAngle(Vector2.up, mouseStartDirection);

                        Vector2 mouseNewDirection = (Event.current.mousePosition - knobRect.center).normalized;
                        float newAngle = CalculateAngle(Vector2.up, mouseNewDirection);


                        float sign = Mathf.Sign(newAngle - startAngle);
                        float delta2 = Mathf.Min(Mathf.Abs(newAngle - startAngle), Mathf.Abs(newAngle - startAngle + 360f), Mathf.Abs(newAngle - startAngle - 360f));
                        valueY -= delta2 * sign;
                    }

                    if (snap > 0)
                    {
                        float mod = valueY % snap;

                        if (mod < (delta * 3) || Mathf.Abs(mod - snap) < (delta * 3))
                            valueY = Mathf.Round(valueY / snap) * snap;
                    }

                    if (originalValueY != valueY)
                    {
                        mousePosition = Event.current.mousePosition;
                        GUI.changed = true;
                    }
                }
            }
        }

        float angleOffset = (CalculateAngle(Vector2.up, zeroVector) + 360f) % 360f;

        Matrix4x4 matrix = GUI.matrix;

        Fill(valueX, valueY, angleOffset, min, max, knobRect, matrix);

        if (min != max)
            GUIUtility.RotateAroundPivot((angleOffset + valueX) * (360 / (max - min)), knobRect.center);
        else
            GUIUtility.RotateAroundPivot((angleOffset + valueX), knobRect.center);

        GUI.DrawTexture(knobRect, GreenLine);

        GUI.matrix = matrix;

        if (min != max)
            GUIUtility.RotateAroundPivot((angleOffset + valueY) * (360 / (max - min)), knobRect.center);
        else
            GUIUtility.RotateAroundPivot((angleOffset + valueY), knobRect.center);

        GUI.DrawTexture(knobRect, RedLine);

        GUI.matrix = matrix;

        //Rect label = new Rect(rect.x + rect.height, rect.y + (rect.height / 2) - 9, rect.height, 18);
        //value = EditorGUI.FloatField(label, value);

        if (min != max)
        {
            valueX = Mathf.Clamp(valueX, min, max);
            valueY = Mathf.Clamp(valueY, min, max);
        }


        originalValueX = valueX;
        originalValueY = valueY;

        GUIUtility.RotateAroundPivot((angleOffset + Angle) * (360 / (max - min)), knobRect.center);
        GUI.DrawTexture(knobRect, Knob);
        GUI.matrix = matrix;
    }

    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        Vector3 right = Vector3.right;
        float angle = Vector3.Angle(from, to);
        return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;
    }

    public static void Fill(float x, float y, float angleOffset, float min, float max, Rect knobRect, Matrix4x4 matrix)
    {
        if(x == 0 && y == 0)
            return;

        float angle = x < y ? y - x : Mathf.Abs(360f - x + y);

        for (int i = 0; i < angle; i++)
        {
            GUIUtility.RotateAroundPivot((angleOffset + x + (float)i) * (360 / (max - min)), knobRect.center);
            GUI.DrawTexture(knobRect, WhiteLine);
            GUI.matrix = matrix;
        }
    }
}
