using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Trap
{
    private Animator animator;
    public Dictionary<string, float> settings = new Dictionary<string, float>
    {
        {"startTime", 0f},
        {"onTime", 1f},
        {"offTime", 1f}
    };
    private Coroutine animationCoroutine;


    private void Awake()
    {
        trapType = TrapType.Spike;
        animator = GetComponent<Animator>();
        settings["startTime"] = 0f;
        settings["onTime"] = 1f;
        settings["offTime"] = 1f;
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
        if (animationCoroutine != null)
        {
            animator.SetTrigger("triggerOff");
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(PlayAnimationSequence());
    }

    private IEnumerator PlayAnimationSequence()
    {
        this.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(settings["startTime"]);
        while (true)
        {
            animator.SetTrigger("triggerIn");
            this.GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(settings["onTime"]);
            animator.SetTrigger("triggerOut");
            this.GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(settings["offTime"]);
        }
    }
}