using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this

public class LoadMenuData : MonoBehaviour
{
    private string currScene;
    // Start is called before the first frame update
    void Awake()
    {
        currScene = SceneManager.GetActiveScene().name;

        if (GameObject.FindWithTag("PersistentData") == null) //check if menu has been loaded by looking for PERSISTENT DATA
        {
            DontDestroyOnLoad(this);
            StartCoroutine(LoadMenu());
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator LoadMenu()
    {
        SceneManager.LoadScene("Menu");
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(currScene);
    }
}
