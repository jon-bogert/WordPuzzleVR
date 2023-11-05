using UnityEngine;

public class LetterBox : MonoBehaviour
{
    [SerializeField] char letter = '\0';
    [SerializeField] bool showLetter = false;

    GameObject letterObject = null;

    private void Start()
    {
        if (letter == 0)
            return;
    }

    public void SetLetter(char letter)
    {
        this.letter = letter;
        GameObject letterPrefab = FindObjectOfType<LetterManager>().GetLetter(letter);
        letterObject = Instantiate(letterPrefab, transform);
        if (!showLetter)
            letterObject.SetActive(false);
    }
}
