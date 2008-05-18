using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{

    
    /// <summary>
    /// A very basic camera class
    /// </summary>
    public class Camera:IUpdateable
    {
        #region Matrix and Vector fields
        protected Matrix m_viewMatrix;
        protected Matrix m_projectionMatrix;

        protected Vector3 m_positionVector = new Vector3(0, 0, 0);
        protected Vector3 m_upVector = new Vector3(0, 1, 0);
        protected Vector3 m_rightVector;
        protected Vector3 m_velocity = new Vector3(0, 0, 0);
        protected Vector3 m_lookVector = new Vector3(0.0f, 0.0f, 1.0f);
        #endregion

        #region Single value fields
        //protected Plane[] m_frustum;

        protected float m_speed = 0;
        protected float m_strafeSpeed = 0;
        protected float m_upSpeed = 0;
        protected float m_rollSpeed = 0;
        protected float m_yaw = 0;
        protected float m_pitch = 0;
        protected float m_maxPitch = Geometry.DegreeToRadian(89.0f);
        protected float m_maxSpeed = 10;

        protected float m_fov;
        protected float m_aspect;
        protected float m_nearPlane;
        protected float m_farPlane;

        protected bool m_enableYMovement = true;
        protected bool m_invertY = false;
        #endregion

        /// <summary>Creates a new Camera</summary>
        public Camera()
        {
            //m_frustum = new Plane[6];
            CreateProjectionMatrix((float)Math.PI / 3.0f, 1.3f, 1.0f, 1000.0f);
            Update(0);
        }
        public Camera(float fov, float aspect, float zNear, float zFar)
        {
            //m_frustum = new Plane[6];
            CreateProjectionMatrix(fov, aspect, zNear, zFar);
            Update(0);
        }

        /// <summary>Creates the projection matrix.</summary>
        /// <param name="fov">Field of view</param>
        /// <param name="aspect">Aspect ratio</param>
        /// <param name="near">Near plane</param>
        /// <param name="far">Far plane</param>
        protected virtual void CreateProjectionMatrix(float fov, float aspect, float near, float far)
        {
            m_fov = fov;
            m_aspect = aspect;
            m_nearPlane = near;
            m_farPlane = far;
            m_projectionMatrix = Matrix.PerspectiveFovRH(m_fov, m_aspect, m_nearPlane, m_farPlane);
        }

        /// <summary>Moves the camera forward and backward.</summary>
        /// <param name="units">Amount to move</param>
        public void MoveForward(float units)
        {
            if (m_enableYMovement)
            {
                m_positionVector += m_lookVector * units;
            }
            else
            {
                Vector3 moveVector = new Vector3(m_lookVector.X, 0.0f, m_lookVector.Z);
                moveVector.Normalize();
                moveVector *= units;
                m_positionVector += moveVector;
            }
        }
        /// <summary>Increases speed</summary>
        /// <param name="units">Amount to accelerate,use negative value to  decelerate</param>
        public void Accelerate(float units)
        {
            m_speed += units;
            if (m_speed > m_maxSpeed)
                m_speed = m_maxSpeed;
            if (m_speed < -m_maxSpeed)
                m_speed = -m_maxSpeed;
        }


        /// <summary>Moves the camera left and right.</summary>
        /// <param name="units">Amount to move</param>
        public void StrafeSpeed(float speed)
        {
            m_strafeSpeed = speed;
        }

        /// <summary>Moves the camera up and down.</summary>
        /// <param name="units">Amount to move.</param>
        public void UpSpeed(float speed)
        {
            m_upSpeed = speed;
        }
        /// <summary>
        /// Sets the camera's roilling speed
        /// </summary>
        /// <param name="speed">speed at which to roll</param>
        public void RollSpeed(float speed)
        {
            m_rollSpeed = speed;
        }

        /// <summary>Yaw the camera around its Y-axis.</summary>
        /// <param name="radians">Radians to yaw.</param>
        public void Yaw(float radians)
        {
            if (radians == 0.0f)
            {
                // Don't bother
                return;
            }
            Matrix rotation = Matrix.RotationAxis(m_upVector, radians);
            m_rightVector = Vector3.TransformNormal(m_rightVector, rotation);
            m_lookVector = Vector3.TransformNormal(m_lookVector, rotation);
        }

        /// <summary>Pitch the camera around its X-axis.</summary>
        /// <param name="radians">Radians to pitch.</param>
        public void Pitch(float radians)
        {
            if (radians == 0.0f)
            {
                // Don't bother
                return;
            }

            radians = (m_invertY) ? -radians : radians;
            m_pitch -= radians;

            if (m_pitch > m_maxPitch)
            {
                radians += m_pitch - m_maxPitch;
            }
            else if (m_pitch < -m_maxPitch)
            {
                radians += m_pitch + m_maxPitch;
            }

            Matrix rotation = Matrix.RotationAxis(m_rightVector, radians);
            m_upVector = Vector3.TransformNormal(m_upVector, rotation);
            m_lookVector = Vector3.TransformNormal(m_lookVector, rotation);
        }

        /// <summary>Roll the camera around its Z-axis.</summary>
        /// <param name="radians">Radians to roll.</param>
        public void Roll(float radians)
        {
            if (radians == 0.0f)
            {
                // Don't bother
                return;
            }
            Matrix rotation = Matrix.RotationAxis(m_lookVector, radians);
            m_rightVector = Vector3.TransformNormal(m_rightVector, rotation);
            m_upVector = Vector3.TransformNormal(m_upVector, rotation);
        }

        public virtual void UpdateMatrix()
        {
            // Calculate the new view matrix
            m_viewMatrix = Matrix.LookAtRH(m_positionVector, m_positionVector + m_lookVector, m_upVector);

            // Calculate new view frustum
            //BuildViewFrustum();

            // Set the camera axes from the view matrix
            m_rightVector.X = m_viewMatrix.M11;
            m_rightVector.Y = m_viewMatrix.M21;
            m_rightVector.Z = m_viewMatrix.M31;
            m_upVector.X = m_viewMatrix.M12;
            m_upVector.Y = m_viewMatrix.M22;
            m_upVector.Z = m_viewMatrix.M32;
            m_lookVector.X = -m_viewMatrix.M13;
            m_lookVector.Y = -m_viewMatrix.M23;
            m_lookVector.Z = -m_viewMatrix.M33;

            // Calculate yaw and pitch
            float lookLengthOnXZ = (float)Math.Sqrt(m_lookVector.Z * m_lookVector.Z + m_lookVector.X * m_lookVector.X);
            m_pitch = (float)Math.Atan2(m_lookVector.Y, lookLengthOnXZ);
            m_yaw = (float)Math.Atan2(m_lookVector.X, m_lookVector.Z);
        }
        /// <summary>Updates the camera and creates a new view matrix.</summary>
        public virtual bool Update(float timeElapsed)
        {
            // Cap velocity to max velocity
            if (m_speed > m_maxSpeed)
                m_speed = m_maxSpeed;
            if (m_speed < -m_maxSpeed)
                m_speed = -m_maxSpeed;
            Roll(m_rollSpeed * timeElapsed);

            // Move the camera
            m_positionVector += m_lookVector * m_speed * timeElapsed +
                m_rightVector * m_strafeSpeed * timeElapsed + m_upVector * m_upSpeed * timeElapsed;
            UpdateMatrix();

            return true;
        }

        #region Querying and setting camera's parameters
        ///// <summary>
        ///// Build the view frustum planes using the current view/projection matrices
        ///// </summary>
        //public void BuildViewFrustum()
        //{
        //    Matrix viewProjection = m_view * m_projection;

        //    // Left plane
        //    m_frustum[0].A = viewProjection.M14 + viewProjection.M11;
        //    m_frustum[0].B = viewProjection.M24 + viewProjection.M21;
        //    m_frustum[0].C = viewProjection.M34 + viewProjection.M31;
        //    m_frustum[0].D = viewProjection.M44 + viewProjection.M41;

        //    // Right plane
        //    m_frustum[1].A = viewProjection.M14 - viewProjection.M11;   
        //    m_frustum[1].B = viewProjection.M24 - viewProjection.M21;
        //    m_frustum[1].C = viewProjection.M34 - viewProjection.M31;
        //    m_frustum[1].D = viewProjection.M44 - viewProjection.M41;

        //    // Top plane
        //    m_frustum[2].A = viewProjection.M14 - viewProjection.M12;   
        //    m_frustum[2].B = viewProjection.M24 - viewProjection.M22;
        //    m_frustum[2].C = viewProjection.M34 - viewProjection.M32;
        //    m_frustum[2].D = viewProjection.M44 - viewProjection.M42;

        //    // Bottom plane
        //    m_frustum[3].A = viewProjection.M14 + viewProjection.M12;   
        //    m_frustum[3].B = viewProjection.M24 + viewProjection.M22;
        //    m_frustum[3].C = viewProjection.M34 + viewProjection.M32;
        //    m_frustum[3].D = viewProjection.M44 + viewProjection.M42;

        //    // Near plane
        //    m_frustum[4].A = viewProjection.M13;   
        //    m_frustum[4].B = viewProjection.M23;
        //    m_frustum[4].C = viewProjection.M33;
        //    m_frustum[4].D = viewProjection.M43;

        //    // Far plane
        //    m_frustum[5].A = viewProjection.M14 - viewProjection.M13;   
        //    m_frustum[5].B = viewProjection.M24 - viewProjection.M23;
        //    m_frustum[5].C = viewProjection.M34 - viewProjection.M33;
        //    m_frustum[5].D = viewProjection.M44 - viewProjection.M43;

        //    // Normalize planes
        //    for ( int i = 0; i < 6; i++ )
        //    {
        //        m_frustum[i] = Plane.Normalize( m_frustum[i] );
        //    }
        //}

        /// <summary>Checks whether a sphere is inside the camera's view frustum.</summary>
        /// <param name="position">Position of the sphere.</param>
        /// <param name="radius">Radius of the sphere.</param>
        /// <returns>true if the sphere is in the frustum, false otherwise</returns>
        //public bool SphereInFrustum( Vector3 position, float radius )
        //{
        //    Vector4 position4 = new Vector4( position.X, position.Y, position.Z, 1f );
        //    for ( int i = 0; i < 6; i++ )
        //    {
        //        if ( Plane.Dot( m_frustum[i], position4 ) + radius < 0 )
        //        {
        //            // Outside the frustum, reject it!
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        /// <summary>Gets the view matrix</summary>
        public Matrix View
        {
            get { return m_viewMatrix; }
        }

        /// <summary>Gets the projection matrix</summary>
        public Matrix Projection
        {
            get { return m_projectionMatrix; }
        }

        /// <summary>Gets and sets the position of the camera</summary>
        public Vector3 PositionV
        {
            get { return m_positionVector; }
            set
            {
                m_positionVector = value;
                UpdateMatrix();
            }
        }

        /// <summary>Gets and sets the field of view</summary>
        public float FOV
        {
            get { return m_fov; }
            set { CreateProjectionMatrix(value, m_aspect, m_nearPlane, m_farPlane); }
        }

        /// <summary>Gets and sets the aspect ratio</summary>
        public float AspectRatio
        {
            get { return m_aspect; }
            set { CreateProjectionMatrix(m_fov, value, m_nearPlane, m_farPlane); }
        }
        public Vector3 LookV
        {
            get
            {
                return m_lookVector;
            }
            set
            {
                m_lookVector = value;
                UpdateMatrix();
            }
        }

        /// <summary>Gets and sets the near plane</summary>
        public float NearPlane
        {
            get { return m_nearPlane; }
            set { CreateProjectionMatrix(m_fov, m_aspect, value, m_farPlane); }
        }

        /// <summary>Gets and sets the far plane </summary>
        public float FarPlane
        {
            get { return m_farPlane; }
            set { CreateProjectionMatrix(m_fov, m_aspect, m_nearPlane, value); }
        }

        /// <summary>Gets and sets the maximum camera velocity</summary>
        public float MaxVelocity
        {
            get { return m_maxSpeed; }
            set { m_maxSpeed = value; }
        }

        /// <summary>Gets and sets whether the y-axis is inverted.</summary>
        public bool InvertY
        {
            get { return m_invertY; }
            set { m_invertY = value; }
        }

        /// <summary>Gets the camera's pitch</summary>
        public float CameraPitch
        {
            get { return m_pitch; }
        }

        /// <summary>Gets the camera's yaw</summary>
        public float CameraYaw
        {
            get { return m_yaw; }
        }

        /// <summary>Gets and sets the maximum pitch in radians.</summary>
        public float MaxPitch
        {
            get { return m_maxPitch; }
            set { m_maxPitch = value; }
        }

        /// <summary>Gets and sets whether the camera can move along its Y-axis.</summary>
        public bool EnableYMovement
        {
            get { return m_enableYMovement; }
            set { m_enableYMovement = value; }
        }
        #endregion
    }
}

