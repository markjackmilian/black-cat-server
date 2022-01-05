using System.Security.Cryptography;
using System.Text;

namespace BlackHat_Server.Class.Crypt
{
    internal class File_Des
    {
        public byte[] EncryptFile(byte[] toEncryptArray, bool useHashing)
        {
            var key = "BlackHat2011_EncryPsW!";

            byte[] keyArray;
            //byte[] toEncryptArray = File.ReadAllBytes(filetoEncrypt);

            //byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(key);
            }

            var tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            var cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            var resultArray =
                cTransform.TransformFinalBlock(toEncryptArray, 0,
                    toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return resultArray;
        }


        public byte[] DecryptFile(byte[] toEncryptArray, bool useHashing)
        {
            var key = "BlackHat2011_EncryPsW!";
            byte[] keyArray;
            //get the byte code of the string


            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = Encoding.UTF8.GetBytes(key);
            }

            var tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            var cTransform = tdes.CreateDecryptor();

            var resultArray = cTransform.TransformFinalBlock(
                toEncryptArray, 0, toEncryptArray.Length);


            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return Byte[] Decrypted
            return resultArray;
        }
    }
}