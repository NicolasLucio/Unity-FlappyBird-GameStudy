using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prefabScript : MonoBehaviour
{
    private Rigidbody2D barrierBody;
    private Vector2 barrierVector;
    
    void Start()
    {
        barrierBody = this.GetComponent<Rigidbody2D>();
        barrierBody.velocity = new Vector2(-5.0f, 0.0f);
    }
    
    void Update()
    {
        barrierVector = transform.position;
        if (barrierVector.x <= -10.0f)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}
