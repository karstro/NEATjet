using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICreatureBuilder
{
    public void Reset();
    public void InitializeParentObject();
    public void InitializePhysicsBody();
    public void InitializeCreatureLimits(
        float radius,
        float maxThrustChangePerSecond,
        float thrustToWeight
    );
    public void InitializeJetParameters(
        int jets,
        float jetLength,
        float jetRadius,
        Vector3 jetArm
    );
    public void InitializeThrusts();
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation);
    public void InitializeCreatureGameObjects();
    public void InitializeColliders();
    public void InitializeRigidbodies(float jetMass);
    public void InitializeJoints(float spring, float damper);
    public void InitializeCreatureModel();
    public void SetBrain(IBrain brain);
    public Creature GetResult();
}
