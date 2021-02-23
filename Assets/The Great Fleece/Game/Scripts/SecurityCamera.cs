using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    private Animator sc_Animator;
    private MeshRenderer sc_Renderer;
    private float gameOverDelay = 0.5f;
    private Color red = new Color(0.6f, 0.1f, 0.1f, 0.3f);

    void Awake()
    {
        sc_Animator = gameObject.GetComponentInParent<Animator>();
        sc_Renderer = gameObject.GetComponent<MeshRenderer>();

        Helpers.Validation.VerifyComponents(gameObject, sc_Animator, sc_Renderer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine("EndGameRoutine");
        }
    }

    IEnumerator EndGameRoutine()
    {
        sc_Animator.enabled = false;

        Color color = sc_Renderer.material.GetColor("_TintColor");

        while (color != red)
        {
            sc_Renderer.material.SetColor("_TintColor", Color.Lerp(color, red, 0.1f));
            color = sc_Renderer.material.GetColor("_TintColor");
            yield return null;
        }

        yield return new WaitForSeconds(gameOverDelay);

        GameManager.Instance.GameOver();
    }
}
