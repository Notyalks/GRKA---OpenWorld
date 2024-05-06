using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{
    public bool canUseJetpack;
    public float jetpackAcceleration = 25f;

    [Range(0f, 1f)]
    public float jetpackDownwardVelocityCancelingFactor = 0.75f;

    public float conssumeDuration = 1.5f;
    public float refillDurationGrounded = 2f;
    public float reffilDurationTheAir = 5f;
    public float refillDelay = 1f;

    public float currentFillRatio;
    float lastTimeOfUse;

    PlayerLocomotion locomotion;
    [SerializeField] private TrailRenderer trailRenderer;

    
    void Start()
    {
       locomotion = GetComponent<PlayerLocomotion>();
        currentFillRatio = 1f;
        trailRenderer.emitting = false;
    }

    void Update()
    {
        if(locomotion.isGrounded)
        {
            canUseJetpack = false;
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            canUseJetpack= true;
        }

        bool jetpackIsInUse = canUseJetpack && currentFillRatio > 0f && Input.GetKeyDown(KeyCode.Space);

        if(jetpackIsInUse)
        {

            trailRenderer.emitting = true;
            lastTimeOfUse = Time.time;
            float totalAcceleration = jetpackAcceleration;
            totalAcceleration += locomotion.gravityIntensity;

            if(locomotion.rb.velocity.y < 0f)
            {
                totalAcceleration += ((-locomotion.rb.velocity.y / Time.deltaTime) * jetpackDownwardVelocityCancelingFactor);
            }

            locomotion.rb.velocity += Vector3.up * totalAcceleration * Time.deltaTime;

            currentFillRatio = currentFillRatio - (Time.deltaTime / conssumeDuration);

        }
        else
        {
            trailRenderer.emitting = false;
            if(Time.time - lastTimeOfUse >= refillDelay)
            {
                float refillRate = 1 / (locomotion.isGrounded ? refillDurationGrounded : reffilDurationTheAir);
                currentFillRatio = currentFillRatio + Time.deltaTime * refillRate;
            }
            currentFillRatio = Mathf.Clamp01(currentFillRatio);
        }
    }
}
