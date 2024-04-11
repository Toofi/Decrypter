Imports System.IO
Imports System.Net
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class DecrypterTests
    Public Shared ReadOnly _decrypter As New Decrypter()
    Private ReadOnly _path As String
    Private ReadOnly _lines As String()
    Private ReadOnly _block As String
    Private ReadOnly _password As String
    Private ReadOnly _message As String

    Sub New()
        _path = ".\Files\testBlock.txt"
        _lines = File.ReadAllLines(_path)
        _block = _lines(0)
        _password = _lines(1)
        _message = _lines(2)
    End Sub

    <TestMethod>
    Sub TheRightPasswordShouldDecryptTheMessage()
        Dim decryptedMessage As String = _decrypter.DecryptString(_block, _password)
        Assert.AreEqual(decryptedMessage, _message)
    End Sub

    <TestMethod>
    Sub WrongPasswordDoesNotDecrypt()
        Dim decryptedMessage As String = _decrypter.DecryptString(_block, "anotherPassword")
        Assert.AreNotEqual(decryptedMessage, _message)
    End Sub

    <TestMethod>
    Sub WrongNonceDoestNotDecrypt()
        Dim blockWithWrongNonce As String = _block.Substring(0, _block.Length - 4) & "7777"
        Dim decryptedMessage As String = _decrypter.DecryptString(blockWithWrongNonce, "password")
        Assert.AreNotEqual(decryptedMessage, _message)
    End Sub

End Class
