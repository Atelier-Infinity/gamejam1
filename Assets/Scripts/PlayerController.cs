using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerInput.IPlayerActions
{
    private PlayerInput _controls;
    private Vector2 _direction;
    
    public float speed = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(_direction.x, 0, _direction.y) * (Time.deltaTime * speed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();
        // Debug.Log(_direction);
    }
    
    /////////////////////////////////////////
    public void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new PlayerInput();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }

    public void OnDisable()
    {
        _controls.Player.Disable();
    }
}
