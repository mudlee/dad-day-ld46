using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Fathercontroller : MonoBehaviour
{
    public static string TAG_NAME = "Player";
    public GameObject babyPrefab;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _movement;
    private float speed = 3f;
    private Dictionary<string, GameObject> _visibleBabies = new Dictionary<string, GameObject>();
    private int _grabbedBabies = 0;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!GameController.Running)
        {
            _movement.Set(0, 0);
            _animator.SetBool("IdleWithBaby", false);
            _animator.SetBool("Walking", false);
            _animator.SetBool("WalkingWithBaby", false);
            return;
        }

        _movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        UpdateAnim();

        if (Input.GetKeyDown(KeyCode.Space) && _grabbedBabies > 0)
        {
            var baby = Instantiate(babyPrefab);
            baby.transform.position = transform.position;
            _grabbedBabies--;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _visibleBabies.Count > 0)
        {
            var notYetGrabbed = new List<string>();
            foreach(var baby in _visibleBabies)
            {
                Debug.Log(string.Format("Grabbing {0}", baby.Key));
                notYetGrabbed.Add(baby.Key);
            }

            foreach(var baby in notYetGrabbed)
            {
                var gameObj = _visibleBabies[baby];
                _visibleBabies.Remove(baby);
                Destroy(gameObj);
                _grabbedBabies++;
            }
        }
    }

    void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _movement * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BabyController.TAG_NAME))
        {
            Debug.Log(string.Format("I see my child: {0}", collision.name));
            _visibleBabies.Add(collision.name, collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(BabyController.TAG_NAME))
        {
            Debug.Log(string.Format("Lost child: {0}", collision.name));
            _visibleBabies.Remove(collision.name);
        }
    }

    void UpdateAnim()
    {
        if(_movement.x > 0)
        {
            _animator.SetBool("IdleWithBaby", false);
            _animator.SetBool("Walking", _grabbedBabies == 0);
            _animator.SetBool("WalkingWithBaby", _grabbedBabies > 0);
            _spriteRenderer.flipY = false;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if(_movement.x < 0)
        {
            _animator.SetBool("IdleWithBaby", false);
            _animator.SetBool("Walking", _grabbedBabies == 0);
            _animator.SetBool("WalkingWithBaby", _grabbedBabies > 0);
            _spriteRenderer.flipY = false;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if(_movement.y > 0)
        {
            _animator.SetBool("IdleWithBaby", false);
            _animator.SetBool("Walking", _grabbedBabies == 0);
            _animator.SetBool("WalkingWithBaby", _grabbedBabies > 0);
            _spriteRenderer.flipY = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if(_movement.y < 0)
        {
            _animator.SetBool("IdleWithBaby", false);
            _animator.SetBool("Walking", _grabbedBabies == 0);
            _animator.SetBool("WalkingWithBaby", _grabbedBabies > 0);
            _spriteRenderer.flipY = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            _animator.SetBool("Walking", false);
            _animator.SetBool("WalkingWithBaby", false);
            _animator.SetBool("IdleWithBaby", _grabbedBabies > 0);
            _spriteRenderer.flipY = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
