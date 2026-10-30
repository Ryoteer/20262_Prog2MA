using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    #region Instance
    public static LoadManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _input = new PlayerInputActions();
    }
    #endregion

    [Header("<color=orange>UI</color>")]
    [Range(0.1f, 2.0f)][SerializeField] private float _fadeTimeScale = 0.25f;
    [SerializeField] private Image _loadBarFill;

    private bool _isLoading = false, _isPressed = false;
    public bool IsLoading { get { return _isLoading; } }
    
    private Image[] _uiImages;
    private PlayerInputActions _input;

    private void OnEnable()
    {
        _input.Enable();

        _input.Menu.Select.performed += SelectPerformed;
        _input.Menu.Select.canceled += SelectCanceled;
    }

    private void OnDisable()
    {
        _input.Disable();

        _input.Menu.Select.performed -= SelectPerformed;
        _input.Menu.Select.canceled -= SelectCanceled;
    }

    private void Start()
    {       
        _uiImages = GetComponentsInChildren<Image>();

        foreach (Image image in _uiImages)
        {
            image.color = new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b, 0.0f);
        }
    }

    public void LoadSceneAsync(string sceneName)
    {
        if (!_isLoading)
        {
            StartCoroutine(LoadSceneAsyncOp(sceneName));
        }
    }

    private IEnumerator LoadSceneAsyncOp(string sceneName)
    {
        _isLoading = true;

        float t = 0.0f;

        _loadBarFill.fillAmount = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime / _fadeTimeScale;

            foreach(Image image in _uiImages)
            {
                image.color = Color.Lerp(
                    new Color(
                    image.color.r, 
                    image.color.g, 
                    image.color.b, 0.0f),
                    new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b, 1.0f)
                    , t);
            }

            yield return null;
        }

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        asyncOp.allowSceneActivation = false;

        while(asyncOp.progress < 0.9f)
        {
            _loadBarFill.fillAmount = asyncOp.progress / 0.9f;

            yield return null;
        }

        _loadBarFill.fillAmount = 1.0f;

        while (!_isPressed)
        {
            yield return null;
        }

        asyncOp.allowSceneActivation = true;

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _fadeTimeScale;

            foreach (Image image in _uiImages)
            {
                image.color = Color.Lerp(
                    new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b, 1.0f),
                    new Color(
                    image.color.r,
                    image.color.g,
                    image.color.b, 0.0f)
                    , t);
            }

            yield return null;
        }

        _isLoading = false;
    }

    private void SelectPerformed(InputAction.CallbackContext value)
    {
        _isPressed = true;
    }

    private void SelectCanceled(InputAction.CallbackContext value)
    {
        _isPressed = false;
    }
}
