using UnityEngine;

public class ExpItem : NewPickupItemBase
{
    public int expValue = 1;

    [Header("Optional : BoxOpenAnim")]
    public Animator animator;
    public string openBoolName = "boxopen";
    public string openClipName = "boxopen";
    public bool useTrigger = true;

    protected override void Awake()
    {
        base.Awake();
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void Init(int value)
    {
        expValue = Mathf.Max(1, value);
        ResetPickupState();
    }

    protected override void OnCollected(Collider2D other)
    {
        GameFacade.Instance.PlayerExpAdd(expValue);
        if (GameFacade.Instance.IsLive)
            GameFacade.Instance.PlaySfx(Sfx.GetItem);

        bool isBox = AnimatorLooksLikeBox(animator, openBoolName, openClipName);
        if (animator != null && isBox)
        {
            if (useTrigger && HasTrigger(animator, openBoolName))
            {
                GameFacade.Instance.PlaySfx(Sfx.OpenBox);
                animator.ResetTrigger(openBoolName);
                animator.SetTrigger(openBoolName);
            }
            else
            {
                animator.Play(openClipName, 0, 0f);
            }

            float delay = GetClipLength(animator, openClipName);
            if (delay <= 0f) delay = autoDisableDelay;
            StartCoroutine(DisableSoon(delay));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    bool HasTrigger(Animator a, string triggerName)
    {
        if (a == null || string.IsNullOrEmpty(triggerName)) return false;
        foreach (var p in a.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == triggerName)
                return true;
        return false;
    }

    bool AnimatorLooksLikeBox(Animator a, string triggerName, string clipName)
    {
        if (a == null) return false;
        if (HasTrigger(a, triggerName)) return true;
        var rc = a.runtimeAnimatorController;
        if (rc == null) return false;
        foreach (var c in rc.animationClips)
            if (c != null && c.name == clipName) return true;
        return false;
    }

    float GetClipLength(Animator a, string clipName)
    {
        if (a == null) return 0f;
        var rc = a.runtimeAnimatorController;
        if (rc == null) return 0f;
        foreach (var c in rc.animationClips)
            if (c != null && c.name == clipName)
                return c.length;
        return 0f;
    }
}
