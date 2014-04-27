using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Laser : MonoBehaviour {

	public Material LaserMaterial;
	public Material LaserPreviewMaterial;
	public float Width = 0.25f;
	bool bFiring;

	Vector3 StartingDragAxis;
	Vector3 DragInitialXAxis;
	bool bUserRotating;

	class LaserHitComparer : IComparer<RaycastHit>
	{
		public Vector3 SortDirection;

		public int Compare(RaycastHit a, RaycastHit b)
		{
			float da = Vector3.Dot(SortDirection, a.point);
			float db = Vector3.Dot(SortDirection, b.point);
			if (da > db)
				return 1;
			return -1;
		}
	}

	LaserHitComparer HitComparer = new LaserHitComparer ();

	struct LaserSegment
	{
		public Vector3 Origin;
		public Vector3 End;
		public LaserSegment(Vector3 org, Vector3 end)
		{
			Origin = org;
			End = end;
		}
	}

	const int MaxSegments = 128;
	LaserSegment[] DrawSegments = new LaserSegment[MaxSegments];
	int SegmentCount;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		SegmentCount = 0;
		bFiring = Input.GetMouseButton (0);
		TraceLaser ();
	}

	void TraceLaser() {
		Ray ray = new Ray(transform.position, transform.right);

		bool bContinue = true;
		while (bContinue) {
			bContinue = false;

			RaycastHit[] hits = Physics.RaycastAll(ray);

			HitComparer.SortDirection = ray.direction;
			Array.Sort(hits, HitComparer);

			bool bLaserDrawn = false;
			bool bStopHits = false;
			foreach (RaycastHit hit in hits) {
				var dmgCmp = hit.collider.gameObject.GetComponent<InteractsWithLaser>();
				if (null == dmgCmp)
					continue;

				var result = dmgCmp.OnLaserHit(this, !bFiring);

				switch (result.ResponseType) {
					case ELaserResponse.Continue:
						break;
					case ELaserResponse.Stop:
						AddLaserSegment(ray.origin, hit.point);
						bLaserDrawn = true;
						bStopHits = true;
						break;
					case ELaserResponse.Bounce:
						AddLaserSegment(ray.origin, hit.point);
						bLaserDrawn = true;
						ray.origin = hit.point;
						Vector3 norm = hit.normal;
						ray.direction = Vector3.Reflect(ray.direction, norm);
						bStopHits = true;
						bContinue = true;
						break;
				}

				if (bStopHits)
					break;
			}

			if (!bLaserDrawn)
				AddLaserSegment(ray.origin, ray.origin + ray.direction * 1000);
		}
	}

	void AddLaserSegment(Vector3 origin, Vector3 end) {
		if (SegmentCount == MaxSegments)
			return;
		DrawSegments [SegmentCount++] = new LaserSegment (origin, end);
	}

	void OnGUI()
	{
		for (int s = 0; s < SegmentCount; s++) {
			LaserSegment segment = DrawSegments[s];
			DrawLaser(segment.Origin, segment.End);
		}
	}

	void DrawLaser(Vector3 origin, Vector3 end)
	{
		Material mat = bFiring ? LaserMaterial : LaserPreviewMaterial;
		if (null == mat) {
			Debug.LogError ("You forgot to set a laser material.");
			return;
		}

		mat.SetPass (0);
		
		Vector3 dir = (end - origin).normalized;
		Vector3 right = 0.5f * Width * new Vector3 (dir.z, -dir.y, -dir.x).normalized;

		GL.Begin (GL.QUADS);
		GL.Color (Color.red);

		GL.Vertex (origin + right);
		GL.Vertex (origin - right);
		GL.Vertex (end - right);
		GL.Vertex (end + right);

		GL.End ();
	}

	void OnMouseDrag(Vector3 dragOffset)
	{
		if (bUserRotating)
		{
			Vector3 currentAxis = dragOffset.normalized;
			//float angle = Vector3.Angle(StartingDragAxis, currentAxis);
			Vector3 startingDragPerp = new Vector3(-StartingDragAxis.y, StartingDragAxis.x, 0);
			Vector2 dragVector = new Vector2(Vector3.Dot(currentAxis, StartingDragAxis), Vector3.Dot(currentAxis, startingDragPerp)).normalized;
			float angle = (float)(Math.Atan2(dragVector.y, dragVector.x) * 180 / Math.PI);

			Quaternion rotation = Quaternion.AngleAxis(-angle, new Vector3(0,1,0));
			Matrix4x4 originalToCurrent = Matrix4x4.TRS(new Vector3(), rotation, new Vector3(1, 1, 1));

			Vector3 newRight = originalToCurrent.MultiplyVector(DragInitialXAxis);
			newRight.y = 0;
			newRight.Normalize();
			Vector3 newForward = new Vector3(-newRight.z, 0, newRight.x);
			transform.right = newRight;
			transform.forward = newForward;
			//transform.up = new Vector3(0, 1, 0);
		}
		else
		{
			bUserRotating = true;
			StartingDragAxis = dragOffset.normalized;
			DragInitialXAxis = transform.right;
		}
	}

	void OnMouseUp(Vector3 mouseScreenPos)
	{
		bUserRotating = false;
	}
}
