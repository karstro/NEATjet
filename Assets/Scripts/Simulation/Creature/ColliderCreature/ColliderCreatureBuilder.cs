using UnityEngine;

public class ColliderCreatureBuilder : CreatureBuilder, ICreatureBuilder {
    private ColliderCreatureInternals CCreature { get => (ColliderCreatureInternals) Creature; set => Creature = value; }

    public new void Reset() {
        CCreature = new();
    }

    public new void InitializeSmoothDamp(float maxSpeed, float smoothTime) {
        CCreature.MaxSpeed = maxSpeed;
        CCreature.SmoothTime = smoothTime;
    }

    public new void InitializeAngularVelocities() {
        CCreature.AngularVelocities = new Vector3[CCreature.Jets];
        for (int jetIndex = 0; jetIndex < CCreature.Jets; jetIndex++) {
            CCreature.AngularVelocities[jetIndex] = Vector3.zero;
        }
    }

}
