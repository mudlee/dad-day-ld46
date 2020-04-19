using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class BabyController : MonoBehaviour
{
    public static string TAG_NAME = "Baby";
    private GameController _gameController;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;

    // MOVEMENT
    private static int SPEED = 1;
    private enum Direction { LEFT, RIGHT, UP, DOWN, NO_DIRECTION };
    private enum State { WANDERING, APPROACHING_NOT_SAFE, REACHED_NOT_SAFE, FATHER_GRABBED };
    private Direction _currentDirection;
    private int _remainingMovementTime = 0;
    private Direction? _collidedDirection;
    private Vector2 _velocity = new Vector2();
    private Vector2? _notSafeLocation;
    private State _state = State.WANDERING;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        if(!GameController.Running)
        {
            return;
        }

        if (_state == State.APPROACHING_NOT_SAFE)
        {
            transform.position = Vector2.MoveTowards(transform.position, _notSafeLocation.Value, SPEED * Time.fixedDeltaTime);
        }
        else if(_state == State.WANDERING)
        {
            if (_remainingMovementTime == 0)
            {
                UpdateDirection((Direction)Random.Range(0, System.Enum.GetValues(typeof(Direction)).Length));
            }
            else
            {
                _remainingMovementTime--;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(NotSafeStuff.TAG_NAME))
        {
            if (_state == State.APPROACHING_NOT_SAFE)
            {
                _state = State.REACHED_NOT_SAFE;
                _notSafeLocation = null;
                var notSafeStuff = collision.gameObject.GetComponent<NotSafeStuff>();
                var damageType = notSafeStuff.damageType;
                _gameController.NotifyBabyDied(transform.position, damageType, notSafeStuff.name);
                Debug.Log(string.Format("Not safe stuff approached {0} - {1}", collision.gameObject.name, damageType));
                TakeDamage(damageType);
                notSafeStuff.DoDamage();
                _collider.enabled = false;
                UpdateDirection(Direction.NO_DIRECTION);
            }
        }
        else
        {
            Debug.Log(string.Format("Collision with {0}",collision.gameObject.name));
            _collidedDirection = _currentDirection;
            UpdateDirection(Direction.NO_DIRECTION);
        }
    }

    void TakeDamage(NotSafeStuff.DamageType damageType)
    {
        switch(damageType)
        {
            case NotSafeStuff.DamageType.Drowning:
                _animator.SetTrigger("GotToxic");
                break;
            case NotSafeStuff.DamageType.Burn:
                _animator.SetTrigger("GotHeat");
                break;
            case NotSafeStuff.DamageType.ElectricShock:
                _animator.SetTrigger("GotElectric");
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(NotSafeStuff.TAG_NAME))
        {
            Debug.Log(string.Format("Not safe stuff noticed {0}", collision.name));
            _state = State.APPROACHING_NOT_SAFE;
            _notSafeLocation = collision.transform.position;
            //UpdateDirection(Direction.NO_DIRECTION);
            _velocity.Set(0, 0);
        }
    }

    void UpdateDirection(Direction direction)
    {
        if (direction != _collidedDirection)
        {
            Debug.Log(string.Format("Updating direction to {0}",direction));
            _remainingMovementTime = direction == Direction.NO_DIRECTION ? (int)Random.Range(50f, 100f) : (int)Random.Range(200f, 400f);
            _currentDirection = direction;

            _animator.SetBool("Walking", _currentDirection != Direction.NO_DIRECTION);
            UpdateVelocity();
        }
    }
    private void UpdateVelocity()
    {
        switch (_currentDirection)
        {
            case Direction.DOWN:
                _velocity.Set(0, -SPEED);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                _spriteRenderer.flipY = true;
                break;
            case Direction.UP:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                _spriteRenderer.flipY = false;
                _velocity.Set(0, SPEED);
                break;
            case Direction.LEFT:
                _velocity.Set(-SPEED, 0);
                transform.rotation = Quaternion.Euler(0, 0, 90);
                _spriteRenderer.flipY = false;
                break;
            case Direction.RIGHT:
                _velocity.Set(SPEED, 0);
                transform.rotation = Quaternion.Euler(0,0,-90);
                _spriteRenderer.flipY = false;
                break;
            case Direction.NO_DIRECTION:
                _velocity.Set(0, 0);
                break;
        }

        _rigidbody.velocity = _velocity;
    }
}
