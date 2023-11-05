using System.Collections.Generic;
using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public Dictionary<Vector2Int, LetterBox> boxes = new Dictionary<Vector2Int, LetterBox>();
    [SerializeField] Transform _destination;

    public void GotoDestination()
    {
        transform.position = _destination.position;
        transform.rotation = _destination.rotation;
        transform.localScale = _destination.localScale;
    }
}
