'
'   CCCCC  PPPPP   MM   MM     Citrix
'  CC      PP   P  MMM MMM         Password
'  CC      PPPPP   MM M MM             Manager
'   CCCCC  PP      MM   MM
'
' Copyright 1990-2009 Citrix Systems, Inc.  All rights reserved.
'
' This file changes codepages; product code; package codes for  appropriate languages.
' arguments <language> , <projectName>

Option Explicit
Dim language,packageType, projectName
language=wscript.Arguments(0)
projectName=wscript.Arguments(1)
Const ForReading = 1
Const ForWriting = 2
Const TristateFalse = 0
Const msiViewModifyAssign         = 3

' source- compile.vbs IM tree
If language <> "" Then
    Dim languageCode, codePage, packageCode
    packageType = UCase(Right(projectName, 3))
      language = UCase(language)
    If language = "ENGLISH"  Or language = "EN" Then
      languageCode = 1033
      packageCode = "1033"
      codePage = 1252
    ElseIf language = "GERMAN" Or language = "DE" Then
      languageCode = 1031
      packageCode= "1031"
      codePage = 1252
    ElseIf language = "FRENCH" Or language = "FR" Then
      languageCode = 1036
      packageCode = "1036"
      codePage = 1252
    ElseIf language = "SPANISH" Or language = "ES" Then
      languageCode = 3082
      packageCode = "3082"
      codePage = 1252
    ElseIf language = "JAPANESE" Or language = "JA" Then
      languageCode = 1041
      packageCode =  "1041"
      codePage = 932
    ElseIf language = "KOREAN" Or language = "KO" Then
      languageCode = 1042
      packageCode =  "1042"
      codePage = 949
    ElseIf language = "CHINESESIMPLIFIED" Or language = "ZH-CN" Then
      languageCode = 2052
      packageCode =  "2052"
      codePage = 936
    ElseIf language = "CHINESETRADITIONAL" Or language = "ZH-TW" Then
      languageCode = 1028
      packageCode =  "1028"
      codePage = 950
    Else
      Wscript.Echo "ERROR: Unsupported language '" & language & "'"
      Wscript.Quit 1
    End If
    FixLanguageInformation projectName, "Codepage", codePage
    FixLanguageInformation projectName, "Package", packageCode
    FixLanguageInformation projectName, "Product", languageCode


End If 

Wscript.Echo "CodePageChange script complete"
Wscript.Quit 0

Sub FixLanguageInformation(package, key, code)
 Dim installer : Set installer = Nothing
  Set installer = Wscript.CreateObject("WindowsInstaller.Installer") 


  ' Open database
  Dim databasePath:databasePath = projectName

  Dim database : Set database = installer.OpenDatabase(databasePath, 1): CheckError

  ' Update value if supplied

	  Dim value:value = code
	  Select Case UCase(key)
		  Case "PACKAGE"  : SetPackageLanguage database, value
		  Case "PRODUCT"  : SetProductLanguage installer, database, value
		  Case "CODEPAGE" : SetDatabaseCodepage database, value
		  Case Else       : Fail "Invalid value keyword"
	  End Select
  ' Extract language info and compose report message

  
  database.Commit  ' no effect if opened ReadOnly
  Set database = nothing
 
 Set installer = nothing


End Sub


' Get language list from summary information
Function PackageLanguage(database)
	On Error Resume Next
	Dim sumInfo  : Set sumInfo = database.SummaryInformation(0) : CheckError
	Dim template : template = sumInfo.Property(7) : CheckError
	Dim iDelim:iDelim = InStr(1, template, ";", vbTextCompare)
	If iDelim = 0 Then template = "Not specified!"
	PackageLanguage = Right(template, Len(template) - iDelim)
	If Len(PackageLanguage) = 0 Then PackageLanguage = "0"
End Function

' Get ProductLanguge property from Property table
Function ProductLanguage(database)
	On Error Resume Next
	Dim view : Set view = database.OpenView("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductLanguage'")
	view.Execute : CheckError
	Dim record : Set record = view.Fetch : CheckError
	If record Is Nothing Then ProductLanguage = "Not specified!" Else ProductLanguage = record.IntegerData(1)
End Function

' Get ANSI codepage of database text data
Function DatabaseCodepage(database)
	On Error Resume Next
	Dim WshShell : Set WshShell = Wscript.CreateObject("Wscript.Shell") : CheckError
	Dim tempPath:tempPath = WshShell.ExpandEnvironmentStrings("%TEMP%") : CheckError
	database.Export "_ForceCodepage", tempPath, "codepage.idt" : CheckError
	Dim fileSys : Set fileSys = CreateObject("Scripting.FileSystemObject") : CheckError
	Dim file : Set file = fileSys.OpenTextFile(tempPath & "\codepage.idt", ForReading, False, TristateFalse) : CheckError
	file.ReadLine ' skip column name record
	file.ReadLine ' skip column defn record
	DatabaseCodepage = file.ReadLine
	Dim iDelim:iDelim = InStr(1, DatabaseCodepage, vbTab, vbTextCompare)
	If iDelim = 0 Then Fail "Failure in codepage export file"
	DatabaseCodepage = Left(DatabaseCodepage, iDelim - 1)
End Function



' Set ProductLanguge property in Property table
Sub SetProductLanguage(installer, database, language)
	On Error Resume Next
	If Not IsNumeric(language) Then Fail "ProductLanguage must be numeric"
	Dim view : Set view = database.OpenView("SELECT `Property`,`Value` FROM `Property`")
	view.Execute : CheckError
	Dim record : Set record = installer.CreateRecord(2)
	record.StringData(1) = "ProductLanguage"
	record.StringData(2) = CStr(language)
	view.Modify msiViewModifyAssign, record : CheckError
End Sub

' Set ANSI codepage of database text data
Sub SetDatabaseCodepage(database, codepage)
	On Error Resume Next
	If Not IsNumeric(codepage) Then Fail "Codepage must be numeric"
	Dim WshShell : Set WshShell = Wscript.CreateObject("Wscript.Shell") : CheckError
	Dim tempPath:tempPath = WshShell.ExpandEnvironmentStrings("%TEMP%") : CheckError
	Dim fileSys : Set fileSys = CreateObject("Scripting.FileSystemObject") : CheckError
	Dim file : Set file = fileSys.OpenTextFile(tempPath & "\codepage.idt", ForWriting, True, TristateFalse) : CheckError
	file.WriteLine ' dummy column name record
	file.WriteLine ' dummy column defn record
	file.WriteLine codepage & vbTab & "_ForceCodepage"
	file.Close : CheckError
	database.Import tempPath, "codepage.idt" : CheckError 
End Sub     

' Set language list in summary information
Sub SetPackageLanguage(database, language)
	On Error Resume Next
	Dim sumInfo  : Set sumInfo = database.SummaryInformation(1) 
	Dim template : template = sumInfo.Property(7) 
	Dim iDelim:iDelim = InStr(1, template, ";", vbTextCompare)
	Dim platform : If iDelim = 0 Then platform = ";" Else platform = Left(template, iDelim)
	sumInfo.Property(7) = platform & language
	sumInfo.Persist 
End Sub

Sub CheckError
	Dim message, errRec
	If Err = 0 Then Exit Sub
	message = Err.Source & " " & Hex(Err) & ": " & Err.Description
	If Not installer Is Nothing Then
		Set errRec = installer.LastErrorRecord
		If Not errRec Is Nothing Then message = message & vbNewLine & errRec.FormatText
	End If
	Fail message
End Sub

Sub Fail(message)
	Wscript.Echo message
	Wscript.Quit 2
End Sub


