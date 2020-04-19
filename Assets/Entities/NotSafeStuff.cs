using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class NotSafeStuff : MonoBehaviour
{
    public static string TAG_NAME = "NotSafeStuff";
    public string name;
    public enum DamageType { Drowning, Burn, ElectricShock, PhysicalDamage }
    public DamageType damageType;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        if(_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    public void DoDamage()
    {
        if (_animator != null)
        {
            Debug.Log(string.Format("{0} is killing a baby", name));
            _animator?.SetTrigger("Kill");
        }
    }
}
