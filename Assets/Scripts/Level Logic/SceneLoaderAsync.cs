using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderAsync : MonoBehaviour //Singleton<SceneLoaderAsync>
{
    public static SceneLoaderAsync Instance;

    [Tooltip("To prevent too fast loading - suffering from success :D")]
    public float minimalSceneLoadingTime;
    float timeLoadingStarted;

    // Loading Progress: private setter, public getter
    private float _loadingProgress;
    public float LoadingProgress { get { return _loadingProgress; } }

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }

        
    }


    public void LoadNewScene(string newSceneName)
    {
        // kick-off the one co-routine to rule them all
        StartCoroutine(LoadScenesInOrder(newSceneName));
        timeLoadingStarted = Time.time;

        MusicManager.Instance.StartPlayingLoadingMusic();
    }

    private IEnumerator LoadScenesInOrder(string newSceneName)
    {
        // LoadSceneAsync() returns an AsyncOperation, 
        // so will only continue past this point when the Operation has finished
        yield return SceneManager.LoadSceneAsync("Loading");

        // as soon as we've finished loading the loading screen, start loading the game scene
        yield return StartCoroutine(LoadScene(newSceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        var asyncScene = SceneManager.LoadSceneAsync(sceneName);

        // this value stops the scene from displaying when it's finished loading
        asyncScene.allowSceneActivation = false;

        while (!asyncScene.isDone)
        {
            // loading bar progress
            _loadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;
            Debug.Log("loading progress: " + _loadingProgress);

            // scene has loaded as much as possible, the last 10% can't be multi-threaded
            if (asyncScene.progress >= 0.9f)
            {
                if((Time.time - timeLoadingStarted )> minimalSceneLoadingTime)
                {
                    asyncScene.allowSceneActivation = true;
                }
                // we finally show the scene
               
            }

            yield return null;
        }
    }
}