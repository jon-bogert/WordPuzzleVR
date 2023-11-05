using UnityEngine;

public class LetterManager : MonoBehaviour
{
    [SerializeField] GameObject[] _letters;

    public GameObject GetLetter(char letter)
    {
        int index = 0;
        if (letter >= 'a')
            index = (int)letter - 97;
        else
            index = (int)letter - 65;

        return _letters[index];
    }
}
