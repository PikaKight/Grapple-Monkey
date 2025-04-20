using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collidable : MonoBehaviour
{
    //variables:

    //filters what objects you collide with
    public ContactFilter2D filter;
    private CapsuleCollider2D capCollidator; 
    private Collider2D[] hits = new Collider2D[10];

    protected virtual void Start(){
        capCollidator = GetComponent<CapsuleCollider2D> ();

    }

    protected virtual void Update(){
        //Collision work
        capCollidator.Overlap(filter, hits);
        for (int i = 0; i < hits.Length; i++){
            if(hits[i] == null){
                 continue;
            }
            OnCollide(hits[i]);


            hits [i] = null;
               
        }
    }

    protected virtual void OnCollide (Collider2D coll){
        Debug.Log(coll.name);
        
    }
    
}
