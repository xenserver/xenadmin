if (WScript.Arguments.Length != 3) {
   WScript.echo("// MsiDiff.js");
   WScript.echo("// Usage: MsiDiff.js base.msi target.msi diff.mst");
   WScript.quit(0);
}
try {
  var installerObj = new ActiveXObject("WindowsInstaller.Installer");
  var baseDatabase = installerObj.OpenDatabase(WScript.Arguments.Item(0), 0);
  var targetDatabase = installerObj.OpenDatabase(WScript.Arguments.Item(1), 0);

  targetDatabase.GenerateTransform(baseDatabase, WScript.Arguments.Item(2));  
  targetDatabase.CreateTransformSummaryInfo(baseDatabase, WScript.Arguments.Item(2), 0, 0);
} catch (ex) {
  try { // for cscript.exe only; not for wscript.exe 
    WScript.StdErr.WriteLine("Error : " + ex.number + " : " + ex.description);
  } catch (ex2) { /* exception on wscript.exe; keep quiet to avoid pop up error dialogs */ }
}
