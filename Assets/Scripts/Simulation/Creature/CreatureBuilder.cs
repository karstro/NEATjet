using UnityEngine;

public class CreatureBuilder : ICreatureBuilder
{
    protected ICreatureInternals Creature { get; set; }

    public virtual void Reset()
    {
        Creature = new ColliderCreatureInternals();
    }

    public void InitializeParentObject()
    {
        Creature.Object = new()
        {
            name = "Creature"
        };
    }

    public void InitializePhysicsBody()
    {
        Creature.PhysicsBody = Creature.Object.AddComponent<Rigidbody>();
        Creature.PhysicsBody.mass = 1f;
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        Creature.SetPositionAndRotation(position, rotation);
    }

    /// <summary>
    /// sets various parameters that describe the creature's limits
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="maxThrustChangePerSecond"></param>
    /// <param name="thrustToWeight"></param>
    public void InitializeCreatureLimits(float radius, float maxThrustChangePerSecond, float thrustToWeight)
    {
        Creature.Radius = radius;
        Creature.MaxThrustChangePerSecond = maxThrustChangePerSecond;
        // #TODO make maxthrust equal the thrust an individual jet can do to achieve the correct thrustToWeight ratio
        Creature.MaxThrust = thrustToWeight;
    }

    /// <summary>
    /// sets various parameters that describe the creature's jets
    /// </summary>
    /// <param name="jets"></param>
    /// <param name="jetLength"></param>
    /// <param name="jetRadius"></param>
    /// <param name="jetArm"></param>
    public void InitializeJetParameters(int jets, float jetLength, float jetRadius, Vector3 jetArm)
    {
        Creature.Jets = jets;
        Creature.JetLength = jetLength;
        Creature.JetRadius = jetRadius;
        Creature.JetArm = jetArm;
    }

    /// <summary>
    /// calculates the locations of the jet's start and end in the creature's local space
    /// </summary>
    /// <param name="jetIndex"></param>
    /// <returns></returns>
    private (Vector3, Vector3) CalculateJetStartAndEnd(int jetIndex)
    {
        // calculate which direction the jetArm points
        var armRotation = CreatureInternals.CalculateJetArmRotation(jetIndex, Creature.Jets);
        // the start of a jet is at the end of the jetArm
        var jetStart = armRotation * Creature.JetArm;
        // when created, a creature's jets always point down
        var jetVector = Creature.JetLength * Vector3.down;
        // the end of the jet is at the end of the jetVector, starting from the jetStart
        var jetEnd = jetStart + jetVector;
        return (jetStart, jetEnd);
    }

    /// <summary>
    /// create a new GameObject with the given position, rotation and parent
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="parentTransform"></param>
    /// <param name="diameter"></param>
    /// <returns></returns>
    private GameObject InitializeGameObject(Vector3 position, Quaternion rotation, Transform parentTransform, float diameter)
    {
        var gameObject = new GameObject();
        var transform = gameObject.transform;
        transform.parent = parentTransform;
        transform.SetLocalPositionAndRotation(position, rotation);
        transform.localScale = diameter * Vector3.one;
        return gameObject;
    }

    /// <summary>
    /// create a GameObject for the start and end of each jet
    /// </summary>
    public void InitializeCreatureGameObjects()
    {
        // initialize the body's object
        var parentTransform = Creature.Object.transform;
        var bodyDiameter = 2 * Creature.Radius;
        Creature.BodyObject = InitializeGameObject(Vector3.zero, Quaternion.identity, parentTransform, bodyDiameter);

        // initialize the arrays that will track the jets' objects
        Creature.JetStarts = new GameObject[Creature.Jets];
        Creature.JetEnds = new GameObject[Creature.Jets];

        // create a GameObject at the start and end of each jet
        for (var jetIndex = 0; jetIndex < Creature.Jets; jetIndex++)
        {
            // calculate where the current jet is
            (var jetStart, var jetEnd) = CalculateJetStartAndEnd(jetIndex);
            var jetRotation = CreatureInternals.CalculateJetArmRotation(jetIndex, Creature.Jets);

            // create the objects at the calculated position and rotation
            var jetDiameter = Creature.JetRadius * 2;
            Creature.JetStarts[jetIndex] = InitializeGameObject(jetStart, jetRotation, parentTransform, jetDiameter);
            Creature.JetEnds[jetIndex] = InitializeGameObject(jetEnd, jetRotation, parentTransform, jetDiameter);
        }
    }

    /// <summary>
    /// creates a SphereCollider at the center of the given GameObject
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    private SphereCollider InitializeSphereCollider(GameObject parent, float radius)
    {
        // create the collider component
        var collider = parent.AddComponent<SphereCollider>();
        // all needed colliders must be at the center of their parent object
        collider.center = Vector3.zero;
        collider.radius = radius;
        // #TODO make colliders collide only with the floor
        return collider;
    }

    /// <summary>
    /// initialize the creature's colliders
    /// </summary>
    public void InitializeColliders()
    {
        // initialize the creature's body's collider
        var radiusForBody = Creature.Radius;
        InitializeSphereCollider(Creature.Object, radiusForBody);

        // initialize the Jets' colliders
        for (var jetIndex = 0; jetIndex < Creature.Jets; jetIndex++)
        {
            // the radius is half the localScale (localScale is the diameter)
            var radiusForJets = 0.5f;
            // initialize the jetStart and end colliders
            InitializeSphereCollider(Creature.JetStarts[jetIndex], radiusForJets);
            InitializeSphereCollider(Creature.JetEnds[jetIndex], radiusForJets);
        }
    }

    /// <summary>
    /// create a Rigidbody on the given GameObject
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="mass"></param>
    /// <returns></returns>
    private Rigidbody CreateRigidbody(GameObject parent, float mass)
    {
        var rigidbody = parent.AddComponent<Rigidbody>();
        rigidbody.mass = mass;
        return rigidbody;
    }

    /// <summary>
    /// initialize the creature's jets' Rigidbodies
    /// </summary>
    /// <param name="jetMass"></param>
    public virtual void InitializeRigidbodies(float jetMass)
    {
        // create a Rigidbody for each jet's start and end
        for (var jetIndex = 0; jetIndex < Creature.Jets; jetIndex++)
        {
            CreateRigidbody(Creature.JetStarts[jetIndex], jetMass);
            CreateRigidbody(Creature.JetEnds[jetIndex], jetMass);
        }
    }

    #region ModelCreation

    /// <summary>
    /// retrieve a copy of the mesh and material of the primitive sphere
    /// </summary>
    /// <returns></returns>
    private (Mesh, Material) GetSphereMeshAndMaterial()
    {
        // create a primitive sphere
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // copy the necessary components
        var sphereMesh = sphere.GetComponent<MeshFilter>().mesh;
        var sphereMeshCopy = Object.Instantiate(sphereMesh);
        var material = sphere.GetComponent<MeshRenderer>().material;
        var materialCopy = Object.Instantiate(material);
        // destroy the sphere, including its original components
        Object.Destroy(sphere);
        // return the copies
        return (sphereMeshCopy, materialCopy);
    }

    /// <summary>
    /// add a mesh and renderer to given component so the unity engine can render it
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="sphereMesh"></param>
    /// <param name="material"></param>
    private void InitializeSphereModel(GameObject parent, Mesh sphereMesh, Material material)
    {
        // create MeshFilter and apply the mesh of a sphere
        var filter = parent.AddComponent<MeshFilter>();
        filter.mesh = sphereMesh;

        // create MeshRenderer and apply default material
        var renderer = parent.AddComponent<MeshRenderer>();
        renderer.material = material;
    }

    private void InitializeBodyModel(Mesh sphereMesh, Material defaultMaterial)
    {
        // add a sphere model to the body of the creature
        // #TODO optionally replace this with the utah teapot for fun
        InitializeSphereModel(Creature.BodyObject, sphereMesh, defaultMaterial);
    }

    private void InitializeJetEndModels(Mesh sphereMesh, Material material)
    {
        // add a sphere model to each of the jet's starts and ends
        for (var jetIndex = 0; jetIndex < Creature.Jets; jetIndex++)
        {
            InitializeSphereModel(Creature.JetStarts[jetIndex], sphereMesh, material);
            InitializeSphereModel(Creature.JetEnds[jetIndex], sphereMesh, material);
        }
    }

    /// <summary>
    /// build a cylinder whose ends are at the given start and end positions
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private GameObject InitializeCylinderFromTo(Transform parent, Vector3 start, Vector3 end, float width)
    {
        // create a cylinder
        var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.parent = parent;
        //Cylinder.name = name;

        // the cylinder's center is the position between the start and end positions
        cylinder.transform.localPosition = (start + end) / 2;

        // change the rotation of the cylinder such that it points from the start toward end
        var startToEnd = end - start;
        cylinder.transform.localRotation = Quaternion.FromToRotation(Vector3.up, startToEnd);

        // shape the cylinder so that it is exacly long enough to touch start and end
        var length = startToEnd.magnitude / 2;
        cylinder.transform.localScale = new Vector3(width, length, width);

        //remove unused components
        var collider = cylinder.GetComponent<Collider>();
        Object.Destroy(collider);
        var body = cylinder.GetComponent<Rigidbody>();
        Object.Destroy(body);

        return cylinder;
    }

    private void InitializeJetLimbModels()
    {
        // initialize the arrays that will hold the objects for the cylinders
        Creature.JetArms = new GameObject[Creature.Jets];
        Creature.JetLegs = new GameObject[Creature.Jets];
        var jetWidth = 2 * Creature.JetRadius;

        // create a GameObject at the start and end of each jet
        for (var jetIndex = 0; jetIndex < Creature.Jets; jetIndex++)
        {
            // calculate where the current jet is
            (var jetStart, var jetEnd) = CalculateJetStartAndEnd(jetIndex);
            var parentTransform = Creature.Object.transform;

            // create the objects and apply the calculated position and rotation
            Creature.JetArms[jetIndex] = InitializeCylinderFromTo(parentTransform, Vector3.zero, jetStart, jetWidth);
            Creature.JetLegs[jetIndex] = InitializeCylinderFromTo(parentTransform, jetStart, jetEnd, jetWidth);
        }
    }

    public void InitializeCreatureModel()
    {
        // get the material and mesh to use for the spheres
        (var sphereMesh, var defaultMaterial) = GetSphereMeshAndMaterial();

        InitializeBodyModel(sphereMesh, defaultMaterial);
        InitializeJetEndModels(sphereMesh, defaultMaterial);
        InitializeJetLimbModels();
    }

    #endregion

    // initialize the thrusts array to all zeroes
    public void InitializeThrusts()
    {
        Creature.ThrustFractions = new float[Creature.Jets];
        for (var jetIndex = 0; jetIndex < Creature.Jets; jetIndex++)
        {
            Creature.ThrustFractions[jetIndex] = 0f;
        }
    }

    public ICreatureInternals GetResult()
    {
        return Creature;
    }

    // settings unique to ConfigurableJointCreatureInternals
    public virtual void InitializeJoints(float spring, float damper) { }

    // settings unique to ColliderCreatureInternals
    public virtual void InitializeSmoothDamp(float maxSpeed, float smoothTime) { }
    public virtual void InitializeAngularVelocities() { }
}
