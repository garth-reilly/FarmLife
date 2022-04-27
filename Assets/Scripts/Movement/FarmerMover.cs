using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Movement
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (Animator))]
    public class FarmerMover : MonoBehaviour
    {
        // GR: Configuration variables
        [SerializeField] float moveSpeed = 6.0f;
        [SerializeField] float rotationSpeed = 720f;
        [SerializeField] float jumpHeight = 2.0f;
        [SerializeField] float gravity = -29.43f; // GR: 3 x Earth gravity
        [SerializeField] GameObject image;
        [SerializeField] float plantDistance = 4f;

        // GR: State variables
        float playerGravitationalVelocity;
        bool groundedPlayer;
        bool airborne = false;
        TileHandler tileHighlighted;
        Egg eggHiglighted;
        int eggCount = 0;
        Vector3 move = Vector3.zero;

        // GR: Referenced variables
        CharacterController characterController;
        Animator animator;
        Vector3 halfCharacterHeight;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            halfCharacterHeight = new Vector3(0f, characterController.height / 2f, 0f);
            Cursor.visible = false;
        }

        void Update()
        {
            if (Input.anyKey)
            {
                // GR: Use below if you always want WASD to move you in the SAME direction, not the direction the camera is facing
                // move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
                move = (Camera.main.transform.right * Input.GetAxis("Horizontal")) + (Camera.main.transform.forward * Input.GetAxis("Vertical"));
                move.y = 0f; // GR: Remove any vertical component

                if (move.sqrMagnitude > 1) // GR: Added to prevent going faster when strafing and moving forward / backward
                {
                    move.Normalize();
                }
                
                if (move != Vector3.zero) 
                {
                    // gameObject.transform.forward = move; // GR: Faces the player in the forward direction, but instantaneously, without turning.
                    Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                    move *= moveSpeed; // GR: Adds some scale of speed to the movement of the player.
                }

                if (groundedPlayer && Input.GetButton("Jump"))
                {
                    playerGravitationalVelocity = Mathf.Sqrt(-2f * jumpHeight * gravity);
                    airborne = true;
                    UpdateJumpAnimator(true);
                }
            }
            playerGravitationalVelocity += gravity * Time.deltaTime;
            move.y = playerGravitationalVelocity;
            characterController.Move(move * Time.deltaTime);
            UpdateMovementAnimator();
            groundedPlayer = characterController.isGrounded; // GR: It is advised to check if the character is grounded after calling the CharacterController.Move() method.
            if (groundedPlayer)
            {
                if (airborne)
                {
                    airborne = false;
                    UpdateJumpAnimator(false);
                }
                move.x = 0f;
                move.z = 0f;
                playerGravitationalVelocity = gravity;
            }        

            bool tileFound = false;
            RaycastHit[] hits = Physics.RaycastAll(GetCrosshairRay());
            foreach (RaycastHit hit in hits)
            {
                TileHandler tileHit = hit.transform.GetComponent<TileHandler>();
                if (tileHit != null)
                {
                    tileFound = true;
                    if (tileHighlighted != tileHit)
                    {
                        if (tileHighlighted != null)
                        {
                            tileHighlighted.Highlight(false);
                            tileHighlighted = null;
                            image.GetComponent<Image>().color = Color.white;
                        }
                        if (Vector3.Distance(tileHit.transform.position, transform.position) < plantDistance)
                        {
                            tileHighlighted = tileHit;
                            tileHighlighted.Highlight(true);
                            image.GetComponent<Image>().color = Color.red;
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(tileHit.transform.position, transform.position) > plantDistance)
                        {
                            tileHighlighted = tileHit;
                            tileHighlighted.Highlight(false);
                            image.GetComponent<Image>().color = Color.white;
                        }
                    }
                    break;
                }
            }
            // if (!tileFound)
            // {
            //     if (tileHighlighted != null)
            //     {
            //         tileHighlighted.Highlight(false);
            //         tileHighlighted = null;
            //         image.GetComponent<Image>().color = Color.white;
            //     }
            // }

            bool eggFound = false;
            hits = Physics.RaycastAll(GetCrosshairRay());            
            foreach (RaycastHit hit in hits)
            {
                Egg eggHit = hit.transform.GetComponent<Egg>();
                if (eggHit != null)
                {
                    eggFound = true;
                    if (eggHiglighted != eggHit)
                    {
                        if (eggHiglighted != null)
                        {
                            eggHiglighted = null;
                            eggHiglighted.GetComponent<MeshRenderer>().material.color = Color.white;
                            image.GetComponent<Image>().color = Color.white;
                        }
                        if (Vector3.Distance(eggHit.transform.position, transform.position) < plantDistance * 2f)
                        {
                            eggHiglighted = eggHit;
                            eggHiglighted.GetComponent<MeshRenderer>().material.color = Color.green;
                            image.GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            if (Vector3.Distance(eggHit.transform.position, transform.position) > plantDistance * 2f)
                            {
                                eggHiglighted = eggHit;
                                eggHiglighted.GetComponent<MeshRenderer>().material.color = Color.white;
                                image.GetComponent<Image>().color = Color.white;
                            }
                        }
                        break;                                               
                    }
                }
            }

            if (!tileFound && !eggFound)
            {
                if (tileHighlighted != null)
                {
                    tileHighlighted.Highlight(false);
                    tileHighlighted = null;
                }
                if (eggHiglighted != null)
                {
                    eggHiglighted.GetComponent<MeshRenderer>().material.color = Color.white;
                    eggHiglighted = null;
                    
                }
                image.GetComponent<Image>().color = Color.white;
            }

            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (tileHighlighted != null)
                    {
                        if (Vector3.Distance(tileHighlighted.transform.position, transform.position) < plantDistance)
                        {
                            tileHighlighted.PlantCarrot();
                        }
                    }
                }                                   
                if (Input.GetKeyDown(KeyCode.K))
                {
                    Health[] healthTargets = FindObjectsOfType<Health>();
                    foreach (Health healthTarget in healthTargets)
                    {
                        if (GetComponent<Health>() != healthTarget)
                        {
                            healthTarget.TakeDamage(20f);
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    Health[] healthTargets = FindObjectsOfType<Health>();
                    foreach (Health healthTarget in healthTargets)
                    {
                        if (GetComponent<Health>() != healthTarget)
                        {
                            if (Vector3.Distance(transform.position, healthTarget.transform.position) < 3f)
                            {
                                healthTarget.TakeDamage(20f);
                            }
                        }
                    }
                }                
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Application.Quit();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (eggHiglighted != null)
                    {
                        eggHiglighted.Eggsplode();
                        eggCount++;
                    }
                    if (eggCount >= 15f)
                    {
                        UpdateDanceAnimator();
                    }
                }
            }
        }

        private static Ray GetCrosshairRay()
        {
            Vector3 screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            return Camera.main.ScreenPointToRay(screenMiddle);
        }

        void UpdateMovementAnimator()
        {
            Vector3 velocity = move;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }

        void UpdateJumpAnimator(bool jumped)
        {
            animator.SetBool("jump", jumped);
        }

        void UpdateDanceAnimator()
        {
            animator.SetTrigger("dance");
        }
    }
}

