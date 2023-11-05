using UnityEngine;

public class LetterChoice : MonoBehaviour
{
    [SerializeField] GameObject _selectVisual;

    LetterSelector _letterSelector;
    bool _selected = false;
    char _letter;

    public bool selected { get { return selected; } }
    public char letter { get { return _letter; } }

    public void Setup(LetterSelector selector, char letter)
    {
        _letterSelector = selector;
        _letter = letter;
        GameObject letterPrefab = FindObjectOfType<LetterManager>().GetLetter(letter);
        Instantiate(letterPrefab, transform);

        _selectVisual.SetActive(false);
    }

    public void ResetSelected()
    {
        _selected = false;
    }

    public void ShowSelectVisual(bool setTo)
    {
        _selectVisual.SetActive(setTo);
    }
}
