using UnityEngine;
using WordPuzzle;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] float _gridOffset = 0.275f;

    [Header("References")]
    [SerializeField] PuzzleBoard _puzzleBoard;
    [SerializeField] GameObject _letterBoxPrefab;
    [SerializeField] LetterSelector _letterSelector;

    private void Awake()
    {
        PuzzleGenerator generator = new PuzzleGenerator();
        Word[] words = generator.Generate(8, out Vector2Int dimensions);

        Vector3 offset = new Vector3(
            transform.position.x + dimensions.x * 0.5f * _gridOffset,
            transform.position.y + dimensions.y * 0.5f * _gridOffset,
            transform.position.z);

        foreach (Word word in words)
        {
            Debug.Log(word);
            foreach (Letter letter in word.letters)
            {
                if (_puzzleBoard.boxes.ContainsKey(letter.pos))
                    continue;

                Vector3 pos = new Vector3(-letter.pos.x * _gridOffset, -letter.pos.y * _gridOffset, 0f) + offset;
                LetterBox box = Instantiate(_letterBoxPrefab, pos, Quaternion.identity, _puzzleBoard.transform).GetComponent<LetterBox>();
                box.SetLetter(letter.val);
                _puzzleBoard.boxes[letter.pos] = box;
            }
        }
        _puzzleBoard.GotoDestination();

        _letterSelector.Setup(words[0]);
    }
}
