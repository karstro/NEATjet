using UnityEngine;

public class ConfigurableJointCreatureBuilder : ICreatureBuilder {
    private ConfigurableJointCreatureInternals creature;

    public void Reset() {
        creature = new ConfigurableJointCreatureInternals();
    }

    public void InitializeParentObject() {
        creature.Object = new GameObject();
    }

    public void InitializePhysicsBody() {
        creature.PhysicsBody = creature.Object.AddComponent<Rigidbody>();
        creature.PhysicsBody.mass = 1f;
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        creature.SetPositionAndRotation(position, rotation);
    }

    // sets various parameters that describe the creature's limits
    public void InitializeCreatureLimits(float radius, float maxThrustChangePerSecond, float thrustToWeight) {
        creature.Radius = radius;
        creature.MaxThrustChangePerSecond = maxThrustChangePerSecond;
        // #TODO make maxthrust equal the thrust an individual jet can do to achieve the correct thrustToWeight ratio
        creature.MaxThrust = thrustToWeight;
    }

    // sets various parameters that describe the creature's jets
    public void InitializeJetParameters(int jets, float jetLength, float jetRadius, Vector3 jetArm) {
        creature.Jets = jets;
        creature.JetLength = jetLength;
        creature.JetRadius = jetRadius;
        creature.JetArm = jetArm;
    }

    // calculates the locations of the jet's start and end in the creature's local space
    private (Vector3, Vector3) CalculateJetStartAndEnd(int jetIndex) {
        // calculate which direction the jetArm points
        Quaternion armRotation = CreatureInternals.CalculateJetArmRotation(jetIndex, creature.Jets);
        // the start of a jet is at the end of the jetArm
        Vector3 jetStart = armRotation * creature.JetArm;
        // when created, a creature's jets always point down
        Vector3 jetVector = creature.JetLength * Vector3.down;
        // the end of the jet is at the end of the jetVector, starting from the jetStart
        Vector3 jetEnd = jetStart + jetVector;
        return (jetStart, jetEnd);
    }

    // create a new GameObject with the given position, rotation and parent
    private GameObject InitializeGameObject(Vector3 position, Quaternion rotation, Transform parentTransform, float diameter) {
        GameObject Object = new();
        Transform transform = Object.transform;
        transform.parent = parentTransform;
        transform.SetLocalPositionAndRotation(position, rotation);
        transform.localScale = diameter * Vector3.one;
        return Object;
    }

    // create a GameObject for the start and end of each jet
    public void InitializeCreatureGameObjects() {
        // initialize the body's object
        Transform parentTransform = creature.Object.transform;
        float bodyDiameter = 2 * creature.Radius;
        creature.BodyObject = InitializeGameObject(Vector3.zero, Quaternion.identity, parentTransform, bodyDiameter);

        // initialize the arrays that will track the jets' objects
        creature.JetStarts = new GameObject[creature.Jets];
        creature.JetEnds = new GameObject[creature.Jets];

        // create a GameObject at the start and end of each jet
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++)
        {
            // calculate where the current jet is
            (Vector3 jetStart, Vector3 jetEnd) = CalculateJetStartAndEnd(jetIndex);
            Quaternion jetRotation = CreatureInternals.CalculateJetArmRotation(jetIndex, creature.Jets);

            // create the objects at the calculated position and rotation
            float jetDiameter = creature.JetRadius * 2;
            creature.JetStarts[jetIndex] = InitializeGameObject(jetStart, jetRotation, parentTransform, jetDiameter);
            creature.JetEnds[jetIndex] = InitializeGameObject(jetEnd, jetRotation, parentTransform, jetDiameter);
        }
    }

    // creates a SphereCollider on the given GameObject
    private SphereCollider InitializeSphereCollider(GameObject parent) {
        // create the collider component
        SphereCollider collider = parent.AddComponent<SphereCollider>();
        // all needed colliders must be at the center of their parent object
        collider.center = Vector3.zero;
        // the radius is half the localScale (localScale is the diameter)
        collider.radius = 0.5f;
        return collider;
    }

    // initialize the creature's colliders
    public void InitializeColliders() {
        // initialize the creature's body's collider
        InitializeSphereCollider(creature.BodyObject);

        // initialize the Jets' colliders
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            // initialize the jetStart and end colliders
            InitializeSphereCollider(creature.JetStarts[jetIndex]);
            InitializeSphereCollider(creature.JetEnds[jetIndex]);
        }
    }

    // create a Rigidbody on the given GameObject
    private Rigidbody CreateRigidbody(GameObject parent, float mass) {
        Rigidbody rigidbody = parent.AddComponent<Rigidbody>();
        rigidbody.mass = mass;
        return rigidbody;
    }

    // initialize the creature's Rigidbodies
    public void InitializeRigidbodies(float jetMass) {
        // create a Rigidbody for each jet's start and end
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            CreateRigidbody(creature.JetStarts[jetIndex], jetMass);
            CreateRigidbody(creature.JetEnds[jetIndex], jetMass);
            //float fraction = .95f;
            //CreateRigidbody(creature.JetStarts[jetIndex], jetMass * fraction);
            //CreateRigidbody(creature.JetEnds[jetIndex], jetMass * (1 - fraction));
        }
    }

    // create a ConfigurableJoint on the given GameObject
    private ConfigurableJoint CreateJoint(GameObject parent, Rigidbody connectedObject, float spring, float damper) {
        // create the ConfigurableJoint
        ConfigurableJoint joint = parent.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedObject;
        // #TODO make jetEnds not collide with BodyObject

        // lock unwanted motion
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // move toward target by spherical interpolation
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        JointDrive drive = joint.slerpDrive;
        drive.positionSpring = spring;
        drive.positionDamper = damper;
        joint.slerpDrive = drive;

        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.massScale = 2f;
        joint.connectedMassScale = 0.5f;

        return joint;
    }

    // create the creature's Joints and initialize their settings
    public void InitializeJoints(float spring, float damper) {
        creature.JetJoints = new ConfigurableJoint[creature.Jets];
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            // gather the objects to be connected by the joint
            GameObject jetStart = creature.JetStarts[jetIndex];
            Rigidbody jetEndBody = creature.JetEnds[jetIndex].GetComponent<Rigidbody>();
            Rigidbody creatureBody = creature.PhysicsBody;

            // attach the jetStart and jetEnd with a FixedJoint
            FixedJoint fixedJoint = jetStart.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = jetEndBody;

            // attach the JetStart GameObject to the creature's body with a configurable joint
            creature.JetJoints[jetIndex] = CreateJoint(jetStart, creatureBody, spring, damper);
        }
    }

    // retrieve a copy of the mesh and material of the primitive sphere
    private (Mesh, Material) GetSphereMeshAndMaterial() {
        // create a primitive sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // copy the necessary components
        Mesh sphereMesh = sphere.GetComponent<MeshFilter>().mesh;
        Mesh sphereMeshCopy = Object.Instantiate(sphereMesh);
        Material material = sphere.GetComponent<MeshRenderer>().material;
        Material materialCopy = Object.Instantiate(material);
        // destroy the sphere, including its original components
        Object.Destroy(sphere);
        // return the copies
        return (sphereMeshCopy, materialCopy);
    }

    // add a mesh and renderer to given component so the unity engine can render it
    private void InitializeSphereModel(GameObject parent, Mesh sphereMesh, Material material) {
        // create MeshFilter and apply the mesh of a sphere
        MeshFilter filter = parent.AddComponent<MeshFilter>();
        filter.mesh = sphereMesh;

        // create MeshRenderer and apply default material
        MeshRenderer renderer = parent.AddComponent<MeshRenderer>();
        renderer.material = material;
    }

    private void InitializeBodyModel(Mesh sphereMesh, Material defaultMaterial) {
        // add a sphere model to the body of the creature
        // #TODO optionally replace this with the utah teapot for fun
        InitializeSphereModel(creature.BodyObject, sphereMesh, defaultMaterial);
    }

    private void InitializeJetEndModels(Mesh sphereMesh, Material material) {
        // add a sphere model to each of the jet's starts and ends
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            InitializeSphereModel(creature.JetStarts[jetIndex], sphereMesh, material);
            InitializeSphereModel(creature.JetEnds[jetIndex], sphereMesh, material);
        }
    }

    // build a cylinder whose ends are at the given start and end positions
    private GameObject InitializeCylinderFromTo(Transform parent, Vector3 start, Vector3 end, float width) {
        // create a cylinder
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.parent = parent;
        //Cylinder.name = name;

        // the cylinder's center is the position between the start and end positions
        cylinder.transform.localPosition = (start + end) / 2;

        // change the rotation of the cylinder such that it points from the start toward end
        Vector3 startToEnd = end - start;
        cylinder.transform.localRotation = Quaternion.FromToRotation(Vector3.up, startToEnd);

        // shape the cylinder so that it is exacly long enough to touch start and end
        float length = startToEnd.magnitude / 2;
        cylinder.transform.localScale = new Vector3(width, length, width);

        //remove unused components
        Collider collider = cylinder.GetComponent<Collider>();
        Object.Destroy(collider);
        Rigidbody body = cylinder.GetComponent<Rigidbody>();
        Object.Destroy(body);

        return cylinder;
    }

    private void InitializeJetLimbModels() {
        // initialize the arrays that will hold the objects
        creature.JetArms = new GameObject[creature.Jets];
        creature.JetLegs = new GameObject[creature.Jets];
        float jetWidth = 2 * creature.JetRadius;

        // create a GameObject at the start and end of each jet
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            // calculate where the current jet is
            (Vector3 jetStart, Vector3 jetEnd) = CalculateJetStartAndEnd(jetIndex);
            Transform parentTransform = creature.Object.transform;

            // create the objects and apply the calculated position and rotation
            creature.JetArms[jetIndex] = InitializeCylinderFromTo(parentTransform, Vector3.zero, jetStart, jetWidth);
            creature.JetLegs[jetIndex] = InitializeCylinderFromTo(parentTransform, jetStart, jetEnd, jetWidth);
        }
    }

    public void InitializeCreatureModel() {
        // get the material and mesh to use for the spheres
        (Mesh sphereMesh, Material defaultMaterial) = GetSphereMeshAndMaterial();

        InitializeBodyModel(sphereMesh, defaultMaterial);
        InitializeJetEndModels(sphereMesh, defaultMaterial);
        InitializeJetLimbModels();
    }

    // initialize the thrusts array to all zeroes
    public void InitializeThrusts() {
        creature.ThrustFractions = new float[creature.Jets];
        for (int jetIndex = 0; jetIndex < creature.Jets; jetIndex++) {
            creature.ThrustFractions[jetIndex] = 0f;
        }
    }

    public ICreatureInternals GetResult() {
        return creature;
    }
}
