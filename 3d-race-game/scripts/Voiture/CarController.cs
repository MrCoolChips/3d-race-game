// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 25/03/2025
// Objectif du script :Un script qui permet à la voiture d'effectuer ses mouvements de base

//Cette classe appartient à UnityStandardAsset, j'indique les parties que j'ai créées moi-même

using System;
using System.Collections;
using System.Data.Common;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Vehicles.Car
{
    internal enum CarDriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }

    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
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
        [SerializeField] private SpeedType m_SpeedType;
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
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        //Les valeurs que j'ai utilisées lors de l'écriture du code (Egemen YAPSIK)

        public GameObject[] feux_arr;
        public GameObject[] feux_avant;
        bool feux;
        Text vitesse;
        Text engrenage;
        int vitesse_actuelle;
        GameObject cadran;
        Image speedometer;
        Image boost;
        public float valeurBoost = 0;
        public float maxboost = 100;
        bool situationBoost = true;
        public GameObject[] effetBoost;
        public ParticleSystem[] effetVitesse;
        public AudioSource[] sons;
        /*
        sons[0] = Boost
        sons[1] = échappement
        sons[2] = Klaxon
        sons[3] = 360 Feu
        sons[4] = Erreur de Boost
        */
        float rotationTotaleX = 0f;
        float rotationTotaleZ = 0f;
        float rotationXPrecedente = 0f;
        float rotationZPrecedente = 0f;
        Text compteurFeu;
        GameObject feu360;
        public int combo360 = 0;
        bool preparationBoost = false;
        float maxTopSpeed;
        public TransitionDeCamera cam;

        public enum typeDeVoiture {
            Typeless,
            Taxi,
            SUV,
            Police,
            Vintage,
            Race

        }
        public typeDeVoiture type;
        public float carPauseTime;
        public float prevSpeed;
        public bool canControl = true;
        public CarAudio Caudio;
        bool manualBoost = false;
        float tempsDeRecharge = 0.4f;
        private float m_CurrentSteerAngle = 0f;
        public float steerSpeed = 5f;
        private float puissanceDeBoost;
        private KeyCode lightsKey = KeyCode.E;
        private KeyCode hornKey = KeyCode.H;
        private KeyCode boostKey = KeyCode.LeftShift;
        private KeyCode brakeKey = KeyCode.Space;


         //Ici, j’ai utilisé Awake() parce que le GameObject étant créé dynamiquement, certaines connexions ne pouvaient pas être établies autrement
        void Awake() {

            EffectuerDUpgrade();
            canControl = true;
            m_Rigidbody = GetComponent<Rigidbody>();
            //Vous pouvez voir que les éléments UI du compteur de vitesse et du compteur de 360 sont automatiquement liés
            maxTopSpeed = m_Topspeed + m_Topspeed/5;
            vitesse = GameObject.FindWithTag("VitesseActuelle").GetComponent<Text>();
            engrenage = GameObject.FindWithTag("EngrenageActuelle").GetComponent<Text>();
            cadran = GameObject.FindWithTag("Cadran");
            speedometer = GameObject.FindWithTag("Speedometer").GetComponent<Image>();
            boost = GameObject.FindWithTag("Boost").GetComponent<Image>();
            compteurFeu = GameObject.FindWithTag("CompteurFeu").GetComponent<Text>();
            feu360 = GameObject.FindWithTag("ImageFeu");
            feu360.SetActive(false);
            
            //Ce code empêche la disparition de tous les objets AudioSource lors du changement de scène
            foreach (var son in sons) {
                if (son != null) {
                    DontDestroyOnLoad(son.gameObject);
                }
            }

            if (type == typeDeVoiture.Race) {
                tempsDeRecharge = 0.6f;
            } else if (type == typeDeVoiture.Vintage) {
                tempsDeRecharge = 0.2f;
            }

            prevSpeed = m_Topspeed;

            if (type == typeDeVoiture.Vintage) {
                puissanceDeBoost = 0.008f;
            } else {
                puissanceDeBoost = 0.015f;
            }
        }

        private void Start()
        {
            SendHealth();
            feux = false;
            Caudio = GetComponent<CarAudio>();
            // Crée un tableau de Quaternions pour stocker la rotation locale initiale de chaque roue
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                // Boucle sur les 4 roues pour enregistrer leur rotation locale actuelle
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            
            // Définit le centre de gravité du véhicule pour ajuster son comportement physique
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);

            //Cela permet à la boostbar de se remplir à intervalles réguliers
            StartCoroutine(boostbar());

            WheelFrictionCurve sidewaysFriction = m_WheelColliders[2].sidewaysFriction;
            sidewaysFriction.stiffness = 2.5f; 
            m_WheelColliders[2].sidewaysFriction = sidewaysFriction;
            m_WheelColliders[3].sidewaysFriction = sidewaysFriction;
            LoadKeyBindings();
            
        }

        void Update() {
            
            if (!canControl) return;
            Frein();
            Lumiere();
            Speedometer();
            Klaxon();
            UtiliseBoost();
            ControleDansAir();
        }

        void EffectuerDUpgrade() {
            /*Applique toutes les upgrades sélectionnées
            Upgrade LVL 1 => améliore la caractéristique sélectionnée de 5 % supplémentaires
            Upgrade LVL 2 => améliore la caractéristique sélectionnée de 10 % supplémentaires
            Upgrade LVL 3 => améliore la caractéristique sélectionnée de 15 % supplémentaires
            Upgrade LVL 4 => améliore la caractéristique sélectionnée de 20 % supplémentaires
            */
            int upgrade = 0;
            for (int i = 0; i < 4; i++) {
                upgrade = PlayerPrefs.GetInt(gameObject.name.Split("(")[0] + "_upgrade_selectionne_" + i, 0);
                if (upgrade != 0) {
                    switch (i) {
                        case 0:
                        m_Topspeed += m_Topspeed * (upgrade * 5 / 100.0f);
                        break;
                        case 1:
                        m_BrakeTorque += m_BrakeTorque * (upgrade * 5 / 100.0f);
                        break;
                        case 2:
                        //Si la capacité de boost dépasse 100 à cause des améliorations, la barre de boost de la voiture est remplie au début
                        maxboost += maxboost* (upgrade * 5 / 100.0f);
                        if (maxboost > 100.0f) {
                            valeurBoost = maxboost - 100.0f;
                            maxboost = 100.0f;
                        }
                        break;
                        case 3:
                        m_FullTorqueOverAllWheels += m_FullTorqueOverAllWheels * (upgrade * 5 / 100.0f);
                        break;
                    }

                };
            }
        }

        void UtiliseBoost() {
            //Cette fonction UtiliseBoost() augmente la vitesse du véhicule lorsque le joueur appuie sur Shift gauche et que le boost est disponible.

            if (((Input.GetMouseButton(2) || Input.GetKey(boostKey)) && valeurBoost > 0 && Input.GetAxis("Vertical") != -1 && !preparationBoost) || (manualBoost && !preparationBoost)) {
                
                //La barre de boost devient rouge et se vide au fur et à mesure
                m_Rigidbody.velocity += puissanceDeBoost * m_Rigidbody.velocity.normalized;
                valeurBoost -= 0.25f;
                //Tant que le boost est utilisé, la vitesse maximale de la voiture augmente
                m_Topspeed = maxTopSpeed; 
                //Si situationBoost est égal à False, la boostbar ne se remplit pas
                situationBoost = false;
                boost.fillAmount = valeurBoost/100;
                boost.color = Color.red;

                if (!sons[0].isPlaying) {
                    sons[0].Play();
                }

                foreach (var effect in effetBoost) {
                    effect.SetActive(true);
                }

                if (valeurBoost <= 0) {
                    foreach (var effect in effetBoost) {
                        effect.SetActive(false);
                    }
                    sons[0].Stop();
                    //Avec un simple calcul mathématique, la vitesse maximale revient à sa valeur initiale
                    m_Topspeed = maxTopSpeed * 5/6; 
                    //Si le boost est complètement épuisé, il ne peut pas être utilisé à nouveau pendant un certain temps, mais pendant cette période, le boost se recharge progressivement
                    StartCoroutine(BoostCooldown());
                    valeurBoost = 0;
                    situationBoost = true;
                    return;
                }
            }

            if (Input.GetMouseButtonUp(2) || Input.GetKeyUp(boostKey)) {
                //orsque nous relâchons la touche Shift gauche, la même action (valeurBoost <= 0) se répète
                foreach (var effect in effetBoost) {
                    effect.SetActive(false);
                }
                m_Topspeed = maxTopSpeed * 5/6; 
                sons[0].Stop();

                situationBoost = true;
            }

        }

        IEnumerator BoostCooldown() {
            //Si le boost est complètement épuisé, il ne peut pas être utilisé à nouveau pendant un certain temps
            preparationBoost = true; 
            boost.color = Color.yellow;
            sons[4].Play();
            yield return new WaitForSeconds(5f); 
            preparationBoost = false;
            boost.color = new Color32(0, 166, 255, 255);
        }   

        void Speedometer() {
            //Cette fonction Speedometer() met à jour le compteur de vitesse du véhicule
            vitesse_actuelle = (int)CurrentSpeed;
            vitesse.text = vitesse_actuelle.ToString();

            //Avec le changement de vitesse, la barre créée avec Fill diminue ou augmente en conséquence
            float fillAmount = Mathf.Clamp(CurrentSpeed / MaxSpeed, 0, 1);
            //Le cadran du compteur tourne en fonction de la vitesse actuelle pour donner une indication visuelle
            float rotationZ = Mathf.Lerp(120.353f, -130.05f, fillAmount);
            speedometer.fillAmount = fillAmount;
            cadran.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        }

        IEnumerator boostbar() {
            //Cette fonction boostbar() est une coroutine qui régénère la barre de boost à intervalles réguliers.
            while (true) {
                yield return new WaitForSeconds(tempsDeRecharge);
                //Si situationBoost == false, c'est-à-dire si le boost n'est pas utilisé ou il est à sa valeur maximal, la boostbar continue de se remplir au fil du temps
                if (situationBoost) {
                    valeurBoost += 2;
                    //Si Boostbar est temporairement désactivé, il deviendra jaune (!preparationBoost == false)
                    if (!preparationBoost) {
                        boost.color = new Color32(0, 166, 255, 255); //Ici, il est bleu
                    }

                    if (valeurBoost >= maxboost) {
                        valeurBoost = maxboost;
                        situationBoost = false;
                    }

                    boost.fillAmount = valeurBoost/100;
                }
            }
        }

        void ControleDansAir() {
            /*
            Cette fonction ControleDansAir() vérifie si la voiture est en l'air en testant si aucune de ses roues ne touche le sol
            Si elle est en l'air, elle applique un couple (AddTorque) pour permettre au joueur de la faire pivoter avec les flèches directionnelles
            De plus, si la hauteur dépasse 2.5 mètres, la caméra d'action sera activé
            */

            cam = GameObject.FindWithTag("GameController").GetComponent<TransitionDeCamera>();
            bool isGrounded = false;
            //Ici, il teste si les roues touchent le sol ou non
            foreach (var wheel in m_WheelColliders) {
                if (wheel.isGrounded)
                {
                    isGrounded = true;
                    break;
                }
            }


            if (!isGrounded) {
                //Il prend les angles de rotation actuels de l'objet (sur les axes X et Z)
                float rotationXActuelle = transform.eulerAngles.x;
                float rotationZActuelle = transform.eulerAngles.z;

                //Calculer les différences d'angle
                float deltaX = rotationXActuelle - rotationXPrecedente ;
                if (deltaX < -180f) {
                    deltaX += 360f;
                }
                if (deltaX > 180f) { 
                    deltaX -= 360f;
                }
                float deltaZ = rotationZActuelle - rotationZPrecedente;
                if (deltaZ < -180f) {
                    deltaZ += 360f;
                } 
                if (deltaZ > 180f) {
                    deltaZ -= 360f;
                }

                //Il calcule le rendement total
                rotationTotaleX  += Mathf.Abs(deltaX);
                rotationTotaleZ  += Mathf.Abs(deltaZ);

                rotationXPrecedente = rotationXActuelle;
                rotationZPrecedente = rotationZActuelle;

                //J'ai utilisé GetKey au lieu de GetKeyDown ici, car je veux que le véhicule accélère à mesure qu'il fait des flips
                //J'ai ajouté AddForce ici parce que je veux que la voiture se déplace légèrement dans la direction de la rotation lorsqu'elle fait un tonneau
                //J'ai utilisé AddTorque pour appliquer une rotation à la voiture.
                if (Input.GetKey(KeyCode.UpArrow) && m_Rigidbody != null) {
                    m_Rigidbody.AddTorque(transform.right * 0.25f, ForceMode.Acceleration); 
                    m_Rigidbody.AddForce(transform.forward * 1f, ForceMode.Acceleration);
                }  else if (Input.GetKey(KeyCode.DownArrow) && m_Rigidbody != null){
                    m_Rigidbody.AddTorque(transform.right * -0.25f, ForceMode.Acceleration); 
                    m_Rigidbody.AddForce(transform.forward * -1f, ForceMode.Acceleration);
                }
                
                if (Input.GetKey(KeyCode.LeftArrow ) && m_Rigidbody != null) {
                    m_Rigidbody.AddTorque(transform.forward  * 0.25f, ForceMode.Acceleration); 
                    m_Rigidbody.AddForce(transform.right * -1f, ForceMode.Acceleration);
                    
                } else if (Input.GetKey(KeyCode.RightArrow) && m_Rigidbody != null) {
                    m_Rigidbody.AddTorque(transform.forward  * -0.25f, ForceMode.Acceleration); 
                    m_Rigidbody.AddForce(transform.right * 1f, ForceMode.Acceleration);
                }

                //Pour donner une ambiance plus cinématographique, la caméra d'action s'active lorsque la hauteur atteint 2,5f et que la voiture est en l'air
               if (m_Rigidbody.position.y > 2.5f) {
                    cam.AjusterLaCamera(3);
                }

                if (rotationTotaleX >= 360f || rotationTotaleZ >= 360f) {
                    rotationTotaleX = 0f;
                    rotationTotaleZ = 0f;
                    //J'ai ajouté un Canvas qui montre notre combo lorsque nous faisons un salto (feu360)
                    feu360.SetActive(true);
                    combo360++;
                    compteurFeu.text = "x" + combo360;
                    sons[3].Play();
                    //J'ai utilisé cette Coroutine pour désactiver à nouveau le Canvas après un certain temps
                    StartCoroutine(ArreterActiveDansTemps(4f));
                }

            } else {
                cam.AjusterLaCamera(cam.cameraActive);
            }
        }

        IEnumerator ArreterActiveDansTemps(float retard) {
            //Augmente la durée du menu 360Feu à chaque fois que la voiture roule. Le menu se ferme lorsque le compteur est épuisé.
            int ancienCombo = combo360;
            while (true) {
                yield return new WaitForSeconds(retard);
                if (ancienCombo != combo360) {
                    ancienCombo = combo360;
                } else {
                    break;
                }
            }
            feu360.SetActive(false);
        }

        void Klaxon() {
            //Lorsque la touche H est pressée, le klaxon fonctionne
            if (Input.GetKeyDown(hornKey)){
                sons[2].Play();
            }
        }

        void Lumiere() {
            //Lorsque la touche E est pressée, les phares avant s'allument
            if (Input.GetKeyDown(lightsKey)){
                feux = !feux;
                foreach (var lumiere in feux_avant) {
                    lumiere.SetActive(feux);
                }
            }
        }

        void Frein() {
            //Cette fonction appartient à Unity
            //Lorsque la touche Space est pressée, le frein à main est activé

            if (Input.GetKeyDown(brakeKey)) {
                //Lorsque la touche Fren est pressée, les feux arrière de la voiture s'allument
                foreach (var lumiere in feux_arr) {
                    lumiere.SetActive(true);
                }
                //Elle applique le frein aux roues lorsque la touche Fren est pressée
                for (int i = 0; i < 4; i++) {
                    m_WheelColliders[i].GetComponent<WheelCollider>().brakeTorque = m_BrakeTorque;
                }
            }

            if (Input.GetKeyUp(brakeKey)) {
                //Une fois que le freinage est terminé, l'effet du frein est également annulé
                foreach (var lumiere in feux_arr) {
                    lumiere.SetActive(false);
                }

                for (int i = 0; i < 4; i++) {
                    m_WheelColliders[i].GetComponent<WheelCollider>().brakeTorque = 0;
                }
            }

        }


        private void GearChanging()
        {
            //Cette fonction appartient à Unity, j'ai seulement intégré le compteur de vitesse
            //Les opérations d'augmentation et de réduction de la vitesse sont effectuées ici

            //f représente la valeur absolue du rapport entre la vitesse actuelle et la vitesse maximale
            float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
            float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
            float downgearlimit = (1/(float) NoOfGears)*m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
                engrenage.text = (m_GearNum + 1).ToString();
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
                engrenage.text = (m_GearNum + 1).ToString();
                ////Une animation d'explosion d'échappement est commence lorsque la vitesse augmente   
                foreach (var effect in effetVitesse) {
                    if (!sons[0].isPlaying) {
                        sons[1].Play();
                        effect.Play();
                    }
                }
            }

            //Si la voiture est a l'arrét, il affiche P
            if (vitesse_actuelle == 0) {
                engrenage.text = "P";
            } else if (vitesse_actuelle > 0) {
                if (!Input.GetKey(brakeKey)) {
                    foreach (var lumiere in feux_arr) {
                        lumiere.SetActive(false);
                    }
                }
                engrenage.text = (m_GearNum + 1).ToString();
            } 
            
            //Si la voiture roule en arriére, il affiche R
            if (Input.GetAxis("Vertical") == -1) {

                foreach (var lumiere in feux_arr) {
                    lumiere.SetActive(true);
                }
                engrenage.text = "R";
            }
        }

        public float EnvoyerLaVitesseMaximale() {
            return m_Topspeed;
        }
        public void AugmenterLaVitesseMaximale(float vitesse) {
            m_Topspeed += vitesse;
        }

        private void ApplyDrive(float accel, float footbrake)
        {
            //Il s'agit d'un script Unity, j'ai simplement amélioré le système de freinage
            float thrustTorque = 0f;

            // Si on accélère vers l'avant
            if (accel > 0f)
            {
                // Calcul du couple moteur selon le type de transmission
                thrustTorque = accel * (m_CurrentTorque / (m_CarDriveType == CarDriveType.FourWheelDrive ? 4f : 2f));

                // Appliquer le couple moteur aux roues appropriées
                if (m_CarDriveType == CarDriveType.FourWheelDrive)
                    for (int i = 0; i < 4; i++)
                        m_WheelColliders[i].motorTorque = thrustTorque;
                else if (m_CarDriveType == CarDriveType.FrontWheelDrive)
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                else // RearWheelDrive
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
            }
            // Si on freine et que la voiture recule
            else if (footbrake > 0f && (Input.GetAxis("Vertical") < 0 || Input.GetMouseButton(1)))
            {
                // Appliquer le couple moteur inversé pour la marche arrière
                thrustTorque = -m_ReverseTorque * footbrake;

                if (m_CarDriveType == CarDriveType.FourWheelDrive)
                    for (int i = 0; i < 4; i++)
                        m_WheelColliders[i].motorTorque = thrustTorque;
                else if (m_CarDriveType == CarDriveType.FrontWheelDrive)
                    m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                else
                    m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
            }
            else
            {
                // Aucun couple moteur appliqué si pas d'accélération ni de freinage
                for (int i = 0; i < 4; i++)
                    m_WheelColliders[i].motorTorque = 0f;
            }

            // Appliquer le frein selon la vitesse et la direction
            for (int i = 0; i < 4; i++)
            {
                if (CurrentSpeed > 5f && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                }
                else
                {
                    m_WheelColliders[i].brakeTorque = 0f;
                }
            }
        }


        //Mes fonctions que j'ai écrites ici sont terminées, le reste appartient a autres membres

        void SendHealth() {
            // Envoie les points de vie (vie) au script de santé en fonction du type de voiture
            /*
            Typeless,
            Taxi,
            SUV,
            Police,
            Vintage,
            Race
            */
            float health = 100f;
            switch ((int) type) {
                case 0:
                health = 80f;
                break;
                case 1:
                health = 70f;
                break;
                case 2:
                health = 100f;
                break;
                case 3:
                health = 90f;
                break;
                case 4:
                health = 50f;
                break;
                case 5:
                health = 40f;
                break;
            }
            GamePlayHandler gpy = GameObject.FindWithTag("GameController").GetComponent<GamePlayHandler>();
            gpy.maxHealth = health;
            gpy.health = health;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canControl) return;
            if (other.gameObject.GetComponent<Booster>() != null)
            {
                other.gameObject.SetActive(false);
                StartManualBoost();
            }
            else if (other.gameObject.GetComponent<SpeedLimit>() != null)
            {
                if (Caudio.isPolice)
                    return;
                if (CurrentSpeed >= GamePlayHandler.Instance.speedCamera)
                {
                    PauseCarFor5Seconds();
                }
            }
            else if (other.gameObject.GetComponent<Slippery>() != null)
            {
                if (type == typeDeVoiture.SUV) return;
                SlipCar(GamePlayHandler.Instance.slipTime);
            }
            else if (other.GetComponent<ObstacleDamage>() != null)
            {
                GamePlayHandler.Instance.GetDamage(other.GetComponent<ObstacleDamage>().damage, other.gameObject);
                if (other.GetComponent<ObstacleDamage>().isDestroyAble)
                {
                    other.gameObject.SetActive(false);
                }
            }
            else if (other.GetComponent<Coins>() != null)
            {
                GamePlayHandler.Instance.AddCoins(other.GetComponent<Coins>().coins, other.gameObject);
                other.gameObject.SetActive(false);

            }
            else if (other.GetComponent<SlowSpeed>() != null)
            {
                if (type == typeDeVoiture.SUV) return;
                m_Topspeed = other.GetComponent<SlowSpeed>().speed;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<ContinuesDamage>() != null)
            {
                if (type == typeDeVoiture.SUV) return;
                GamePlayHandler.Instance.GetDamage(other.GetComponent<ContinuesDamage>().Damage, other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<SlowSpeed>() != null)
            {
                m_Topspeed = prevSpeed;
            }
        }
        public void StartManualBoost()
        {

            StartCoroutine(StartManualBoostEnum());
        }
        IEnumerator StartManualBoostEnum()
        {
            valeurBoost = maxboost;
            situationBoost = false;
            boost.fillAmount = valeurBoost / 100;
            manualBoost = true;
            yield return new WaitUntil(() => valeurBoost <= 0);
            manualBoost = false;
        }

        public void SlipCar(float duration = 2f)
        {
            StartCoroutine(SlipEffect(duration));
        }
        public void PauseCarFor5Seconds()
        {
            foreach (var effect in effetBoost) {
                effect.SetActive(false);
            }
            sons[0].Stop();
            //Avec un simple calcul mathématique, la vitesse maximale revient à sa valeur initiale
            m_Topspeed = maxTopSpeed * 5/6; 
            StartCoroutine(BoostCooldown());
            situationBoost = true;
            StartCoroutine(PauseCarRoutine());
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

        private IEnumerator PauseCarRoutine()
        {
            Vector3 originalVelocity = m_Rigidbody.velocity;

            // Stop car immediately
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;

            // Disable player control (assuming Move is called externally)
            canControl = false;

            yield return new WaitForSeconds(5f);

            // Re-enable control
            canControl = true;
        }

        private void LoadKeyBindings()
        {
            if (PlayerPrefs.HasKey("LightsKey"))
                lightsKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LightsKey"));
            if (PlayerPrefs.HasKey("HornKey"))
                hornKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("HornKey"));
            if (PlayerPrefs.HasKey("BoostKey"))
                boostKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BoostKey"));
            if (PlayerPrefs.HasKey("BrakeKey"))
                brakeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BrakeKey"));
        }

        //Notre fonctions que nous avons écrites ici sont terminées, le reste appartient a Unity
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
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

            float currentSpeed = m_Rigidbody.velocity.magnitude * 3.6f;
            float speedFactor = Mathf.Clamp01(currentSpeed / m_Topspeed);
            float steerLimit = Mathf.Lerp(m_MaximumSteerAngle, 10f, speedFactor);

            m_CurrentSteerAngle = Mathf.Lerp(m_CurrentSteerAngle, steering * steerLimit, steerSpeed * Time.deltaTime);

            m_WheelColliders[0].steerAngle = m_CurrentSteerAngle;
            m_WheelColliders[1].steerAngle = m_CurrentSteerAngle;

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
            TractionControl();
        }


        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
                    break;
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
            switch (m_CarDriveType)
            {
                case CarDriveType.FourWheelDrive:
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);

                        AdjustTorque(wheelHit.forwardSlip);
                    }
                    break;

                case CarDriveType.RearWheelDrive:
                    m_WheelColliders[2].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);

                    m_WheelColliders[3].GetGroundHit(out wheelHit);
                    AdjustTorque(wheelHit.forwardSlip);
                    break;

                case CarDriveType.FrontWheelDrive:
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
