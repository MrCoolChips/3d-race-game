using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum AdversaireCarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum AdversaireSpeedType
    {
        MPH,
        KPH
    }
    public class AdversaireController : MonoBehaviour
    {
        [SerializeField] private AdversaireCarDriveType m_AdversaireCarDriveType = AdversaireCarDriveType.FourWheelDrive;
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private AdversaireSpeedType m_AdversaireSpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;
        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        public enum typeDeVoiture {
            Typeless,
            Taxi,
            SUV,
            Police,
            Vintage,
            Race

        }

        public int indexTrajetDeComputer;
        public typeDeVoiture type;
        [SerializeField] private float valeurBoost = 0f;
        private float maxBoost = 100f;
        private bool isBoosting = false;
        private bool isOnCooldown = false;
        double boostCooldownTime;
        public GameObject[] effetBoost;
        private float maxTopSpeed;
        private float prevSpeed;

        void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();     
        }

        private void Start()
        {
            maxTopSpeed = m_Topspeed + m_Topspeed/5;
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
            System.Random rand = new System.Random();
            boostCooldownTime = 25.0 + (rand.NextDouble() * (50.0 - 25.0));
            StartCoroutine(BoostCooldown());
            prevSpeed = m_Topspeed;
            
        }

        void Update() {
            if (valeurBoost > 0 && !isOnCooldown) {
                isBoosting = true;
            }
            else {
                isBoosting = false;
            }

            if (isBoosting) {
                UtiliseBoost();
            }
        }


        void UtiliseBoost() {
            m_Rigidbody.velocity += 0.015f * m_Rigidbody.velocity.normalized;
            valeurBoost -= 0.5f;

            m_Topspeed = maxTopSpeed;

            foreach (var effect in effetBoost) {
                effect.SetActive(true);
            }

            if (valeurBoost <= 0) {
                foreach (var effect in effetBoost) {
                    effect.SetActive(false);
                }
                m_Topspeed = maxTopSpeed * 5 / 6;
                StartCoroutine(BoostCooldown());
            }
        }

        IEnumerator BoostCooldown() {
            isOnCooldown = true;
            yield return new WaitForSeconds((float)boostCooldownTime);
            isOnCooldown = false;
            valeurBoost = maxBoost;
        }

        void StartManualBoost() {
            isOnCooldown = false;
            valeurBoost = maxBoost;
        }

        public void PauseCarFor5Seconds()
        {
            foreach (var effect in effetBoost) {
                effect.SetActive(false);
            }
            if (valeurBoost > 0) {
                m_Topspeed = maxTopSpeed * 5/6; 
            }
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
        }

        public void SlipCar(float duration = 2f)
        {
            StartCoroutine(SlipEffect(duration));
        }

        private IEnumerator SlipEffect(float duration)
        {
            yield return null;
            foreach (var wheel in m_WheelColliders)
            {
                wheel.forwardFriction = ModifyFriction(wheel.forwardFriction, 0.2f);
                wheel.sidewaysFriction = ModifyFriction(wheel.sidewaysFriction, 0.2f);

            }

            while (duration > 0)
            {
                yield return null;
                duration -= Time.deltaTime;
                m_Rigidbody.AddTorque(transform.forward * 0.25f, ForceMode.Acceleration);
                m_Rigidbody.AddForce(transform.right * -1f, ForceMode.Acceleration);
            }


            foreach (var wheel in m_WheelColliders)
            {
                wheel.forwardFriction = ModifyFriction(wheel.forwardFriction, 1f);
                wheel.sidewaysFriction = ModifyFriction(wheel.sidewaysFriction, 1f);
            }
        }

        private WheelFrictionCurve ModifyFriction(WheelFrictionCurve friction, float stiffness)
        {
            friction.stiffness = stiffness;
            return friction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Booster>() != null)
            {
                other.gameObject.SetActive(false);
                StartManualBoost();
            } else if (other.gameObject.GetComponent<SpeedLimit>() != null)
            {
                if (type == typeDeVoiture.Police)
                    return;
                if (CurrentSpeed >= GamePlayHandler.Instance.speedCamera)
                {
                    PauseCarFor5Seconds();
                }
            } else if (other.gameObject.GetComponent<Slippery>() != null)
            {
                if (type == typeDeVoiture.SUV) return;
                SlipCar(GamePlayHandler.Instance.slipTime);
            } else if (other.GetComponent<Coins>() != null)
            {
                other.gameObject.SetActive(false);
            } else if (other.GetComponent<SlowSpeed>() != null)
            {
                if (type == typeDeVoiture.SUV) return;
                m_Topspeed = other.GetComponent<SlowSpeed>().speed;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<SlowSpeed>() != null)
            {
                m_Topspeed = prevSpeed;
            }
        }


        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
            float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
            float downgearlimit = (1/(float) NoOfGears)*m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }
        } 

        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }


        private void CalculateGearFactor()
        {
            float f = (1/(float) NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = m_GearNum/(float) NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering*m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
            if (handbrake > 0f)
            {
                var hbTorque = handbrake*m_MaxHandbrakeTorque;
                m_WheelColliders[2].brakeTorque = hbTorque;
                m_WheelColliders[3].brakeTorque = hbTorque;
            }


            CalculateRevs();
            GearChanging();

            AddDownForce();
            //CheckForWheelSpin();
            TractionControl();
        }


        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_AdversaireSpeedType)
            {
                case AdversaireSpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case AdversaireSpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }


        private void ApplyDrive(float accel, float footbrake)
        {
            float thrustTorque;

            switch (m_AdversaireCarDriveType)
            {
                case AdversaireCarDriveType.FourWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 4f);
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;
                    }
                    break;

                case AdversaireCarDriveType.FrontWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                    break;

                case AdversaireCarDriveType.RearWheelDrive:
                    thrustTorque = accel * (m_CurrentTorque / 2f);
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                    break;
            }

            // Geri vitese geçme koşulunu kaldıralım veya kontrol ekleyelim:
            for (int i = 0; i < 4; i++)
            {
                if (footbrake > 0)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;

                    // Geri vites yalnızca manuel olarak etkinleştirilirse verilsin (örneğin AI geri çıkma fonksiyonu çağırdıysa)
                    // Eğer AI'nın bilinçli olarak geri gitmesini istemiyorsan bu satırı tamamen kaldır:
                    // m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
                }
                else
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                }
            }
        }



        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up*m_Downforce*
                                                         m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            switch (m_AdversaireCarDriveType)
            {
                case AdversaireCarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case AdversaireCarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case AdversaireCarDriveType.FrontWheelDrive:
                    m_WheelColliders[0].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[1].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }
    }
}
