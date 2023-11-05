using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] bool leftHand = false;

    [Header("References")]
    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;

    [Header("Inputs")]
    [SerializeField] InputActionReference _inputActivateLeft;
    [SerializeField] InputActionReference _inputActivateRight;
    [SerializeField] InputActionReference _inputDeactivateLeft;
    [SerializeField] InputActionReference _inputDeactivateRight;

    bool isPressing = false;

    private void Awake()
    {
        SetController();
        _inputActivateLeft.action.performed += InputActivateLeft;
        _inputActivateRight.action.performed += InputActivateRight;
        _inputDeactivateLeft.action.performed += InputDeactivateLeft;
        _inputDeactivateRight.action.performed += InputDeactivateRight;
    }
    private void OnDestroy()
    {
        _inputActivateLeft.action.performed -= InputActivateLeft;
        _inputActivateRight.action.performed -= InputActivateRight;
        _inputDeactivateLeft.action.performed -= InputDeactivateLeft;
        _inputDeactivateRight.action.performed -= InputDeactivateRight;
    }

    void SetController()
    {
        Debug.Log("Setting Controller");
        Transform destination = (leftHand) ? leftController : rightController;

        transform.position = destination.position;
        transform.rotation = destination.rotation;
        transform.parent = destination;
    }

    private void OnTriggerEnter(Collider other)
    {
        LetterChoice choice = other.GetComponent<LetterChoice>();
        if (choice is null)
            return;

        Debug.Log(choice.letter);
        choice.ShowSelectVisual(true);
    }

    private void OnTriggerExit(Collider other)
    {
        LetterChoice choice = other.GetComponent<LetterChoice>();
        if (choice is null)
            return;

        choice.ShowSelectVisual(false);
    }

    void InputActivateLeft(InputAction.CallbackContext ctx)
    {
        if (!leftHand)
        {
            if (isPressing)
                return;

            leftHand = true;
            SetController();
            return;
        }

        isPressing = true;
    }

    void InputActivateRight(InputAction.CallbackContext ctx)
    {
        if (leftHand)
        {
            if (isPressing)
                return;

            leftHand = false;
            SetController();
            return;
        }

        isPressing = true;
    }

    void InputDeactivateLeft(InputAction.CallbackContext ctx)
    {
        if (!leftHand)
            return;

        isPressing = false;
    }

    void InputDeactivateRight(InputAction.CallbackContext ctx)
    {
        if (leftHand)
            return;

        isPressing = false;
    }
}
