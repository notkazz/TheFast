using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement : MonoBehaviour
{ 

    ///body variables
    [SerializeField] private float speed;
    private Rigidbody2D body;

    //jump variables
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groudLayer;
    bool isJumping;
    
    ///dashing variables
    [SerializeField] private float dashVelocity = 14f;
    [SerializeField] private float dashingTime = 0.5f;
    private Vector2 dashingDirection;
    private bool isDashing;
    private bool canDash = false;

    private Vector2 eventDirection;

    public float cooldownTime= 20;
    private float nextDashTime= 0;
    
    ///dash trail
    private TrailRenderer trailRenderer;

    //Animator
    public Animator animator;

    //make player face movement
    [HideInInspector]
    public bool isFacingLeft;
    
    public bool spawnFacingLeft;
    private Vector2 facingLeft;

    //sounds
    public Player_Audio playSound;

    // bounce

    ///Start
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        playSound = GetComponent<Player_Audio>();
        //make player face movement
        facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);
        if(spawnFacingLeft)
        {
            transform.localScale = facingLeft;
            isFacingLeft = true;
        }
    }
    
    ///update player movement
    private void Update()
    {
        Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groudLayer);
        
        resetPlayer();
        walk(dir);
        jump();
        if (Time.time > nextDashTime){
            dash();
        }
        
        bounce();
        
    }

    ///walking code
    private void walk(Vector2 dir)
    {
        animator.SetFloat("Speed", Mathf.Abs(dir.x * speed));
        body.velocity = (new Vector2(dir.x * speed, body.velocity.y));

        if (body.velocity.x > 0 && isFacingLeft)
        {
            isFacingLeft = false;
            Flip();
        }
        if (body.velocity.x < 0 && !isFacingLeft)
        {
            isFacingLeft = true;
            Flip();
        }
    }

    ///jumping code
    private void jump()
    {   
        
        if ((Input.GetKey(KeyCode.UpArrow)) && (isGrounded == true))
            body.velocity = new Vector2(body.velocity.x,speed);
            animator.SetBool("isJumping", false);
            

        if (body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (body.velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow))
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (isGrounded == false && (Input.GetKey(KeyCode.UpArrow)))
        {
            onJumping();
        }
    }

    public void onJumping()
    {
        animator.SetBool("isJumping", true);
        if (isDashing == false)
        {
            //playSound.Play(1);
        }
    }

    ///dashing code
    private void dash()
    {
        var dashInput = Input.GetButtonDown("Dash");
        if (dashInput && canDash && (Time.time > nextDashTime))
        {
            isDashing = true;

            nextDashTime = Time.time + cooldownTime;
            playSound.Play(0);
            canDash = false;
            trailRenderer.emitting = true;
            dashingDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashingDirection == Vector2.zero)
            {
                dashingDirection = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
            StartCoroutine(cooldownTime2());
        }

        if (isDashing)
        {
            body.velocity = dashingDirection.normalized * dashVelocity;
            return;
        }

        if (isDashing == false)
        {
            canDash = true;
        }
    }

    private void bounce(){
        if(isDashing)
        {
            if(isGrounded){
            }
            //if hits a wall or ground
            
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    
    private IEnumerator cooldownTime2()
    {
        yield return new WaitForSeconds(cooldownTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    protected virtual void Flip()
    {
        if (isFacingLeft)
        {
            transform.localScale = facingLeft;
        }
        if (!isFacingLeft)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    void resetPlayer()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void onTriggerEnter2d(Collider2D other)
    {
        Debug.Log("hit registered");

    }

}