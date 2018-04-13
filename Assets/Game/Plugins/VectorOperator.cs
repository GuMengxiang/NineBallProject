// this is a utility class that does some of the vector calculations for ball physics
// we will be changing a lot of these when we dig real deep into the physics

using UnityEngine;
using System.Collections;

public class VectorOperator
{
	public static float getRayDistance (Ray ray, Vector3 point)
	{
		return getRayNormal(ray, point).magnitude;
	}

	public static Vector3 getRayPoint (Ray ray, Vector3 point)
	{
		return ray.origin + Vector3.Project(point - ray.origin, ray.direction);
	}

	public static Vector3 getRayNormal (Ray ray, Vector3 point)
	{
		Vector3 origin = ray.origin;
		Vector3 direction = ray.direction;
		Vector3 delta = point - origin;
		Vector3 project = Vector3.Project(delta, direction);
		Vector3 normal = delta- project;
		return normal;
	}

    public static bool isPocketableAngle(Vector3 start, Vector3 mid, Vector3 end)
    {
        return Vector3.Dot((mid - start).normalized, (end - mid).normalized) > 0;
    }

    public static float getAngleFromVector(Vector3 inVector)
    {
        // only care about x and z, always assume y is zero

        float tan = inVector.z / inVector.x;
        float sin = inVector.z / (Mathf.Sqrt(inVector.z * inVector.z + inVector.x * inVector.x));

        float angle = Mathf.Abs(Mathf.Atan(tan));

        if (Mathf.Atan(tan) >= 0 && Mathf.Asin(sin) >= 0)
        {
            return angle * Mathf.Rad2Deg;
        }
        else if (Mathf.Atan(tan) <= 0 && Mathf.Asin(sin) >= 0)
        {
            return (- angle + Mathf.PI * 1.0f) * Mathf.Rad2Deg;
        }
        else if (Mathf.Atan(tan) >= 0 && Mathf.Asin(sin) <= 0)
        {
            return (angle + Mathf.PI * 1.0f) * Mathf.Rad2Deg;
        }
        else if (Mathf.Atan(tan) <= 0 && Mathf.Asin(sin) <= 0)
        {
            return (- angle + Mathf.PI * 2.0f) * Mathf.Rad2Deg;
        }
        else
        {
            return 0.0f;
        }
    }

    public static Vector3 getVectorFromAngle(float inAngle)
    {
        inAngle = Mathf.Abs(inAngle % 360); // make sure it is between 0 and 360

        if (inAngle <= 90)
        {
            return new Vector3(1.0f, 0.0f, Mathf.Tan(inAngle * Mathf.Deg2Rad));
        }
        else if (inAngle >= 90 && inAngle <= 180)
        {
            return new Vector3(-1.0f, 0.0f, - Mathf.Tan(inAngle * Mathf.Deg2Rad));
        }
        else if (inAngle >= 180 && inAngle <= 270)
        {
            return new Vector3(-1.0f, 0.0f, Mathf.Tan(inAngle * Mathf.Deg2Rad));
        }
        else if (inAngle >= 270 && inAngle <= 360)
        {
            return new Vector3(1.0f, 0.0f, Mathf.Tan(inAngle * Mathf.Deg2Rad));
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static Vector2 getLeftPerpendicular(Vector2 normal)
    {
        return new Vector2(-normal.y, normal.x);
    }

	public static Vector3 getPerpendicularXZ(Vector3 normal)
	{
		return new Vector3(-normal.z, 0.0f, normal.x);
	}

    public static Vector3 counterClockWisePerp(Vector3 inV)
    {
        return new Vector3(-inV.z, inV.y, inV.x);
    }

    public static Vector3 clockWisePerp(Vector3 inV)
    {
        return new Vector3(inV.z, inV.y, -inV.x);
    }

	public static Vector3 getProjectXZ (Vector3 currentVector, bool saveLength)
	{
		Vector3 projectXZ = new Vector3(currentVector.x, 0.0f, currentVector.z);

		return saveLength? currentVector.magnitude * projectXZ.normalized : projectXZ;
	}

	public static Vector3 getPerpendicularVectorVector (Vector3 current, Vector3 normal)
	{
		return current - Vector3.Project(current, normal);
	}

    public static Vector3 getCollisionSphereDirections(float radius, Vector3 currentVelocity, Vector3 currentPosition, LayerMask hitCheckMask, float maxDistance, RaycastHit hit)
	{
		Vector3 newDirection = Vector3.zero;
        Ray ray = new Ray(currentPosition/* - radius * currentVelocity.normalized*/, currentVelocity.normalized);

//		if(Physics.SphereCast(ray, radius, out hit, maxDistance, hitCheckMask)) // dajiang hack, WTF is this???
		{
			if(hit.collider.GetComponent<Rigidbody>())
			{
                Vector3 ourPernendicularTarget = (hit.collider.GetComponent<Rigidbody>().position - currentPosition).normalized;

                newDirection = getPerpendicularXZ(ourPernendicularTarget);

                if (Vector3.Dot(currentVelocity, newDirection) <= 0.0f)
                {
                    newDirection = new Vector3(-newDirection.x, newDirection.y, -newDirection.z);
                }
			}
			else
			{
                newDirection = currentVelocity - 2.0f * Vector3.Project(currentVelocity, -hit.normal);
			}

            string collisionName = LayerMask.LayerToName(hit.collider.gameObject.layer);

			if(collisionName == "Wall")
            {
                newDirection = getProjectXZ(newDirection, true);
            }
		}

        return newDirection;
	}

    public static Vector3 CleanYAxis(Vector3 inVec)
    {
        return new Vector3(inVec.x, 0.0f, inVec.z).normalized;
    }

    public static Vector3 getBallWallVelocity(float radius, Vector3 currentVelocity, Vector3 currentAngularVelocity, Vector3 currentPosition, LayerMask hitCheckMask, float maxDistance)
	{
		Vector3 newVelocity = Vector3.zero;
		
		Ray ray = new Ray(currentPosition, currentVelocity.normalized);
		RaycastHit hit;

		if(Physics.SphereCast(ray, radius, out hit, maxDistance, hitCheckMask))
		{
			newVelocity = currentVelocity - 2.0f * Vector3.Project(currentVelocity, -hit.normal);
			newVelocity = getProjectXZ(newVelocity , true);

            newVelocity += 0.1f * Vector3.Magnitude(currentAngularVelocity) * Vector3.Cross(-hit.normal, Vector3.Normalize(currentAngularVelocity));
		}

        return newVelocity;
    }

	public static void KeepInCube (Transform cube, float radius, Transform sphere)
	{
		if(!sphereInCube(sphere.position, radius, cube))
		{
			Vector3 _ballLocalPosition = VectorOperator.getLocalPosition(cube, sphere.position);
			float displacementX1 = _ballLocalPosition.x + radius - 0.5f*cube.localScale.x;
			float displacementX2 = _ballLocalPosition.x - radius + 0.5f*cube.localScale.x;

			if( displacementX1 > 0.0f )
			{
				sphere.position -= displacementX1*cube.right;
			}
			else
				if( displacementX2 < 0.0f )
			{
				sphere.position -= displacementX2*cube.right;
			}

			float displacementZ1 = _ballLocalPosition.z + radius - 0.5f*cube.localScale.z;
			float displacementZ2 = _ballLocalPosition.z - radius + 0.5f*cube.localScale.z;

			if( displacementZ1 > 0.0f )
			{
				sphere.position -= displacementZ1*cube.forward;
			}
			else
				if( displacementZ2 < 0.0f )
			{
				sphere.position -= displacementZ2*cube.forward;
			}
		}
	}

    public static bool FitPositionInCubeBounds(Transform inCube, Vector3 inPosition, float radius)
    {
        float rightBallX = inPosition.x + radius;
        float leftBallX = inPosition.x - radius;
        float topBallZ = inPosition.z + radius;
        float botBallZ = inPosition.z - radius;

        float rightCubeX = inCube.position.x + inCube.localScale.x * 0.5f;
        float leftCubeX = inCube.position.x - inCube.localScale.x * 0.5f;
        float topCubeZ = inCube.position.z + inCube.localScale.z * 0.5f;
        float botCubeZ = inCube.position.z - inCube.localScale.z * 0.5f;

        if (rightCubeX > rightBallX && leftCubeX < leftBallX && topCubeZ > topBallZ && botCubeZ < botBallZ)
        {
            return true;
        }

        return false;
    }

    public static Vector3 FitPositionInBounds(Vector3 inPosition)
    {
        return inPosition;
    }

	public static void MoveBallInQuad (Transform cube, float radius, Vector3 hitPoint, ref Vector3 position)
	{
		if(sphereInCube(hitPoint, radius, cube))
		{
			position = hitPoint;
		}
		else
		{
            Vector3 _ballMoveLocalPosition = VectorOperator.getLocalPosition(cube, hitPoint);
			
			if(!(Mathf.Abs(_ballMoveLocalPosition.x) <= -radius + 0.5f * cube.localScale.x) )
			{
				if(Mathf.Abs( _ballMoveLocalPosition.z ) <= -radius + 0.5f * cube.localScale.z)
				{
					Vector3 ballMovePositionX = _ballMoveLocalPosition.x > 0.0f? VectorOperator.getWordPosition(cube, (-radius + 0.5f*cube.localScale.x)*Vector3.right ): VectorOperator.getWordPosition(cube, (radius - 0.5f*cube.localScale.x)*Vector3.right);
					position = new Vector3(ballMovePositionX.x, position.y, hitPoint.z);
				}
			}
			else
			{
				if(Mathf.Abs( _ballMoveLocalPosition.x ) <= -radius + 0.5f*cube.localScale.x)
				{
					Vector3 ballMovePositionZ = _ballMoveLocalPosition.z > 0.0f? VectorOperator.getWordPosition(cube, (-radius + 0.5f*cube.localScale.z)*Vector3.forward ):
				    VectorOperator.getWordPosition(cube, (radius - 0.5f*cube.localScale.z)*Vector3.forward);
					position = new Vector3(hitPoint.x, position.y, ballMovePositionZ.z);
				}
			}
		}
	}

    // +- range
    public static float generateRandomNumber(float inRange)
    {
        float rd = Random.Range(-100, 100) / 100.0f;

        return rd * inRange;
    }

    // return a normalized vector
    public static Vector3 generateRandomXZVector()
    {
        Vector3 xz = new Vector3(Random.Range(-100, 100), 0.0f, Random.Range(-100, 100));

        return Vector3.Normalize(xz);
    }

    public static bool DetermineTrueIfSameVectorThree(Vector3 inPosOne, Vector3 inPosTwo)
    {
        if ((inPosOne - inPosTwo).magnitude < 0.001f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	public static Vector3 getLocalPosition(Transform parent, Vector3 wordPosition)
	{
		Vector3 localPosition = wordPosition - parent.position;
		return new Vector3( Vector3.Dot(localPosition, parent.right),
		                    Vector3.Dot(localPosition, parent.up),
		                    Vector3.Dot(localPosition, parent.forward)
		                   );
	}

	public static Vector3 getWordPosition(Transform parent, Vector3 localPosition)
	{
		return parent.position + localPosition.x*parent.right + localPosition.y*parent.up + localPosition.z*parent.forward;
	}

    public static bool sphereInCube(Vector3 point, float radius,  Transform cube)
	{
		Vector3 localPos = getLocalPosition(cube, point);
		Vector3 cubeSqale = cube.lossyScale;
		return Mathf.Abs(localPos.x) + radius <= 0.5f*cubeSqale.x && Mathf.Abs(localPos.y) + radius <= 0.5f*cubeSqale.y && Mathf.Abs(localPos.z) + radius <= 0.5f*cubeSqale.z;
	}
}
