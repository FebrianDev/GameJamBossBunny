using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private Transform[] bg;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var b in bg)
        {
            b.localPosition = Vector3.MoveTowards(b.localPosition, new Vector3(0.5f,2.866188f, 0.5318106f), 0.1f);

            if (b.localPosition.x <= 0.5)
            {
                b.transform.localPosition = new Vector3(86.5f, 2.866188f, 0.5318106f);
            }
        }
    }
}
