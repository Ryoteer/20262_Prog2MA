using System.Collections;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [Header("<color=blue>Gameplay</color>")]
    [SerializeField] private float _fadeTimeScale = 5.0f;
    [SerializeField] private float _fadeInterval = 3.0f;

    private bool _isActive = false;

    private Collider _collider;
    private Material _material;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerBehaviour>() && !_isActive)
        {
            StartCoroutine(FadePlatform());
        }
    }

    private IEnumerator FadePlatform()
    {
        _isActive = true;

        float t = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime / _fadeTimeScale;

            _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, Mathf.Lerp(1.0f, 0.0f, t));

            yield return null;
        }

        _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, 0.0f);

        _collider.enabled = false;

        yield return new WaitForSeconds(_fadeInterval);

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _fadeTimeScale;

            _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, Mathf.Lerp(0.0f, 1.0f, t));

            yield return null;
        }

        _material.color = new Color(_material.color.r, _material.color.g, _material.color.b, 1.0f);

        _collider.enabled = true;

        _isActive = false;
    }
}
