Due to the change in .NET 1.1, you must install CodeAccessSecurityAttribute(which SAFSecurityAttribute inherits)
to GAC in order to use it.  You would get a "Failed to load assembly SAF.Authorization" error when compiling the
Test.SAF.Authorization solution if you didn't add SAF.Authorization.dll to GAC.

To add SAF.Authorization.dll to GAC, use the following instruction:

1. Open the command prompt and navigate to "\SAF\SAF.Authorization\bin\Debug"
2. run "gacutil /i SAF.Authroization.dll" at the prompt.

Then compile the solution and hit F5 to run.

