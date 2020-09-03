using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    [SerializeField]
    float WallJumpHeigth = 2;
    [SerializeField]
    float WallSlide = 2;

    private BoxCollider2D boxCollider;

    private Vector2 velocity;
    [SerializeField]
    private bool grounded;

    [SerializeField]
    private float G_mutiple = 0;




    [SerializeField]
  private  GameObject CheckWall_Left;
    [SerializeField]
  private  BoxCollider2D L_Wall;
    [SerializeField]
    private GameObject CheckWall_Rigth;
    [SerializeField]
    private BoxCollider2D R_Wall;
    [SerializeField]
    private bool IsWall;
    [SerializeField]
    private LayerMask WallLayer;

    private int Wall_LR=0;

    [SerializeField]
   private bool isdead=false;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        L_Wall = GetComponent<BoxCollider2D>();
        R_Wall = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        if (!isdead) { 
        float moveInput = Input.GetAxisRaw("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, walkAcceleration * Time.deltaTime);


        float acceleration = grounded ? walkAcceleration : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }



       
        if (!grounded&&IsWall && Input.GetButtonDown("Jump"))
        {
            velocity = new Vector2(speed*Wall_LR,jumpHeight* WallJumpHeigth);
        }
        // if (IsWall &&!grounded&&velocity.x>0 ||velocity.x<-3)
        // {
        //    velocity.y = Physics2D.gravity.y/ WallSlide * Time.deltaTime;
        // }
        //else velocity.y=Physics2D.gravity.y;

        bool left = (BoxCollider2D)Physics2D.OverlapBox(CheckWall_Left.transform.position,L_Wall.size , 0, WallLayer);
        bool rigth= (BoxCollider2D)Physics2D.OverlapBox(CheckWall_Rigth.transform.position, R_Wall.size, 0, WallLayer);
        IsWall = left|| rigth;


        if (left)
            Wall_LR = 1;
        if (rigth)
            Wall_LR = -1;

        grounded = false;
    

    Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
    foreach (Collider2D hit in hits)
    {
        if (hit == boxCollider)
            continue;
   
        ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
   
        if (colliderDistance.isOverlapped)
        {
            transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
            {
                grounded = true;
            }
        }
        
               
        }
    
    if (grounded)
        {
            velocity.y = 0;

            // Jumping code we implemented earlier—no changes were made here.
            if (Input.GetButton("Jump"))
            {
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                Debug.Log(velocity.y);
                
            }
    
        }
       else
        velocity.y += Physics2D.gravity.y*G_mutiple * Time.deltaTime;
        

        
        
    }

        }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            isdead = true;
        }

    }
}
