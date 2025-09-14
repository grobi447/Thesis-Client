using System.Collections;
using UnityEngine;

public class SpikeTile : Tile
{
    private Animator animator;
    public float animationFirstStartDelay;
    public float animationStartDelay;
    public float idleTime;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animationFirstStartDelay = 0f;
        animationStartDelay = 1f;
        idleTime = 1f;
    }

    void Start()
    {
        animationCoroutine = StartCoroutine(PlayAnimationSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartTimer();
        }
    }

    public void RestartTimer()
    {
        if (animationCoroutine != null) {
            animator.SetTrigger("triggerOff");
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(PlayAnimationSequence());
    }

    private IEnumerator PlayAnimationSequence()
    {
        yield return new WaitForSeconds(animationFirstStartDelay);
        while (true)
        {
            animator.SetTrigger("triggerIn");
            yield return new WaitForSeconds(idleTime);
            animator.SetTrigger("triggerOut");
            yield return new WaitForSeconds(animationStartDelay);
        }
    }
}