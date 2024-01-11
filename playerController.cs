using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    //variables
    [SerializeField]
    private float speed;
    [SerializeField]
    private float rotationSpeed;


    //screen and player boundaries
    private Vector2 boundaries;
    private float spriteWidth;
    private float spriteHeight;

    #region Game Loop
    // Start is called before the first frame update
    void Start()
    {
        boundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        spriteWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x ;//checking width of sprite
        spriteHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y ;//checking height of sprite
    }

    // Update is called once per frame - called first
    void Update()
    {
        moveSprite();
    }

    //Late Update is called after update, so the player can move, but only within screen boundaries.
    void LateUpdate()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x,boundaries.x + spriteWidth, boundaries.x * -1 - spriteWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, boundaries.y + spriteHeight, boundaries.y * -1 - spriteHeight);
        transform.position = viewPos;

    }
    #endregion
    #region Functions
    void moveSprite()
    {
        float horizontalInput = Input.GetAxis("Horizontal");//x
        float verticalInput = Input.GetAxis("Vertical");//y

        Vector2 mov = new Vector2(horizontalInput, verticalInput);
        float inputMagnitude = Mathf.Clamp01(mov.magnitude);
        mov.Normalize();

        //moving character relative to the move
        transform.Translate(mov * speed * inputMagnitude * Time.deltaTime, Space.World);

        

        if (mov != Vector2.zero)//if moving
        {
            Quaternion rotateP = Quaternion.LookRotation(Vector3.forward, mov);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateP, rotationSpeed * Time.deltaTime);
        }
    }

    #endregion
}