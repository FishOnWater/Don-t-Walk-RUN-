using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VectorScript : MonoBehaviour
{
    public float damage;
    public float range;
    public float fireRate;
    private float nextTimeToFire = 0.0f;
    public ParticleSystem muzzle;
    public GameObject hitParticle;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void alterTimer()
    {
       
    }

    void Shoot()
    {
        Debug.Log("SHOOT");
        muzzle.Play();
        RaycastHit hit;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Physics.Raycast(transform.position, transform.right, out hit, 100.0f))
        {
            if (hit.collider.enabled)
            {
                Debug.Log(hit.transform.name);

                if (GameObject.FindGameObjectWithTag("Terrain"))
                {
                    GameObject impactParticle = Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactParticle, 1.0f);
                }
            }
        }
    }
}