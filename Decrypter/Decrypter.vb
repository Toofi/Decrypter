Imports System.Security.Cryptography
Imports System.Text

Public Class Decrypter
    Public Shared Function DecryptString(ByVal cipherText As String, ByVal password As String) As String

        Dim values = ExtractMessageAndNonce(cipherText)
        Dim messageByte As Byte() = values.Item1
        Dim nonce As UInteger = values.Item2

        Dim SaltBytes As Byte() = Encoding.UTF8.GetBytes(password)
        Dim key As New Rfc2898DeriveBytes(password, SaltBytes)
        Dim cryptoKey As Byte() = key.GetBytes(16)
        Dim iv As Byte() = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}
        Dim aes As New AesCryptoServiceProvider()
        aes.BlockSize = 128
        aes.KeySize = 128
        aes.Key = cryptoKey
        aes.IV = iv
        aes.Padding = PaddingMode.PKCS7
        aes.Mode = CipherMode.CBC

        Dim cryptoTransform As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)

        'InvalidCastException s'il manque le nonce
        While nonce > 0
            Try
                messageByte = cryptoTransform.TransformFinalBlock(messageByte, 0, messageByte.Length)
            Catch ex As CryptographicException
                'créer un faux message avec le même nonce
                Dim falseDataBytes As Byte() = Encoding.UTF8.GetBytes("Oops! Wrong password, try again :)")

                Dim falseBlock As Byte() = TransformMessage(falseDataBytes, nonce, password)
                messageByte = falseBlock
                Continue While
            End Try
            Dim messageString As String = BitConverter.ToString(messageByte).Replace("-", "")
            Dim firstTenChars As String = messageString.Substring(0, Math.Min(messageString.Length, 100))

            Console.Write(firstTenChars + vbCr)
            If nonce Mod 100 = 0 Then
                Console.WriteLine("")
            End If
            nonce -= 1

        End While

        Return Encoding.UTF8.GetString(messageByte)
    End Function

    Private Shared Function ExtractMessageAndNonce(ByVal cipherText As String) As (Byte(), UInteger)
        Dim values As String() = cipherText.Split("|"c)

        Dim message As String = values(0)

        Dim messageByte As Byte() = Convert.FromBase64String(message)
        'FormatException si message n'est pas base64 ou s'il manque le | mais un nonce
        Dim nonce As String = values(1)
        'IndexOutOfRangeException s'il y a bien un message base64 mais pas de | ni de nonce
        Dim nonceInt As UInteger
        UInteger.TryParse(nonce, nonceInt)
        Return (messageByte, nonce)
    End Function

    Private Shared Function TransformMessage(ByVal dataBytes As Byte(), ByVal nonce As UInteger, ByVal password As String) As Byte()
        Dim cryptoTransformer As ICryptoTransform = GetCryptoTransformer(password)
        Dim i As UInteger = 0
        While i < nonce
            dataBytes = cryptoTransformer.TransformFinalBlock(dataBytes, 0, dataBytes.Length)
            i += 1
        End While
        Return dataBytes
    End Function

    Private Shared Function GetCryptoTransformer(ByVal password As String) As ICryptoTransform
        Dim SaltBytes As Byte() = Encoding.UTF8.GetBytes(password)
        Dim key As New Rfc2898DeriveBytes(password, SaltBytes)
        Dim cryptoKey As Byte() = key.GetBytes(16)
        Dim iv As Byte() = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16}

        Dim aes As New AesCryptoServiceProvider()
        aes.BlockSize = 128
        aes.KeySize = 128
        aes.Key = cryptoKey
        aes.IV = iv
        aes.Padding = PaddingMode.PKCS7
        aes.Mode = CipherMode.CBC
        Dim cryptoTransform As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)
        Return cryptoTransform
    End Function
End Class
