using UnityEngine;
using WordPuzzle;

public class LetterSelector : MonoBehaviour
{
    [SerializeField] float _radius = 1f;
    [SerializeField] GameObject _letterChoicePrefab;
    [SerializeField] GameObject _letterChoiceParent;
    int _length = 0;

    public int length { get { return _length; } }

    public void Setup(Word word)
    {
        //Load
        _length = word.letters.Length;
        char[] letters = new char[_length];
        for (int i = 0; i < _length; ++i)
            letters[i] = word.letters[i].val;

        //Shuffle
        for (int i = _length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            char temp = letters[i];
            letters[i] = letters[j];
            letters[j] = temp;
        }

        //Distribute
        float angle = 360f / _length;
        for (int i = 0; i < _length; i++)
        {
            float a = angle * i * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Sin(a), Mathf.Cos(a)) * _radius;
            LetterChoice choice = Instantiate(_letterChoicePrefab, pos, Quaternion.identity, _letterChoiceParent.transform).GetComponent<LetterChoice>();
            choice.Setup(this, letters[i]);
        }

        _letterChoiceParent.transform.position = transform.position;
        _letterChoiceParent.transform.rotation = transform.rotation;
        _letterChoiceParent.transform.localScale = transform.localScale;
    }
}
