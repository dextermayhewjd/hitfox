using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashObject : MonoBehaviour
{
    public Collider collider;
    public Transform objectToSquashTransform;
    private Vector3 originalScale;
    private bool isSquashed;

    private void Start()
    {
        originalScale = objectToSquashTransform.localScale;
        isSquashed = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Forklift") && collision.gameObject.GetComponent<NewCarUserControl>().driving && !isSquashed)
        {
            StartCoroutine(SquashAndRestore());
        }
    }

    private IEnumerator SquashAndRestore()
    {
        // Squash the object and set isSquashed to true
        if (CompareTag("Hedgehog")) {
            objectToSquashTransform.localScale = new Vector3(objectToSquashTransform.localScale.x, objectToSquashTransform.localScale.y, objectToSquashTransform.localScale.z * 0.2f);   
        } else {
            objectToSquashTransform.localScale = new Vector3(objectToSquashTransform.localScale.x, objectToSquashTransform.localScale.y * 0.2f, objectToSquashTransform.localScale.z);
        }
        isSquashed = true;
        collider.enabled = false;
        // Wait for n seconds
        yield return new WaitForSeconds(5f);

        // Restore the NPC to its original size and set isSquashed to false
        collider.enabled = true;
        objectToSquashTransform.localScale = originalScale;
        isSquashed = false;
    }
}
