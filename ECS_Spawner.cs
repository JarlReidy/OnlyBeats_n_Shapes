using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.VisualScripting;
using UnityEngine.UIElements;

using Button = UnityEngine.UI.Button;
using Scale = Unity.Transforms.Scale;

#region Move Objects

public struct MoveObject : IComponentData
{
    public float3 velocity;
    public float3 velocity2;
    public float3 velocity3;
    public float3 velocity4;
    public float3 velocity5;

    public float3 randomVel;//for explosions

    public float3 windVel;
    
}

public class MoveSystem : ComponentSystem
{

    EntityManager entityManager;
    
    

    public int counter = 0;
    public int spherecounter = 0;
    public float3 tracker;

    public Translation playerTrans;

    Vector3 temp;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;//this will manage all of our entities

        
    }

    protected override void OnUpdate()
    {
        // Debug.Log(counter);
        counter++;
        spherecounter++;
        var mult = 1.0f * Time.DeltaTime;//speed multiplier we use
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);


        Entities.WithAll<MoveObject>().ForEach((Entity Entity, ref Translation trans, ref MoveObject move) =>
        {
                //moving the first entity shape
            Entities.WithAll<shape1Tag>().ForEach((Entity shape1entity, ref Translation shape1trans, ref MoveObject moveshape1) =>
            {
                shape1trans.Value += moveshape1.velocity * Time.DeltaTime;
                if (math.lengthsq(shape1trans.Value) > 225.0f * 225.0f)//if the shape goes off screen
                {
                    PostUpdateCommands.DestroyEntity(shape1entity);
                    moveshape1.velocity = 0.5f;
                }
            });
                //});
                // Entities.WithAll<MoveObject>().ForEach((Entity Entity, ref Translation trans, ref MoveObject Move)=>
                //  { 
            Entities.WithAll<shape2Tag>().ForEach((Entity shape2entity, ref Translation shape2trans, ref MoveObject moveshape2) =>
            {
                counter++;
                shape2trans.Value += moveshape2.velocity2 * Time.DeltaTime;

                if (math.lengthsq(shape2trans.Value) > 225.0f * 255.0f)
                {
                    PostUpdateCommands.DestroyEntity(shape2entity);//if shape goes off screen                      
                }

            });

            Entities.WithAll<shape3Tag>().ForEach((Entity shape3entity, ref Translation shape3trans, ref MoveObject moveshape3) =>
            {
                shape3trans.Value += moveshape3.velocity3 * Time.DeltaTime;

                if (math.lengthsq(shape3trans.Value) > 225.0f * 235.05f)
                {
                    PostUpdateCommands.DestroyEntity(shape3entity);
                }
            });


            //Debug.Log(counter);
           

            Entities.WithAll<playerTag>().ForEach((Entity playerentity, ref Translation playertrans, ref MoveObject playermove) =>
            {
                Vector3 targetPosition = playertrans.Value;
                Vector3 shapePosition = Vector3.zero;

                Entities.WithAll<shape4Tag>().ForEach((Entity shape4entity, ref Translation shape4trans, ref MoveObject moveshape4) =>
                {
                    shapePosition = shape4trans.Value;
                    shapePosition = Vector3.MoveTowards(shapePosition, targetPosition, 20.0f * Time.DeltaTime);
                    if(counter > 950)
                    {
                        PostUpdateCommands.DestroyEntity(shape4entity);
                        counter = 0;
                    }

                });

            });

            Entities.WithAll<shape5Tag>().ForEach((Entity shape5entity, ref Translation shape5trans, ref MoveObject moveshape5) =>
            {

                shape5trans.Value += moveshape5.velocity5 * Time.DeltaTime;
                if(spherecounter > 1250)
                {
                    shape5trans.Value = Vector3.MoveTowards(shape5trans.Value, center, 15.0f * Time.DeltaTime);
                    spherecounter = 0;
                }

            
            });
            

            
        });

        Entities.WithAll<shape6Tag>().ForEach((Entity shape6entity, ref Translation shape6trans, ref MoveObject moveshape6) =>
        {
            shape6trans.Value += moveshape6.windVel * Time.DeltaTime;
            if (math.lengthsq(shape6trans.Value) > 300.0f * 300.0f)
            {
               // PostUpdateCommands.DestroyEntity(shape6entity);//if shape goes off screen                      
            }

        });
                

                Entities.ForEach((Entity shapeEntity, ref HasTarget hasTarget, ref Translation trans, ref MoveObject moveshape4) =>
                {
                    Translation targetTranslation = World.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);//getting the target entities transform values

                    //float3 targetMove = math.normalize(targetTranslation.Value - trans.Value);//decreasing the translation differences between them

                    //trans.Value = targetMove * moveshape4.velocity4 * Time.DeltaTime;
                    trans.Value = Vector3.MoveTowards(trans.Value, targetTranslation.Value, 35f * Time.DeltaTime);
                    Debug.Log(targetTranslation.Value);

                    if (math.distance(trans.Value, targetTranslation.Value) < .2f)
                    {
                        //close to target, destroy the target
                        //PostUpdateCommands.DestroyEntity(hasTarget.targetEntity);//destroy the player
                        //PostUpdateCommands.RemoveComponent(shapeEntity, typeof(HasTarget));//remove the hastarget tag so other entities can pursue it

                    }
                });


            

        
           
            
    }
}

#endregion

#region Tracking Shapes

public struct HasTarget:IComponentData
{
    public Entity targetEntity;
}

public class TrackingSystem : ComponentSystem
{

    //private float velocity4 = 1.5f;

    EntityManager entityManager;
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;//this will manage all of our entities
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<shape4Tag>().ForEach((Entity shape4_entity, ref Translation shape4_trans, ref MoveObject move4) =>
        {
            Entity targetEntity = Entity.Null;//checking the closest target entity
            Vector3 shapePosition = shape4_trans.Value;
            //Debug.Log(shapePosition);
            Vector3 targetPosition = Vector3.zero;

            Entities.WithAll<playerTag>().ForEach((Entity playerentity, ref Translation playertrans, ref MoveObject playermove) =>
            {
                

                
                if(targetEntity == Entity.Null)
                {
                    //there is no target
                    targetEntity = playerentity;
                    targetPosition = playertrans.Value;
                }
                else
                {
                    if(math.distance(shapePosition, playertrans.Value) < math.distance(shapePosition, targetPosition))
                    {
                        //closest target becomes new target
                        targetEntity = playerentity;
                        targetPosition = playertrans.Value;
                    }
                }


            });

            if(targetEntity != Entity.Null)
            {
                Debug.DrawLine(shapePosition, targetPosition);
                Debug.Log(targetPosition);
                PostUpdateCommands.AddComponent(shape4_entity, new HasTarget { targetEntity = targetEntity });
            }

           

        });
    }
}


#endregion

#region Kill Objects
public struct KillShape : IComponentData
{
    
}

public class KillSystem : ComponentSystem
{
    public ECS_Spawner spawner = new ECS_Spawner();//referencing the external script
    //we will use this to switch the spawning booleans, preventing us from cluttering the screen too much

    private int numItems;

    void Start()
    {
       
    }
    
    
    protected override void OnUpdate()
    {
        //Debug.Log(spawner.Count);
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Elim();
        }
    }

    private void Elim()
    {
        
        Entities.WithAll<KillShape>().ForEach((Entity shape_entity) =>
        {
            PostUpdateCommands.DestroyEntity(shape_entity);
            
        });
    }
}

#endregion

#region Move Player
public struct MovePlayer : IComponentData
{
    //variables
    public float3 velocity;
    float horizontalInput;
    float verticalInput;

}

public class PlayerSystem : ComponentSystem
{
    private Vector2 boundaries;
    private float spriteWidth;
    private float spriteHeight;

    private float xClamp;
    private float yClamp;

   


    public Vector3 clampPos;
    void Start()
    {
        //boundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        //xClamp = Mathf.Abs(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)).x);
        //yClamp = Mathf.Abs(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)).y);
        spriteWidth = 8.0f;
        spriteHeight = 8.0f;
    }
    
    
    protected override void OnUpdate()
    {
        xClamp = Mathf.Abs(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)).x);
        yClamp = Mathf.Abs(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)).y);
      
        Entities.WithAll<MovePlayer>().ForEach(( ref Translation translate, ref MovePlayer pMove) =>
        {
            float horizontalInput = Input.GetAxis("Horizontal");//x
            float verticalInput = Input.GetAxis("Vertical");//y
            
            pMove.velocity = new float3(horizontalInput, verticalInput, 0.0f);
          
            translate.Value +=  pMove.velocity * 250 * Time.DeltaTime;//moving the player
            
            //checking the bounds
            clampPos = translate.Value;
            clampPos.x = Mathf.Clamp(clampPos.x, -xClamp + spriteWidth/2, xClamp - spriteWidth/2);
            clampPos.y = Mathf.Clamp(clampPos.y, -yClamp + spriteHeight/2, yClamp -spriteHeight/2);
            translate.Value = clampPos;
            

        });
    }
}
#endregion

#region Collision
public class CollideSystem : ComponentSystem
{
    public bool hasHit { get; private set; } //readonly bool

    private int icounter = 0;
  

    void Start()
    {
        //hasHit = false;
    }
    protected override void OnUpdate()
    {
        //Debug.Log(icounter);

        Entities.WithAll<shape1Tag>().ForEach((Entity bEntity, ref Translation bTrans, ref Scale bs) =>
        {
            float3 bpos = bTrans.Value;
            float bsize = bs.Value;

            Entities.WithAll<playerTag>().ForEach((Entity rEntity, ref Translation rTrans, ref Scale rs) =>
            {
                // check for collision here
                float3 rpos = rTrans.Value;
                float rsize = rs.Value;
                if (math.lengthsq(bpos - rpos) < (bsize + rsize) * (bsize + rsize))
                {
                    PostUpdateCommands.DestroyEntity(bEntity);
                    //PostUpdateCommands.DestroyEntity(rEntity);
                }
            });
            Entities.WithAll<shape2Tag>().ForEach((Entity b1Entity, ref Translation b1Trans, ref Scale b1s) =>
            {
                float3 b1pos = b1Trans.Value;
                float b1size = b1s.Value;
                if (math.lengthsq(bpos - b1pos) < (bsize + b1size) * (bsize + b1size))
                {
                    //PostUpdateCommands.Instantiate(b1Entity);//if they collide, a new one is made.
                    //hasHit = true;
                }
            });
        });

        Entities.WithAll<shape2Tag>().ForEach((Entity entity_2, ref Translation trans_2, ref Scale scale_2) =>
        {
            float3 pos_2 = trans_2.Value;
            float size_2 = scale_2.Value;

            //if collides with shape 3
            Entities.WithAll<shape3Tag>().ForEach((Entity entity_3, ref Translation trans_3, ref Scale scale_3) =>
            {
                float3 pos_3 = trans_3.Value;
                float size_3 = scale_3.Value;
                if (math.lengthsq(pos_2 - pos_3) < (size_2 + size_3) * (size_2 + size_3))
                {

                    for (int i = 0; i < 3; i++)
                    {
                        PostUpdateCommands.Instantiate(entity_3);
                        pos_3 = pos_3 / 2;

                    }

                }
            });
        });

        Entities.WithAll<shape5Tag>().ForEach((Entity entity_5, ref Translation trans_5, ref Scale scale_5, ref MoveObject moveshape5) =>
        {
            float3 pos_5 = trans_5.Value;
            float size_5 = scale_5.Value;
            Entities.WithAll<shape1Tag>().ForEach((Entity bEntity, ref Translation bTrans, ref Scale bs, ref MoveObject moveshape1) =>
            {
                float3 spawn = pos_5;
                float3 bpos = bTrans.Value;
                float bsize = bs.Value;
                if (math.lengthsq(pos_5 - bpos) < (size_5 + bsize) * (size_5 + bsize))
                {
                    PostUpdateCommands.DestroyEntity(entity_5);
                    for (int i = 0; i < 8; i++)
                    {
                        PostUpdateCommands.Instantiate(bEntity);
                        bpos = spawn;
                        moveshape1.velocity = new Vector3(UnityEngine.Random.Range(-15f, 15f) * 1.5f, UnityEngine.Random.Range(-15f, 15f) * 1.5f, 0);
                    }
                }
            });
            Entities.WithAll<shape4Tag>().ForEach((Entity bEntity4, ref Translation bTrans4, ref Scale bs4, ref MoveObject moveshape4) =>
            {
                float3 spawn = pos_5;
                float3 bpos4 = bTrans4.Value;
                float bsize4 = bs4.Value;
                if (math.lengthsq(pos_5 - bpos4) < (size_5 + bsize4) * (size_5 + bsize4))
                {
                    PostUpdateCommands.DestroyEntity(entity_5);
                    for (int i = 0; i < 8; i++)
                    {
                        PostUpdateCommands.Instantiate(bEntity4);
                        PostUpdateCommands.SetComponent(bEntity4, new Scale { Value = 0.5f });
                        PostUpdateCommands.SetComponent(bEntity4, new MoveObject { randomVel = new float3(UnityEngine.Random.Range(-15f, 15f) * 1.5f, UnityEngine.Random.Range(-15f, 15f) * 1.5f, 0) });
                    }
                }
                       // bpos4 = spawn;
                       // moveshape4.velocity4 = new Vector3(UnityEngine.Random.Range(-15f, 15f) * 1.5f, UnityEngine.Random.Range(-15f, 15f) * 1.5f, 0);
                    
                
            });


        });
 
    }
}
#endregion

#region Emissive Materials
public struct Particle : IComponentData { }


public partial class EmitSystem : SystemBase
{
    UnityEngine.ParticleSystem particleSystem;
    Transform particleSystemTransform;
    UnityEngine.ParticleSystem.EmitParams emitParams;

    float interval;
    float timer;

    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }

    public void Init(UnityEngine.ParticleSystem particleSystem)
    {
        this.particleSystem = particleSystem;
        particleSystemTransform = particleSystem.transform;
        interval = 1f / particleSystem.emission.rateOverTimeMultiplier;//this will make sure its a fixed value
        //that we can save in cache memory. To make it look better, you could do it in Update(), but we are
        //going for performance
        
        Enabled = true;//Can run the system
    }

    protected override void OnUpdate()
    {

        var deltaTime = (float)Time.DeltaTime;//time passed since last frame
        timer += deltaTime;

        var count = Mathf.RoundToInt(timer / interval);

        //this will move our particle system with every transform which has the 'Particle' tag
        if (count > 0)
        {
            timer -= count * interval;
            Entities.WithAll<Particle>().ForEach((in Translation translation) =>
            {
                particleSystemTransform.position = translation.Value;
                particleSystem.Emit(count);
            }).WithoutBurst().Run();
           
        }
    }
}

#endregion


#region Spawner Skeleton


//tag components - we will use these to distinguish entities from one another in our playing systems.

public struct playerTag : IComponentData { }
public struct shape1Tag : IComponentData { }
public struct shape2Tag : IComponentData { }
public struct shape3Tag : IComponentData { }
public struct shape4Tag : IComponentData { }
public struct shape5Tag : IComponentData { }
public struct shape6Tag : IComponentData { }


public class ECS_Spawner : MonoBehaviour
{
    EntityManager entityManager;

    PlayerSystem playerSystem;//referencing our playersystem

    
    //bool
    public bool hasPlayerSpawned;
    public bool shape1Spawn;
    public bool shape2Spawn;
    public bool growsphere;

    public float Beats { get; set; } = 0;

    //player button
    [Header("Button Objects")]
    public Button playerbutton;

    //player
    [Header("Player")]
    [SerializeField] private Mesh playermesh;//player mesh
    [SerializeField] private Material playermaterial;//player material
    [SerializeField] private GameObject playerspawn;//playerspawn
    Entity playerPrototype;

    public ParticleSystem particles;
   
    public int Count { get; set; } = 0;

  
   
    //shape1
    [Header("Shape 1")]
    [SerializeField] private Mesh shape1;//the mesh 
    [SerializeField] private Material shape1Material;//the material
    [SerializeField] private GameObject enemyspawn;

    public ParticleSystem particles2;
    Entity shape1Prototype;//so it can be accessed later

    public float Counter { get; set; } = 0;
    public float SCounter { get; set; } = 0;
    
    //shape2
    [Header("Shape 2")]
    [SerializeField] private Mesh shape2;//the mesh
    [SerializeField] private Material shape2Material;
    [SerializeField] private GameObject enemy2spawn;
    Entity shape2Prototype;

    //shape3
    [Header("Shape 3")]
    [SerializeField] private Mesh shape3;//the mesh
    [SerializeField] private Material shape3Material;
    [SerializeField] private GameObject enemy3spawn;
    Entity shape3Prototype;

    //shape 4
    [Header("Shape 4")]
    [SerializeField] private Mesh shape4;//the mesh
    [SerializeField] private Material shape4Material;//the material
    [SerializeField] private GameObject enemy4spawn;
    Entity shape4Prototype;

    [Header("Shape 5")]
    [SerializeField] private Mesh shape5;//the mesh
    [SerializeField] private Material shape5Material;//the material
    [SerializeField] private GameObject enemy5spawn;
    [SerializeField] private GameObject enemy5target;
    Entity shape5Prototype;

    [Header("Background")]
    [SerializeField] private Mesh wind1;// the mesh
    [SerializeField] private Material windMaterial;
    [SerializeField] private GameObject windspawner;
    Entity windPrototype;

    // Start is called before the first frame update
    void Start()
    {

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;//this will manage all of our entities

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EmitSystem>().Init(particles);
        //this will grab our particles and convert them into entity objects. We can use this 
        // to extend for other shapes to have a particle system following them.

        //skeleton code which will create all of our entities.
        var playerdesc = new RenderMeshDescription(
            playermesh,
            playermaterial
            );

        Entity playerentity = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            playerentity,
            entityManager,
            playerdesc);

        entityManager.AddComponentData(playerentity, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(playerentity, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(playerentity, new Scale { Value = 6.5f });
        entityManager.AddComponentData(playerentity, new MovePlayer());
        entityManager.AddComponentData(playerentity, new MoveObject());
        entityManager.AddComponentData(playerentity, new playerTag());
        entityManager.AddComponentData(playerentity, new Particle());
        playerPrototype = playerentity;

        var desc = new RenderMeshDescription(
            shape1,
            shape1Material);

        Entity entity = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            entity,
            entityManager,//
            desc);

        entityManager.AddComponentData(entity, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity, new Scale { Value = 8.0f });
        entityManager.AddComponentData(entity, new MoveObject());
        entityManager.AddComponentData(entity, new KillShape());
        entityManager.AddComponentData(entity, new shape1Tag());
 
        shape1Prototype = entity;

        var desc2 = new RenderMeshDescription(
            shape2,
            shape2Material);

        Entity entity2 = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            entity2,
            entityManager,
            desc2);

        entityManager.AddComponentData(entity2, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity2, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity2, new Scale { Value = 8.0f });
        entityManager.AddComponentData(entity2, new MoveObject());
        entityManager.AddComponentData(entity2, new KillShape());
        entityManager.AddComponentData(entity2, new shape2Tag());
        shape2Prototype = entity2;

        var desc3 = new RenderMeshDescription(
            shape3,
            shape3Material);

        Entity entity3 = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            entity3,
            entityManager,
            desc3);


        entityManager.AddComponentData(entity3, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity3, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity3, new Scale { Value = 8.0f });
        entityManager.AddComponentData(entity3, new MoveObject());
        entityManager.AddComponentData(entity3, new KillShape());
        entityManager.AddComponentData(entity3, new shape3Tag());
        shape3Prototype = entity3;

        var desc4 = new RenderMeshDescription(
            shape4,
            shape4Material);

        Entity entity4 = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            entity4,
            entityManager,
            desc4);

        entityManager.AddComponentData(entity4, new Translation { Value = new float3(0.0f,0.0f,0.0f) });
        entityManager.AddComponentData(entity4, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity4, new Scale { Value = 3.5f });
        entityManager.AddComponentData(entity4, new MoveObject());
        entityManager.AddComponentData(entity4, new shape4Tag());
        entityManager.AddComponentData(entity4, new KillShape());
        shape4Prototype = entity4;

        var desc5 = new RenderMeshDescription(
            shape5,
            shape5Material);

        Entity entity5 = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            entity5,
            entityManager,
            desc5);

        entityManager.AddComponentData(entity5, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity5, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity5, new Scale { Value = 10.0f });
        entityManager.AddComponentData(entity5, new MoveObject());
        entityManager.AddComponentData(entity5, new shape5Tag());
        entityManager.AddComponentData(entity5, new KillShape());
        shape5Prototype = entity5;

        var desc6 = new RenderMeshDescription(
            wind1,
            windMaterial);

        Entity entity6 = entityManager.CreateEntity(typeof(Prefab));
        RenderMeshUtility.AddComponents(
            entity6,
            entityManager,
            desc6);
        float3 scaler = new float3(90.0f, 2.0f, 0.0f);

        entityManager.AddComponentData(entity6, new Translation { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity6, new Rotation { Value = quaternion.identity });
        entityManager.AddComponentData(entity6, new ScalePivot { Value = new float3(65.0f,10.0f,0.0f)});
        entityManager.AddComponentData(entity6, new MoveObject());
        entityManager.AddComponentData(entity6, new shape6Tag());
        windPrototype = entity6;



        //geometry spawn booleans
        hasPlayerSpawned = false;
        shape1Spawn = false;
        shape2Spawn = false;
        growsphere = false;

        //Beats - this is a dynamic value which we can use so objects spawn in the same pattern as part
        //of a 'song'.
        Beats = 1f;

    }

    private void Update()
    {
        

        float counter = 0;
        
        Debug.Log(SCounter);
        //InvokeRepeating("Wind", 0f, Beats * 60);

        //if'1' is pressed, this will be called to spawn the first shape
        if (Input.GetKeyDown((KeyCode.Alpha0)))
        {
            CancelInvoke();//this will stop all coroutines, letting us reset the entities which are spawned
            Count = 0;//resetting the counter as we have already removed all of the entities which were present
        }
        else if(Input.GetKeyDown(KeyCode.Alpha1))
        {
           
            FirstSpawner();
            
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            counter = counter * Time.deltaTime;
            SecondSpawner();
            
         
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {

            ThirdSpawner();
        
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
        
            FourthSpawner();
        
        }

        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            growsphere = true;
            FifthSpawner();

        }
        else if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            WindSpawner();
        }


        if(growsphere)
        {
            SCounter++;
            
        }
       
        
    }
#endregion

#region Spawners
    
    

    public void FirstSpawner()
    {

        //This is our first spawner. It uses a set time interval to re-call the function. We could test how
        //different time intervals affect performance. This can instantiate up to 10,000 entities before frame-rate
        //completely drops.

        //1st test:
        //InvokeRepeating("Spawn", 0f, 0.5f);
        //2nd test:
        //InvokeRepeating("Spawn", 0f, 2.5f);
        //3rd test:
        //InvokeRepeating("Spawn", 0f, 3f);
        //4th test:
        //InvokeRepeating("Spawn", 0f, 5f);
        InvokeRepeating("Spawn", 0f, Beats);
    }

    public void SecondSpawner()
    {
        InvokeRepeating("Spawn2", 0f, Beats);//this will recall the spawner every 2 seconds
    }

    public void ThirdSpawner()
    {
        InvokeRepeating("Spawn3", 0f, Beats);//this will recall the spawner every 2 seconds
    }

    public void FourthSpawner()
    {
        InvokeRepeating("Spawn4", 0f, Beats * 3);
    }

    public void FifthSpawner()
    {

        InvokeRepeating("Spawn5", 0f, Beats * 6);
       
    }

    public void WindSpawner()
    {
        InvokeRepeating("Wind", 0f, Beats);
    }

    public void Wind()
    {
        Vector3 position = new Vector3(windspawner.transform.position.x, windspawner.transform.position.y, 0f);
        //Vector3 velocity = new Vector3(-85.5f, 0f, 0f);
        float speed = 1.1f;

        Entity windEntity = entityManager.Instantiate(windPrototype);
        entityManager.SetComponentData(windEntity, new Translation { Value = new float3(position.x, position.y + (UnityEngine.Random.Range(-100f, 100f)), 0) });
        entityManager.SetComponentData(windEntity, new Rotation { Value = Quaternion.Euler(90, 90, 0) });
        entityManager.SetComponentData(windEntity, new MoveObject { windVel = new float3(-95f * speed, 0f,0f) });
    }
    public void Spawn()
    {
        shape1Spawn = true;
        //This coroutine is called whenever we click the 'spawn shape 1' button
        Vector3 position = new Vector3(enemyspawn.transform.position.x + UnityEngine.Random.Range(-10f, 10f), enemyspawn.transform.position.y + UnityEngine.Random.Range(-10f, 10f), 0f);
        Vector3 velocity = new Vector3(UnityEngine.Random.Range(-250f, 250f), UnityEngine.Random.Range(-10f, 10f), 0f);

        float speed = 1.1f;//random speed
        for (int i = 0; i < 5; i++)
        {
            Entity newEntity = entityManager.Instantiate(shape1Prototype);//this creates the new entity whenever the button is pressed

            entityManager.SetComponentData(newEntity, new Translation { Value = new float3(position.x, position.y, position.z) });
            //entityManager.SetComponentData(newEntity, new Scale {  Value = Mathf.Lerp(UnityEngine.Random.Range(-.001f, 0.001f), UnityEngine.Random.Range(-.001f, 0.001f), UnityEngine.Random.Range(-.001f, 0.001f))});
            //the entities will move in a random velocity, move direction * speed = velocity of x,y (z is redundant)
            entityManager.SetComponentData(newEntity, new MoveObject { velocity = new float3(-12.5f * speed, UnityEngine.Random.Range(-10f, 6.5f) * speed, 0) });
    
            Count = Count + 1;
        }
    }

    public void Spawn2()
    {
        //This shape acts as a 'spawner' for further geometries
        shape2Spawn = true;
        Vector3 position = new Vector3(enemy2spawn.transform.position.x, enemy2spawn.transform.position.y, 0);
        float speed2 = 1.5f;


        Entity newEntity2 = entityManager.Instantiate(shape2Prototype);
        //Counter++;//have the timer update per second

        entityManager.SetComponentData(newEntity2, new Translation { Value = new float3(position.x, position.y + (UnityEngine.Random.Range(-100f, 100f)), 0) });
        entityManager.SetComponentData(newEntity2, new MoveObject { velocity2 = new float3(-20.5f * speed2, UnityEngine.Random.Range(-0.5f, 0.5f), 0) });
        entityManager.SetComponentData(newEntity2, new Rotation {Value = Quaternion.Euler(UnityEngine.Random.Range(-190f, 90f)* speed2,(UnityEngine.Random.Range(-90f, 90f))*speed2, 0)});

        /*    
            for(int i=0; i<8; i++)
            {
                //Debug.Log("got here now!");
                Entity newEntity = entityManager.Instantiate(shape1Prototype);
                entityManager.SetComponentData(newEntity, new Translation { Value = new float3(transform.position.x, transform.position.y, 0) });
                entityManager.SetComponentData(newEntity, new MoveObject { velocity = new float3(UnityEngine.Random.Range(-15f, 15f) * speed2, UnityEngine.Random.Range(-15f, 15f) * speed2, 0) });
                Count = Count + 1;
            }
        
        Counter = 0;//resetting the counter
         */
        Count = Count + 1;//updating the counter for each time the coroutine is called   
    }

    public void Spawn3()
    {
        Vector3 position = new Vector3(enemy3spawn.transform.position.x, enemy3spawn.transform.position.y, 0);
        float speed3 = 1.5f;
        Entity newEntity3 = entityManager.Instantiate(shape3Prototype);

        entityManager.SetComponentData(newEntity3, new Translation { Value = new float3(position.x + (UnityEngine.Random.Range(-200, 200)), position.y, 0) });
        entityManager.SetComponentData(newEntity3, new MoveObject{ velocity3 = new float3(UnityEngine.Random.Range(-20f,20f), 20.5f * speed3, 0) });
        Count = Count + 1;
        //Debug.Log("got here");
    }

    public void Spawn4()
    {
        Vector3 position = new Vector3(enemy4spawn.transform.position.x, enemy4spawn.transform.position.y, enemy4spawn.transform.position.z);
        float speed4 = 2.5f;

        Entity newEntity4 = entityManager.Instantiate(shape4Prototype);


        entityManager.SetComponentData(newEntity4, new Translation { Value = new Vector3(position.x, position.y, 0) });
        entityManager.SetComponentData(newEntity4, new MoveObject { velocity4 = 300.0f });
       
    
    }

    public void Spawn5()
    {
        float speed = 110.5f;
        Vector3 velocity = Vector3.zero; //intializing the float
        Entity newEntity5 = entityManager.Instantiate(shape5Prototype);
        Vector3 pos5 = new Vector3(enemy5spawn.transform.position.x, enemy5spawn.transform.position.y, 0);
        Vector3 target = new Vector3(enemy5target.transform.position.x, enemy5target.transform.position.y, 0);
        entityManager.SetComponentData(newEntity5, new Translation { Value = new float3(pos5.x, pos5.y, 0) });

        velocity = Vector3.MoveTowards(velocity, target, speed * Time.deltaTime);

        entityManager.SetComponentData(newEntity5, new MoveObject { velocity5 = velocity });
        Count = Count + 1;
        //Debug.Log(velocity + "   " );
    }

    public void SpawnPlayer()
    {
        Vector3 position = new Vector3(playerspawn.transform.position.x, playerspawn.transform.position.y, 0);
        Entity newPlayer = entityManager.Instantiate(playerPrototype);//this instantiates the player
      
        entityManager.SetComponentData(newPlayer, new Translation { Value = new float3(position.x, position.y, 0) });
        entityManager.SetComponentData(newPlayer, new MoveObject { });
        entityManager.SetComponentData(newPlayer, new MovePlayer { velocity = 720f });
        entityManager.SetComponentData(newPlayer, new Particle { });

        hasPlayerSpawned = true;
    }

    //public void Spawn1


}
#endregion