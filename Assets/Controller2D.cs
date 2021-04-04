using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {
    
    public LayerMask collisionMask;

    const float skinWidth = .025f;
    public Vector2Int rayCount = new Vector2Int(4, 4);
    
    float horizontalRaySpacing;
    float verticleRaySpacing;
    float maxClimbSlope = 70;
    float maxDescendAngle = 75;

    BoxCollider2D collisionCollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Start() {
        collisionCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = collisionCollider.bounds;
        bounds.Expand(skinWidth*-2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = collisionCollider.bounds;
        bounds.Expand(skinWidth*-2);

        rayCount.x = Mathf.Clamp(rayCount.x, 2, int.MaxValue);
        rayCount.y = Mathf.Clamp(rayCount.y, 2, int.MaxValue);

        horizontalRaySpacing    = bounds.size.y / (rayCount.x - 1);
        verticleRaySpacing      = bounds.size.x / (rayCount.y - 1);
    }

    public void Move(Vector2 velocity) {
        collisions.Reset();
        UpdateRaycastOrigins();

        if(velocity.y < 0) {
            DescendSlope(ref velocity);
        }
        if(velocity.x != 0) {
            HorizontalCollisions(ref velocity);
        }
        if(velocity.y != 0) {
            VerticleCollisions(ref velocity);
        }
        transform.position += new Vector3(velocity.x, velocity.y, 0);
    }

    void HorizontalCollisions(ref Vector2 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        for(int i = 0; i < rayCount.x; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up*(horizontalRaySpacing*i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right*directionX, rayLength, collisionMask);
        
            if(hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if(i == 0 && slopeAngle <= maxClimbSlope) {
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld) {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart*directionX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart*directionX;
                }

                if(!collisions.climbingSlope || slopeAngle > maxClimbSlope) {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    rayLength = hit.distance;

                    if(collisions.climbingSlope) {
                        velocity.y = Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }
    void VerticleCollisions(ref Vector2 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for(int i = 0; i < rayCount.y; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right*(verticleRaySpacing*i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up*directionY, rayLength, collisionMask);
        
            if(hit) {
                velocity.y = (hit.distance - skinWidth)*directionY;
                rayLength = hit.distance;

                if(collisions.climbingSlope) {
                    velocity.x = velocity.y/Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle*Mathf.Deg2Rad)*moveDistance;
        if(velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY; 
            velocity.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);
    
        if(hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
                if(Mathf.Sign(hit.normal.x) == directionX) {
                    if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
                        float moveDist = Mathf.Abs(velocity.x);
                        float decendVelocityY = Mathf.Sin(slopeAngle*Mathf.Deg2Rad)*moveDist;
                        velocity.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDist * Mathf.Sign(velocity.x);
                        velocity.y -= decendVelocityY;
                        collisions.slopeAngle = slopeAngle;
                        collisions.decendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    private struct RaycastOrigins {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below, left, right, climbingSlope, decendingSlope;
        public float slopeAngle, slopeAngleOld;
        public void Reset() {
            above = below = left = right = climbingSlope = decendingSlope = false;
            slopeAngle = slopeAngleOld = 0;
        }
    }
}