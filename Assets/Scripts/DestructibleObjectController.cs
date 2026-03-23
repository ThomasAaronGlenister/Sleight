using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectController : MonoBehaviour
{

    [SerializeField] private int mnNumberofParticles;

    [SerializeField] private int ForceMulitplierMax = 5;
    [SerializeField] private int ForceMulitplierMin = 1;

    //Prefab Particle Object used for sub attacks
    public GameObject mcParticlePrefab;

    private Sprite[] macParticleSprites;

    public Sprite TestSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Trigger for this Object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("DestructibleObject: Destroy");

        float lfRandomAngle = 0;
        int ForceMultiple = 0;

        for (int lnPart = 0; lnPart < mnNumberofParticles; lnPart++)
        {
            GameObject lcParticle = Instantiate(mcParticlePrefab, this.transform.position, Quaternion.Euler(0, Random.Range(0f, 360f), 0));

            lcParticle.GetComponent<SpriteRenderer>().sprite = TestSprite;

            lcParticle.transform.localScale *= Random.Range(0.5f, 2f);

            lfRandomAngle = Random.Range(-0.9f, 0.9f);

            ForceMultiple = Random.Range(ForceMulitplierMin, ForceMulitplierMax);

            lcParticle.GetComponent<Rigidbody2D>().AddForce(new Vector2(lfRandomAngle, Mathf.Abs(lfRandomAngle)) * ForceMultiple, ForceMode2D.Impulse);
        }

        Destroy(this.gameObject);
    }
}
