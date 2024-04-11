Imports System

Module Program
    Sub Main(args As String())
        Dim retry As String = "r"
        While retry = "r"
            Dim messageIsDecrypted = False
            While messageIsDecrypted <> True
                Console.WriteLine("Type the block to decrypt:")
                Dim messageForDecrypt = Console.ReadLine

                Console.WriteLine("Type the block password:")
                Dim passWordForDecrypt = Console.ReadLine

                Try
                    Dim decryptedMessage As String = Decrypter.DecryptString(messageForDecrypt, passWordForDecrypt)
                    Console.WriteLine(vbCrLf)
                    Console.WriteLine("Congratulations! The block is well decrypted!")
                    Console.WriteLine("Here's the result :")

                    Console.WriteLine(decryptedMessage)
                    messageIsDecrypted = True
                Catch ex As Exception _
                    When TypeOf ex Is ArgumentException _
                    OrElse TypeOf ex Is FormatException _
                    OrElse TypeOf ex Is IndexOutOfRangeException _
                    OrElse TypeOf ex Is InvalidCastException
                    Console.WriteLine("Bad message or password!")
                End Try
            End While
            Console.WriteLine("Do you want to retry (r) ?")
            retry = Console.ReadLine()
        End While
    End Sub
End Module
