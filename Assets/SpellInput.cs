using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInput : MonoBehaviour
{

    bool spellCasting;
    List<Vector2> mousePositions;

    public LineRenderer lineRenderer;

    public float maxDeviationAngle;
    public float minLength;
    // Start is called before the first frame update
    void Start()
    {
        mousePositions = new List<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            spellCasting = true;
        }
        if (spellCasting)
        {
            mousePositions.Add(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            spellCasting = false;
            processMousePositions();
        }
    }


    void processMousePositions()
    {

        List<Vector2> smoothMousePositions = new List<Vector2>();

        for (int i = 1; i < mousePositions.Count; i++)
        {
            smoothMousePositions.Add((mousePositions[i - 1] + mousePositions[i]) / 2);
        }
        mousePositions.Clear();

        List<(Vector2, Vector2)> lines = new List<(Vector2, Vector2)>();

        int firstPos = 0;

        for (int i = 1; i < smoothMousePositions.Count - 1; i++)
        {
            Vector2 seg1 = smoothMousePositions[i - 1] - smoothMousePositions[i];
            Vector2 seg2 = smoothMousePositions[i] - smoothMousePositions[i + 1];
            if (Vector2.Angle(seg1, seg2) > maxDeviationAngle)
            {
                lines.Add((smoothMousePositions[firstPos], smoothMousePositions[i]));
                firstPos = i;
            }
        }
        lines.Add((smoothMousePositions[firstPos], smoothMousePositions[smoothMousePositions.Count - 1]));


        List<(Vector2, Vector2)> filtered = new List<(Vector2, Vector2)>();

        foreach (var line in lines)
        {
            if ((line.Item1 - line.Item2).magnitude > minLength)
            {
                filtered.Add(line);
            }
        }

        Vector3[] positions = new Vector3[filtered.Count + 1];
        foreach (var line in filtered)
        {
            print(line.Item1.ToString() + ", " + line.Item2.ToString());

        }

        for (int i = 0; i < filtered.Count; i++)
        {
            positions[i] = Camera.main.ScreenToWorldPoint((Vector3)filtered[i].Item1 + Vector3.forward);
        }
        positions[filtered.Count] = Camera.main.ScreenToWorldPoint((Vector3)filtered[filtered.Count - 1].Item2 + Vector3.forward);
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
