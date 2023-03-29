using DungeonDraws.Game;
using DungeonDraws.Level;
using UnityEngine;

namespace RTS_Cam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RtsCamera : MonoBehaviour
    {

        #region Foldouts

#if UNITY_EDITOR

        public int lastTab = 0;

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion

        private Transform _mTransform; //camera tranform
        public bool useFixedUpdate; //use FixedUpdate() or Update()

        #region Movement

        public float keyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;

        #endregion

        #region Height

        public bool initWithOriginHeight = true;//true means camera will have it init view,otherwise will move to minHeight's associate pos.
        bool _originHeightSetted;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        public float heightDampening = 5f;
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float _zoomPos; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public bool limitMap = true;
        public float limitXMin = -50f; //x limit of map
        public float limitXMax = 50f;
        public float limitYMin = -50f; //z limit of map
        public float limitYMax = 50f;

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                if (!zoomIn && zoomOut)
                    return 1;
                if (zoomIn)
                    return -1;
                return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if (rotateLeft && rotateRight)
                    return 0;
                if (rotateLeft)
                    return -1;
                if (rotateRight)
                    return 1;
                return 0;
            }
        }
        

        #endregion

        
        private void Awake()
        {
            GameStatusHandler.Instance.OnWorldBuilt += (_, _) => { MoveCameraToMap(); };
        }
        private void MoveCameraToMap()
        {
            Debug.Log(LevelManager.Instance.GetCameraMapGridLocation());
            Vector4 bounds = LevelManager.Instance.GetBounds();
            limitXMin = bounds.x;
            limitXMax = bounds.z;
            limitYMin = bounds.y;
            limitYMax = bounds.w;
            if (Camera.main != null) Camera.main.transform.Translate(LevelManager.Instance.GetCameraMapGridLocation());
        }
        #region Unity_Methods

        private void Start()
        {
            _mTransform = transform;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            if (FollowingTarget)
                FollowTarget();
            else
                Move();

            HeightCalculation();
            Rotation();
            LimitPosition();
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = _mTransform.InverseTransformDirection(desiredMove);

                _mTransform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = _mTransform.InverseTransformDirection(desiredMove);

                _mTransform.Translate(desiredMove, Space.Self);
            }

            if (usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = _mTransform.InverseTransformDirection(desiredMove);

                _mTransform.Translate(desiredMove, Space.Self);
            }

            //旋转视角
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(0))
            {
                float xAngle = _mTransform.localEulerAngles.x + MouseAxis.y;
                if (xAngle > 90f)
                {
                    xAngle = 90f;
                }
                if (xAngle < 45f)
                {
                    xAngle = 45f;
                }

                var localEulerAngles = _mTransform.localEulerAngles;
                localEulerAngles = new Vector3(xAngle, localEulerAngles.y, localEulerAngles.z);
                _mTransform.localEulerAngles = localEulerAngles;
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();

            if (initWithOriginHeight && _originHeightSetted == false)
            {
                if (distanceToGround > maxHeight)
                {
                    distanceToGround = maxHeight;
                }
                if (distanceToGround < minHeight)
                {
                    distanceToGround = minHeight;
                }
                _zoomPos = (distanceToGround - minHeight) / (maxHeight - minHeight);
                //Debug.Log("distanceToGround:" + distanceToGround + ", zoomPos:" + zoomPos);
                _originHeightSetted = true;
                return;
            }


            if (useScrollwheelZooming)
                _zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                _zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

            _zoomPos = Mathf.Clamp01(_zoomPos);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, _zoomPos);
            float difference = 0;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;


            var position = _mTransform.position;
            position = Vector3.Lerp(position,
                new Vector3(position.x, targetHeight + difference, position.z), Time.deltaTime * heightDampening);
            _mTransform.position = position;
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void Rotation()
        {
            if (useKeyboardRotation)
                transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * rotationSped, Space.World);

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
                _mTransform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World);
        }

        /// <summary>
        /// follow target if target != null
        /// 这种FollowTarget实质为:设置相机x,z坐标改成目标坐标，同时附加设置targetOffset。
        /// 这种效果不是太理想，特别是相机处于旋转状态下，设置FollowTarget后，相机的视野中心并不是目标物体。
        /// 较理想的是根据目标物体位置和当前相机信息反算出相机的目标位置
        /// </summary>
        // private void FollowTarget()
        //{
        //    Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
        //    m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        //}
        private void FollowTarget()
        { 
            var hitPos = GetZRaycastHitGroundPos();
            var position = targetFollow.position;
            hitPos.y = position.y;

            Vector3 moveVec = position - hitPos;
            var position1 = _mTransform.position;
            Vector3 targetPos = position1 + moveVec;
            Vector3.Distance(targetPos, position1);
            //Debug.Log("dis:" + dis);
            position1 = Vector3.MoveTowards(position1, targetPos, Time.deltaTime * followingSpeed);
            _mTransform.position = position1;
        }
 
        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;

            var position = _mTransform.position;
            position = new Vector3(Mathf.Clamp(position.x, limitXMin, limitXMax),
                position.y,
                Mathf.Clamp(position.z, limitYMin, limitYMax));
            _mTransform.position = position;
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(_mTransform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, groundMask.value))
                return (hit.point - _mTransform.position).magnitude;


            return 0f;
        }



        /// <summary>
        /// Get obj's z direction collider with ground's pos
        /// </summary>
        /// <returns></returns>
        private Vector3 GetZRaycastHitGroundPos()
        {
            Ray ray = new Ray(_mTransform.position, _mTransform.forward);
            RaycastHit hit;
            //Debug.Log("groundMask .value" + groundMask.value + " ,xxx:" + (1 << 6));

            if (Physics.Raycast(ray, out hit, 100, groundMask.value))
                return hit.point;

            return Vector3.zero;
        }

        #endregion

        #region Inspector Ext
        [ContextMenu("CurHeight")]
        void DoLogHeight()
        {
            Debug.Log("height:" + DistanceToGround());
        }
        #endregion
    }
}