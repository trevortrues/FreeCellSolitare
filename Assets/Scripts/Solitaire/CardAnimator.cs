using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimator : MonoBehaviour
{
    public static CardAnimator Instance { get; private set; }

    [Header("Animation Settings")]
    public float moveDuration = 0.25f;
    public float cascadeDelay = 0.05f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private int activeAnimations = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsAnimating()
    {
        return activeAnimations > 0;
    }

    public void AnimateCard(GameObject card, Vector3 targetPosition, Action onComplete = null)
    {
        if (card == null)
        {
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(AnimateCardCoroutine(card, targetPosition, onComplete));
    }

    public void AnimateCascade(List<GameObject> cards, List<Vector3> targetPositions, Action onComplete = null)
    {
        if (cards == null || cards.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(AnimateCascadeCoroutine(cards, targetPositions, onComplete));
    }

    private IEnumerator AnimateCardCoroutine(GameObject card, Vector3 targetPosition, Action onComplete)
    {
        activeAnimations++;

        Vector3 startPosition = card.transform.position;
        float elapsed = 0f;

        float originalZ = targetPosition.z;
        Vector3 animationTarget = new Vector3(targetPosition.x, targetPosition.y, -10f);

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsed / moveDuration);

            Vector3 currentPos = Vector3.Lerp(startPosition, animationTarget, t);
            card.transform.position = currentPos;

            yield return null;
        }

        card.transform.position = targetPosition;

        activeAnimations--;
        onComplete?.Invoke();
    }

    private IEnumerator AnimateCascadeCoroutine(List<GameObject> cards, List<Vector3> targetPositions, Action onComplete)
    {
        int completedAnimations = 0;
        int totalAnimations = cards.Count;

        for (int i = 0; i < cards.Count; i++)
        {
            int index = i;
            AnimateCard(cards[i], targetPositions[i], () =>
            {
                completedAnimations++;
            });

            if (i < cards.Count - 1)
            {
                yield return new WaitForSeconds(cascadeDelay);
            }
        }

        while (completedAnimations < totalAnimations)
        {
            yield return null;
        }

        onComplete?.Invoke();
    }

    public void AnimateCardImmediate(GameObject card, Vector3 targetPosition)
    {
        if (card != null)
        {
            card.transform.position = targetPosition;
        }
    }
}
