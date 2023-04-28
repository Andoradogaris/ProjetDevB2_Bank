using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<GameObject> weaponList;
    private int weaponIndex;

    [Header("Camera")]
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private bool lockCursor;
    [SerializeField]
    [Range(0.1f, 10)] private float sensitivity;
    [SerializeField]
    private float maxUp;
    [SerializeField]
    private float maxDown;
    private float xRotation;

    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float carryingSpeed;
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool grounded;
    [SerializeField] private Transform orientation;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Booleans")]
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isCarrying;
    [SerializeField] private bool isHidden;
    private bool isDead;

    [Header("UI")]
    [SerializeField] private TMP_Text canTakeText;

    [Header("Health")]
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    [Header("Other")]
    [SerializeField] private GameObject goldBag;
    [SerializeField] private GameObject spawnOffset;
    [SerializeField] private Animator anim;

    private void Start()
    {
        canTakeText.enabled = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if(!isDead)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            xRotation -= Input.GetAxis("Mouse Y") * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -maxUp, maxDown);
            camera.localRotation = Quaternion.Euler(xRotation, 0, 0);

            // ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

            CheckInput();
            SpeedControl();

            // handle drag
            if (grounded)
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = 0;
            }

            if (isHidden)
            {
                for (int i = 0; i < weaponList.Count; i++)
                {
                    weaponList[i].SetActive(false);
                }
            }
            else
            {
                SwitchWeapon();
            }

            if(Input.GetKeyDown(KeyCode.F))
            {
                isHidden = !isHidden;
            }
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void CheckInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(KeyCode.LeftShift) && !isCarrying)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (!isHidden)
        {
            if (!isCarrying)
            {
                DetectGoldBags();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                weaponIndex--;
                ClampIndex();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                weaponIndex++;
                ClampIndex();
            }

            //Shoot
            if (Input.GetButton("Fire1"))
            {
                if (weaponIndex == 0 && weaponList[weaponIndex].GetComponent<Rifle>().canShoot)
                {
                    weaponList[weaponIndex].GetComponent<Rifle>().StartCoroutine("Shoot");
                }
                else if(weaponIndex == 1 && weaponList[weaponIndex].GetComponent<Pistol>().canShoot)
                {
                    weaponList[weaponIndex].GetComponent<Pistol>().StartCoroutine("Shoot");
                }
                
            }

            if (Input.GetKeyDown(KeyCode.R))
                if (weaponIndex == 0)
                {
                    weaponList[weaponIndex].GetComponent<Rifle>().StartCoroutine("Reload");
                }
                else if(weaponIndex == 1 && weaponList[weaponIndex].GetComponent<Pistol>().canShoot)
                {
                    weaponList[weaponIndex].GetComponent<Pistol>().StartCoroutine("Reload");
                }
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (isRunning)
        {
            moveSpeed = sprintSpeed;
        }
        else if(isCarrying)
        {
            moveSpeed = carryingSpeed;

            if (Input.GetKeyDown(KeyCode.G))
            {
                Instantiate(goldBag, spawnOffset.transform.position, spawnOffset.transform.rotation);
                isCarrying = false;
            }
        }
        else
        {
            moveSpeed = walkSpeed;
        }


        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void DetectGoldBags()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("GoldBag"))
            {
                canTakeText.enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    canTakeText.enabled = false;
                    isCarrying = true;
                    Destroy(hit.transform.gameObject);
                }
            }
            else
            {
                canTakeText.enabled = false;
            }
        }
    }

    private void ClampIndex()
    {
        if(weaponIndex < 0)
        {
            weaponIndex = 0;
        }
        else if(weaponIndex > 1)
        {
            weaponIndex = 1;
        }
    }

    private void SwitchWeapon()
    {
        if(weaponIndex == 0)
        {
            weaponList[weaponIndex].SetActive(true);
            weaponList[1].SetActive(false);
        }
        else
        {
            weaponList[0].SetActive(false);
            weaponList[weaponIndex].SetActive(true);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        ClampHealth();
    }

    private void ClampHealth()
    {
        if(health < 0)
        {
            health = 0;
            isDead = true;
        }
        else if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
}