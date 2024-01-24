using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationBox : MonoBehaviour
{
    public bool delete = false;

    public IEnumerator startDeleteTimer()
    {

        //Debug.Log("In coroutine before yield");
        yield return new WaitForSeconds(10.0f);
        //delete = true;
        //Debug.Log("In coroutine after yield");
        Destroy(gameObject);

       
    }
}
