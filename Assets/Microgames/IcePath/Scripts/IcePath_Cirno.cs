﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePath_Cirno : MonoBehaviour {

    [HideInInspector] public GameObject icecream;
    [SerializeField] private int diff;

    private bool isHit = false;
    private bool hasWon = false;

    Animator anim;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip moveClip;
    public AudioClip victoryClip;

    [Header("Tilt settings")]
    [SerializeField] private float  tiltAngle = 10f;
    [SerializeField] private float  tiltSpeed = 100f;
    [SerializeField] private        Transform tiltPivot;

    private float tiltDirection;
    private float currentAngle;
    float getTiltAngleGoal() => tiltAngle * tiltDirection;

    // Cirno's position
    [HideInInspector]   public int      cirnoGridX, cirnoGridY; // Current  position in grid array
                        private Vector2 cirnoEndPos;            // Next     position in scene
    private float cirnoSpeed = 0f;

    // Get the grid map
    private string[,] _tile;

    private Vector2 origin;

    void Start () {

        origin = new Vector2(-5.5f, 4);

        // Assign privates
        _tile        = IcePath_GenerateMap.tile;

        // Set starting position
        transform.position = mapPos(cirnoGridX, -cirnoGridY);
        cirnoEndPos = transform.position;

        // Animation
        anim = GetComponentInChildren<Animator>();
        tiltPivot.eulerAngles = Vector3.forward * getTiltAngleGoal();
	}
	
	void Update () {
        // Is this the ice cream?
        if (_tile[cirnoGridX, cirnoGridY] == "B" &&
            transform.position == (Vector3)cirnoEndPos) {

            if (!hasWon) {
                Win();
                hasWon = true;
                tiltDirection = 0f;
            }

        }

        // Update angle
        currentAngle = Mathf.MoveTowards(currentAngle, getTiltAngleGoal(), tiltSpeed * Time.deltaTime);
        tiltPivot.eulerAngles = Vector3.forward * currentAngle;

        // Is this a Waka passing?
        int wakaIndex;

        if (int.TryParse(_tile[cirnoGridX, cirnoGridY], out wakaIndex)) {
            // Is Waka passing through?
            GameObject   waka        = IcePath_GenerateMap.wakaObject[wakaIndex];
            IcePath_Waka wakaScript  = waka.GetComponent<IcePath_Waka>();

            if (!wakaScript.isPassable) {
                if (!isHit) {
                    // Get hit

                    Die();
                    isHit = true;

                    MicrogameController.instance.playSFX(hitSound, volume: 0.75f,
                        panStereo: AudioHelper.getAudioPan(transform.position.x));

                    MicrogameController.instance.setVictory(victory: false, final: true);

                }
            }

        }

        // Has Cirno been hit?
        if (isHit) {
            // Lose condition - fly away now
            transform.position = transform.position + (new Vector3(-8, 8, 0) * Time.deltaTime);
            transform.Find("Spin Pivot").Find("Rig").Rotate(new Vector3(0, 0, 270 * Time.deltaTime));

        } else

        // Has Cirno won?
        if (hasWon) {

            /* oh. she has? alright */

        } else
        
        // Move on as usual
        {

            // Is Cirno locked into her current grid yet?
            if (((Vector2)transform.position - cirnoEndPos).magnitude > 0.25f) {
                MathHelper.moveTowards2D(transform, cirnoEndPos, 12f);

            } else
            // Lock her into place and check for the next movement
            {
                transform.position = cirnoEndPos;

                // Movement
                int moveX = (Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0)  - (Input.GetKeyDown(KeyCode.LeftArrow) ? 1 : 0);
                int moveY = (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0)     - (Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0);

                if (moveX != 0) {
                    moveY = 0;
                } else {
                    moveX = 0;
                }

                // Player is moving
                if (moveX != 0 ||
                    moveY != 0) {
                    // Valid movement?
                    if (canWalkInto(cirnoGridX + moveX, cirnoGridY - moveY)) {
                        cirnoGridX += moveX;
                        cirnoGridY -= moveY;


                        cirnoEndPos = mapPos(cirnoGridX, -cirnoGridY);

                        MicrogameController.instance.playSFX(moveClip,
                            pitchMult: Random.Range(.96f, 1.04f),
                            panStereo: AudioHelper.getAudioPan(transform.position.x),
                            volume: .5f);
                        tiltDirection = tiltDirection == 0f ? -1f : -tiltDirection;
                        if (moveX != 0 && (float)moveX != Mathf.Sign(tiltPivot.localScale.x))
                            tiltPivot.localScale = new Vector3(-tiltPivot.localScale.x, tiltPivot.localScale.y, tiltPivot.localScale.z);
                    }
                }

            }

        }

    }

    void Die() {
        // Explosion I guess
        Transform shadow = transform.Find("Shadow");
        Destroy(shadow.gameObject);

        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();
        particle.Play();

    }

    void Win() {
        // Animation I guess
        MicrogameController.instance.setVictory(victory: true, final: true);
        MicrogameController.instance.playSFX(victoryClip);

        anim.SetBool("hasWon", true);

    }

    bool canWalkInto(int posX, int posY) {
        // Can Cirno walk here?

        if (isWithin(posX, 0, 11) && // Is the position within the grid array?
            isWithin(posY, 0, 11)) {
            
            return (_tile[posX, posY] == "A" || // Is it the start isle?
                    _tile[posX, posY] == "B" || // Is it the end isle?
                    _tile[posX, posY] == "#" || // Is it an ice tile?
                    _tile[posX, posY] == "@" || // Is it an isle tile?
                    _tile[posX, posY] == "0" || // Is it a Waka passing?
                    _tile[posX, posY] == "1" ||
                    _tile[posX, posY] == "2" ||
                    _tile[posX, posY] == "3" ||
                    _tile[posX, posY] == "4" ||
                    _tile[posX, posY] == "5" ||
                    _tile[posX, posY] == "6" ||
                    _tile[posX, posY] == "7" ||
                    _tile[posX, posY] == "8");

        } else {
            return false;
        }

    }

    Vector2 mapPos(float posX, float posY) {
        return (new Vector2(-5.5f + posX, 4 + posY));
    }

    bool isWithin(float input, float min, float max) {
        // Is the value within this range?
        return (input >= min &&
                input <= max);
    }

}