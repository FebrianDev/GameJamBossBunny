using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    [SerializeField]private Animator [] anim;
    [SerializeField]private GameObject [] go;

    [SerializeField] private Transform bg;
    void Start()
    {
        StartCoroutine("StartAnimation");
    }

    // Update is called once per frame
    void Update()
    {
        bg.position += Vector3.left * (Time.deltaTime * 30f);
    }

    IEnumerator StartAnimation()
    {
        anim[0].enabled = true;
        anim[1].enabled = false;
        anim[2].enabled = false;
        go[0].SetActive(true);
        go[1].SetActive(false);
        go[2].SetActive(false);
        yield return new WaitForSeconds(5);
        anim[0].enabled = false;
        anim[1].enabled = true;
        anim[2].enabled = false;
        go[0].SetActive(false);
        go[1].SetActive(true);
        go[2].SetActive(false);
        yield return new WaitForSeconds(6);
        anim[0].enabled = false;
        anim[1].enabled = false;
        anim[2].enabled = true;
        go[0].SetActive(false);
        go[1].SetActive(false);
        go[2].SetActive(true);

        SceneManager.LoadScene("MainMenu");
    }

    public void Skip()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
