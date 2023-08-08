using System;
using System.Collections;
using UnityEngine;

namespace CheatEngine
{
    public class CheatEngine : MonoBehaviour
    {
        public bool cheatEnabled = true;
        public bool unlockDrivers = true; //Toggle para habilitar o deshabilitar UnlockDrivers

        //Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(UnlockDrivers());
        }
        private void Update()
        {
            if (cheatEnabled)
            {
                if (unlockDrivers)
                {
                    StartCoroutine(UnlockDrivers()); //Iniciar la coroutine UnlockDrivers
                }
            }
        }

        public void ToggleCheatEnabled()
        {
            cheatEnabled = !cheatEnabled; //Invertir el estado del toggle

#if DEBUG
            Debug.Log("Cheat Enabled:" + cheatEnabled);
#endif
        }

        public void ToggleUnlockDrivers()
        {
            unlockDrivers = !unlockDrivers; //Invertir el estado del toggle
            Debug.Log("Unlock Drivers:" + unlockDrivers);
        }

        private IEnumerator UnlockDrivers()
        {
            byte[] data = { 0xFF, 0x01 }; //Datos a escribir
            IntPtr address = new IntPtr(0x8006B9E8); //Dirección Base

            for (int i = 0; i < 35; i++)
            {
                IntPtr modifiedAddress = IntPtr.Add(address, i * 0xA); //Desplazamiento
                uint modifiedAddressUint = (uint)modifiedAddress;
                LegacyMemoryReader.WriteBytes(modifiedAddressUint, data);
                yield return null; //Espera un frame antes de continuar con la siguiente iteración
            }
            //Fin de la coroutine
        }
    }
}