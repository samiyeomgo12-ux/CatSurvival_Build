using UnityEngine;
using System.Collections;

public abstract class NewPickupItemBase : MonoBehaviour
{
    [Header("����")]
    public float autoDisableDelay = 0.05f;

    protected Collider2D col;
    protected bool picked;

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    protected virtual void OnEnable()
    {
        ResetPickupState(); 
    }
    protected void ResetPickupState()
    {
        picked = false;
        if (col) col.enabled = true;
    }

    protected virtual bool CanPickup(Collider2D other)
    {
        return other.CompareTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (picked || !CanPickup(other)) return;

        picked = true;
        if(col) col.enabled = false;

        OnCollected(other);
    }

    protected abstract void OnCollected(Collider2D other);
    
    protected IEnumerator DisableSoon(float delay = -1 )
    {
        yield return new WaitForSeconds(delay >= 0f ? delay : autoDisableDelay);
        gameObject.SetActive(false);
    }
}
