using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Assets.Code
{

    class VelocityMotionController : RaycastController, IMotionController
    {

        public float maxSlopeAngle = 80;

        public VelocityCollisionInfo collisions;
        [HideInInspector]
        public Vector2 playerInput;

        public LayerMask SolidLayer => solidLayer;
        public LayerMask JumpThroughLayer => oneWayLayer;
        public LayerMask AllLayer => solidLayer | oneWayLayer;

        public RaycastOrigins raycastOrigins;

        protected override void Start()
        {
            base.Start();
            collisions.faceDir = 1;
        }

        public void UpdateRaycastOrigins()
        {
            Bounds bounds = boxCollider.bounds;
            bounds.Expand(skinWidth * -2);

            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            Move(velocity, false);
            return new CollisionInfo()
            {
                Below = collisions.below && !collisions.slidingDownMaxSlope,
                Above = collisions.above,
                Left = collisions.left,
                Right = collisions.right,
            };
        }

        public CollisionInfo CheckMove(Vector3 velocity, Vector3 down, Vector3 position)
        {
            return new CollisionInfo();
        }

        public bool OnJumpThrough(Vector3 down)
        {
            return false;
        }

        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;
            playerInput = input;

            var bounds = boxCollider.bounds;
            bounds.Expand(-skinWidth * 2);
            collisions.overlapping = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0, solidLayer)
                .Where(c => c.gameObject.GetComponent<PlatformEffector2D>() != null && c.gameObject.GetComponent<PlatformEffector2D>().useOneWay)
                .ToArray();

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            if (moveAmount.x != 0)
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref moveAmount);
            if (moveAmount.y != 0)
            {
                VerticalCollisions(ref moveAmount);
            }

            transform.Translate(moveAmount);

            if (standingOnPlatform)
            {
                collisions.below = true;
            }
        }

        void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            if (Mathf.Abs(moveAmount.x) < skinWidth)
            {
                rayLength = 2 * skinWidth;
            }

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, solidLayer);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit)
                {

                    if (collisions.overlapping.Contains(hit.collider))
                    {
                        continue;
                    }
                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i == 0 && slopeAngle <= maxSlopeAngle)
                    {
                        if (collisions.descendingSlope)
                        {
                            collisions.descendingSlope = false;
                            moveAmount = collisions.moveAmountOld;
                        }
                        float distanceToSlopeStart = 0;
                        if (slopeAngle != collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - skinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        collisions.left = directionX == -1;
                        collisions.right = directionX == 1;
                    }
                }
            }
        }

        void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {

                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, solidLayer);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    if (hit.collider.tag == "Through")
                    {
                        if (directionY == 1 || hit.distance == 0)
                        {
                            continue;
                        }
                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }
                        if (playerInput.y == -1)
                        {
                            collisions.fallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", .5f);
                            continue;
                        }
                    }

                    moveAmount.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }

            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, solidLayer);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != collisions.slopeAngle)
                    {
                        moveAmount.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }

        void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbmoveAmountY)
            {
                moveAmount.y = climbmoveAmountY;
                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                collisions.below = true;
                collisions.climbingSlope = true;
                collisions.slopeAngle = slopeAngle;
                collisions.slopeNormal = slopeNormal;
            }
        }

        void DescendSlope(ref Vector2 moveAmount)
        {

            RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, solidLayer);
            RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, solidLayer);
            if (maxSlopeHitLeft ^ maxSlopeHitRight)
            {
                SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
                SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
            }

            if (!collisions.slidingDownMaxSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, solidLayer);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                            {
                                float moveDistance = Mathf.Abs(moveAmount.x);
                                float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                                moveAmount.y -= descendmoveAmountY;

                                collisions.slopeAngle = slopeAngle;
                                collisions.descendingSlope = true;
                                collisions.below = true;
                                collisions.slopeNormal = hit.normal;
                            }
                        }
                    }
                }
            }
        }

        void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        {

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    collisions.slopeAngle = slopeAngle;
                    collisions.slidingDownMaxSlope = true;
                    collisions.slopeNormal = hit.normal;
                }
            }

        }

        void ResetFallingThroughPlatform()
        {
            collisions.fallingThroughPlatform = false;
        }

        public struct VelocityCollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public bool climbingSlope;
            public bool descendingSlope;
            public bool slidingDownMaxSlope;

            public float slopeAngle, slopeAngleOld;
            public Vector2 slopeNormal;
            public Vector2 moveAmountOld;
            public int faceDir;
            public bool fallingThroughPlatform;
            public Collider2D[] overlapping;

            public void Reset()
            {
                overlapping = new Collider2D[0];
                above = below = false;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;
                slidingDownMaxSlope = false;
                slopeNormal = Vector2.zero;

                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
            }
        }

        public struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

    }

}