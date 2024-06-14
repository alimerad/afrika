using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escalator : MonoBehaviour
{
    public GameObject m_BottomPos; 
    public GameObject m_TopPos;

    public float m_Speed;

    [SerializeField] GameEvent m_EscalatorHasReachedTarget; 

    public void OnDoorHasBeenClosed(Component sender, object data)
    {
        if (sender != transform.root) return;

        Vector3 target = Vector3.Distance(transform.position, m_BottomPos.transform.position) > Vector3.Distance(transform.position, m_TopPos.transform.position) ? m_BottomPos.transform.position : m_TopPos.transform.position;
        
        StartCoroutine(LerpPosition(target, 5));

    }


    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        m_EscalatorHasReachedTarget.Raise(transform.root);
    }

}
