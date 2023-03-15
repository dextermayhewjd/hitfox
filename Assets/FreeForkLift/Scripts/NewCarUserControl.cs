using System;
using UnityEngine;

    [RequireComponent(typeof (NewCarController))]
    public class NewCarUserControl : MonoBehaviour
    {
        private NewCarController m_Car; // the car controller we want to use
        public Transform Car;
        bool Candrive;
        bool Driving = false;
        private Collider collider = null;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<NewCarController>();
        }


        private void FixedUpdate()
        {
            float h = 0, v = 0;

            // pass the input to the car!
            if (Driving) {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
            }

            #if !MOBILE_INPUT
            float handbrake = Input.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
            
            #else
            m_Car.Move(h, v, v, 0f);
            #endif
        }

        private void Update()
        {
            if (Candrive && Input.GetButtonDown("Interact"))
            {
                Driving = !Driving;
                if (Driving)
                {
                    // Here we parent Car with player
                    collider.transform.SetParent(Car);
                    collider.gameObject.SetActive(false);
                }
                else
                {
                    // Here We Unparent the Player with Car
                    collider.transform.SetParent(null);
                    collider.gameObject.SetActive(true);
                }
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Player")
            {
                Candrive = true;
                collider = col;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.tag == "Player")
            {
                Candrive = false;
                collider = null;
            }
        }
    }

