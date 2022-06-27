using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using DigitalSignageClient.Data.Model;
using DigitalSignageClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DigitalSignageClient.Models.LicenseInfo;

namespace DigitalSignageClient.Controllers
{
    [Produces("application/json")]
    [Route("api/Licenses")]
    public class LicensesController : Controller
    {



        [HttpGet]
        [Route("GetUID")]
        public ActivatedScreen GetUID()
        {

            ActivatedScreen screen = new ActivatedScreen();
            string dId = GetDeviceId();
            if (String.Equals(dId, "ERROR"))
            {
                return new ActivatedScreen() { UID = "MAC NOT FOUND" };
            }
            screen.UID = StringCipher.Encrypt(dId,"A1D6A1D6");
            screen.Id = "0";
            return screen;
        }

        private string GetDeviceId()
        {
            try
            {
                string deviceId = new DeviceIdBuilder()
                    .AddMotherboardSerialNumber()
                    .ToString();
                return deviceId;

            }
            catch (Exception ex)
            {
                return "ERROR";
            }

        }



        [HttpGet]
        [Route("RemoveLicense")]
        public ActivatedScreen RemoveLicense()
        {
            ActivatedScreen screen = new ActivatedScreen();
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\License\", "license.xml");
            screen.UID = StringCipher.Encrypt(GetDeviceId(), "R3M2V6");
            screen.Id = "0";
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return screen;
            }
            else
            {
                return new ActivatedScreen() { UID = "NOT ACTIVATED" };
            }
        }

        //[HttpGet]
        //[Route("CheckLicense")]
        //public string CheckLicense()
        //{
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\License\", "license.xml");

        //    if (System.IO.File.Exists(filePath))
        //        return GetExpireDate().ToShortDateString();
        //    else
        //        return "NOT EXIST";
        //}

        //private DateTime GetExpireDate()
        //{
        //    //LicenseStatus screen = _li
        //    screen.ExpireDate = StringCipher.Decrypt(screen.ExpireDate, "L4Y6S1N6S6");
        //    screen.ServerUID = StringCipher.Decrypt(screen.ServerUID, "L4Y6S1N6S6");
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\License\", "license.xml");

        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return new DateTime(1900, 01, 01);
        //    }
        //    DecryptFile(filePath, "F4n2e5l6");
        //    ScreenLicense license = DeSerializeObject(filePath);
        //    EncryptFile(filePath, "F4n2e5l6");
        //    return DateTime.Parse(license.ExpireDate);

        //}

        //[HttpGet]
        //[Route("CheckExpireDate")]
        //public bool CheckExpireDate()
        //{
        //    DateTime expireDate = GetExpireDate();
        //    return expireDate >= DateTime.Now ? true : false;
        //}

        private void EncryptFile(string filePath, string key)
        {
            byte[] plainContent = System.IO.File.ReadAllBytes(filePath);
            using (var DES = new DESCryptoServiceProvider())
            {
                DES.IV = Encoding.UTF8.GetBytes(key);
                DES.Key = Encoding.UTF8.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(plainContent, 0, plainContent.Length);
                    cryptoStream.FlushFinalBlock();
                    System.IO.File.WriteAllBytes(filePath, memStream.ToArray());

                }
            }
        }

        private static void DecryptFile(string filePath, string key)
        {

            byte[] encrypted = System.IO.File.ReadAllBytes(filePath);
            using (var DES = new DESCryptoServiceProvider())
            {
                DES.IV = Encoding.UTF8.GetBytes(key);
                DES.Key = Encoding.UTF8.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateDecryptor(), CryptoStreamMode.Write);
                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    cryptoStream.FlushFinalBlock();
                    System.IO.File.WriteAllBytes(filePath, memStream.ToArray());

                }
            }
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        private void SerializeObject(ScreenLicense screen, string fileName)
        {
            if (screen == null) { return; }
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(screen.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, screen);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }


        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private ScreenLicense DeSerializeObject(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return new ScreenLicense(); }

            ScreenLicense screen = new ScreenLicense();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(ScreenLicense);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        screen = (ScreenLicense)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return screen;
        }

    }
}
