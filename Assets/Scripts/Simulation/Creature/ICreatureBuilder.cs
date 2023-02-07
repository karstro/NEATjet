using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICreatureBuilder
{
    public void Reset();
    public void InitializeGameObject();
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation);
    public void InitializePhysicsBody();
    public void InitializeCreatureLimits(
        float radius,
        float maxJetAngleChangePerSecond,
        float thrustToWeight
    );
    public void InitializeJets(
        int jets,
        float jetLength,
        float jetRadius,
        Vector3 jetArm
    );
    public void SetJetAngles(Vector3[] jetAngles);
    public void SetJetAngles();
    public void SetThrusts(float[] thrusts);
    public void SetThrusts();
    public void InitializeColliders();
    public void SetBrain(IBrain brain);
    public Creature GetResult();
}
