using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;

    class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
    {
        // Encoded RSAPublicKey
        private static string PUB_KEY = "30818902818100C4A06B7B52F8D17DC1CCB47362" +
            "C64AB799AAE19E245A7559E9CEEC7D8AA4DF07CB0B21FDFD763C63A313A668FE9D764E" +
            "D913C51A676788DB62AF624F422C2F112C1316922AA5D37823CD9F43D1FC54513D14B2" +
            "9E36991F08A042C42EAAEEE5FE8E2CB10167174A359CEBF6FACC2C9CA933AD403137EE" +
            "2C3F4CBED9460129C72B0203010001";

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();
            Debug.Log("pk :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: " + pk);

            return true;

        }
    }

