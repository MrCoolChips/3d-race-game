// Prenom : Egemen 
// Nom: YAPSIK 
// Date : 15/04/2025
// Objectif du script : Contrôle de la voiture avec les mouvements de la souris et du clavier
// Note: Il s’agit d’un script Unity, et j’ai uniquement ajouté le contrôle via les mouvements de la souris

using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car;

        private void Awake()
        {
            m_Car = GetComponent<CarController>();
        }

        private void FixedUpdate()
        {
            // Entrées clavier pour tourner et accélérer
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            // Mouvement horizontal de la souris
            float mouseX = Input.GetAxis("Mouse X");

            // Calcul de la direction à partir de la souris (valeurs limitées entre -1 et 1)
            float steering = Mathf.Clamp(mouseX, -1f, 1f);

            // Clic gauche pour avancer
            if (Input.GetMouseButton(0)) {
                v = 1f;
            }

            // Clic droit pour reculer
            if (Input.GetMouseButton(1)) {
                v = -1f;
            }

        #if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump"); // frein à main

            // Si la souris est utilisée pour diriger
            if (steering != 0) {
                m_Car.Move(steering, v, v, handbrake);
            } else {
                m_Car.Move(h, v, v, handbrake);
            }
        #else
            // Même logique, sans le frein à main pour mobile
            if (steering != 0) {
                m_Car.Move(steering, v, v, 0f);
            } else {
                m_Car.Move(h, v, v, 0f);
            }
        #endif
        }
    }
}
