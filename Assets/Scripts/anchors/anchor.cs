using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Anchor : ScriptableObject
{
	enum anchorType
	{

	}
    string anchorName;
	Vector3 worldPosition;
	Vector3 worldRotation;

	float latitude;
	float longitude;

	string text;
	List<Anchor> anchorNeighbourList;
	Anchor startingAnchor;
	Anchor entranceAnchor;
}
