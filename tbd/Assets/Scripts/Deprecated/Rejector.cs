using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rejector : MonoBehaviour {

    public float rejectTime;
    private float rejectTimeLeft;
    private GameObject player;
    private Reject rejectComponent;

    public GameObject rejectorParticleEffectPrefab;
    private GameObject rejectorParicleEffect;
    private Vector3 previousPosition;

    // Use this for initialization
    void Start () 
    {
        rejectTimeLeft = rejectTime;
        Vector3 position = transform.position;
        rejectorParicleEffect = Instantiate(rejectorParticleEffectPrefab);
        rejectorParicleEffect.transform.position = position;
    }
  
    // Update is called once per frame
    void Update () 
    {
        rejectTimeLeft -= Time.deltaTime;

        if(rejectTimeLeft <= 0.0f)
        {
            Reject reject = player.GetComponent<Reject>();
            Vector3 reject_dir = rejectorParicleEffect.transform.position - previousPosition;
            reject.RejectInDirection(reject_dir);

            Destroy(gameObject);
            ParticleSystem psys = rejectorParicleEffect.GetComponent<ParticleSystem>();
            psys.enableEmission = false;
            DestroyRequest destroy_request = rejectorParicleEffect.GetComponent<DestroyRequest>();
            destroy_request.Destroy();
        }

        Vector3 start_position = transform.position;
        Vector3 end_position = player.transform.position;
        Vector3 delta = end_position - start_position;

        float perc = (1.0f - rejectTimeLeft) / rejectTime;
        Vector3 effect_position = start_position + perc * delta;
        previousPosition = rejectorParicleEffect.transform.position;
        rejectorParicleEffect.transform.position = effect_position;
    }

    public void SetPositionAndPlayer(Vector3 position, GameObject player)
    {
        transform.position = position;
        this.player = player;
        rejectComponent = player.GetComponent<Reject>();
    }
}
