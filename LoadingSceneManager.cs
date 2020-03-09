using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    private GameObject loadCircle;

    [SerializeField]
    Image progressBar;
    // Start is called before the first frame update
    void Start()
    {
        loadCircle = GameObject.Find("Circle");
        int ran = Random.Range(1, 8);
        GameObject creature = Instantiate<GameObject>(PublicLevel.friendlyPrefab[ran,0],loadCircle.transform);
        creature.GetComponent<DefaultCreature>().Loading();
        creature.GetComponent<DefaultCreature>().SetCreature(loadCircle.transform.position, loadCircle.transform.position, 0, 0, GameControl.Sides.Friendly);

        creature.transform.localScale = new Vector3(creature.transform.localScale.x * 1.5f, creature.transform.localScale.y * 1.5f, creature.transform.localScale.z *1.5f);
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while(!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                /*progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if(progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }*/
            }
            else
            {
                //progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                //if(progressBar.fillAmount == 1.0f)
                //{
                 op.allowSceneActivation = true;
                 yield break;
                //}
            }
        }
    }
}
